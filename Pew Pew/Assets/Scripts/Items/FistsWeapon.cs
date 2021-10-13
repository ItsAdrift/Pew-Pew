using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FistsWeapon : Item
{
    [SerializeField] Camera cam;

    float nextTimeToAttack = 0f;

    PhotonView PV;
    NotificationManager notificationManager;
    public Animator animator;

    void Awake()
    {
        PV = transform.root.GetComponent<PhotonView>();
        notificationManager = transform.root.GetComponentInChildren<NotificationManager>();
    }

    public override void Use()
    {
        MeleeInfo meleeInfo = (MeleeInfo)itemInfo;
        if (nextTimeToAttack == 0f || Time.time >= nextTimeToAttack)
        {
            Attack();
            nextTimeToAttack = Time.time + (1f / meleeInfo.attackRate);
        }

    }

    public void Attack()
    {
        MeleeInfo meleeInfo = (MeleeInfo)itemInfo;

        animator.Play("Punch");
        

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, meleeInfo.range))
        {
            if (hit.collider.gameObject.GetComponentInParent<IDamageable>() != null)
            {
                PhotonView hitView = hit.collider.gameObject.GetComponentInParent<PhotonView>();
                if (hitView.IsMine)
                    return;

                string hitNickName = hitView.Owner.NickName;

                bool isDead = hit.collider.gameObject.GetComponentInParent<IDamageable>().TakeDamage(meleeInfo.damage, PV.Owner.NickName);
                if (isDead)
                {
                    string message = hitNickName + " was killed by " + PV.Owner.NickName;
                    notificationManager.SendDelayedGlobalNotification(message, false, 0.1f);
                    notificationManager.SendNotification("You killed " + hitNickName);
                }
            }

        }
    }

}
