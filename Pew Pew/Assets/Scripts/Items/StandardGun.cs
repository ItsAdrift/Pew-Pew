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
    PlayerController playerController;
    NotificationManager notificationManager;

    void Awake()
    {
        PV = transform.root.GetComponent<PhotonView>();
        playerController = PV.gameObject.GetComponent<PlayerController>();
        notificationManager = PV.gameObject.GetComponent<NotificationManager>();
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
                float _damage = gunInfo.damage;
                if (hit.collider.name.Equals("HeadShot"))
                {
                    _damage = _damage * 1.25f;
                }
                PhotonView hitView = hit.collider.gameObject.GetComponentInParent<PhotonView>();
                if (hitView.IsMine)
                    return;

                string hitNickName = hitView.Owner.NickName;

                Vector3 _position = hit.collider.gameObject.transform.root.position;
                Quaternion _rotation = hit.collider.gameObject.transform.rotation;

                bool isDead = hit.collider.gameObject.GetComponentInParent<IDamageable>().TakeDamage(gunInfo.damage, PV.Owner.NickName);
                if (isDead)
                {
                    playerController.playerManager.AddKill();
                    DropManager.Instance.DropHealthpack(_position, _rotation);
                    // Moved to PlayerManager

                    //string message = hitNickName + " was killed by " + PV.Owner.NickName;
                    //notificationManager.SendDelayedGlobalNotification(message, false, 0.1f);
                    //notificationManager.SendNotification("You killed " + hitNickName);
                }

                playerController.HitOther();
            }
           

            PV.RPC("RPC_PlayImpactEffect", RpcTarget.All, hit.point, hit.normal, gunInfo.itemName);
            
        }
    }
}
