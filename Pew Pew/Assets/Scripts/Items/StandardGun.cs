using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class StandardGun : Gun
{
    [SerializeField] Camera cam;
   
    float nextTimeToFire = 0f;

    PhotonView PV;
    NotificationManager notificationManager;

    void Awake()
    {
        PV = transform.root.GetComponent<PhotonView>();
        notificationManager = transform.root.GetComponentInChildren<NotificationManager>();
    }

    public override void Use()
    {
        GunInfo gunInfo = (GunInfo) itemInfo;
        if (nextTimeToFire == 0f || Time.time >= nextTimeToFire)
        {
            Shoot();
            nextTimeToFire = Time.time + (1f / gunInfo.fireRate);
        }
        
    }

    public void Shoot()
    {
        GunInfo gunInfo = (GunInfo) itemInfo;

        particles.Play();
        PV.RPC("RPC_PlaySoundEffect", RpcTarget.All, gunInfo.itemName);

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, gunInfo.range))
        {
            if (hit.collider.gameObject.GetComponentInParent<IDamageable>() != null)
            {
                string hitNickName = hit.collider.gameObject.GetComponentInParent<PhotonView>().Owner.NickName;

                bool isDead = hit.collider.gameObject.GetComponentInParent<IDamageable>().TakeDamage(gunInfo.damage, PV.Owner.NickName);
                if (isDead)
                {
                    string message = hitNickName + " was killed by " + PV.Owner.NickName;
                    notificationManager.SendDelayedGlobalNotification(message, false, 0.1f);
                    notificationManager.SendNotification("You killed " + hitNickName);
                }
            }
           

            PV.RPC("RPC_PlayImpactEffect", RpcTarget.All, hit.point, hit.normal, gunInfo.itemName);
            
        }
    }
}
