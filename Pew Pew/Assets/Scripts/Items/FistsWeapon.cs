using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FistsWeapon : Item
{
    [SerializeField] Camera cam;

    float nextTimeToAttack = 0f;

    PhotonView PV;
    PlayerController playerController;
    NotificationManager notificationManager;
    public Animator animator;

    void Awake()
    {
        PV = transform.root.GetComponent<PhotonView>();
        playerController = PV.gameObject.GetComponent<PlayerController>();
        notificationManager = PV.gameObject.GetComponent<NotificationManager>();
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

                Vector3 _position = hit.collider.gameObject.transform.root.position;
                Quaternion _rotation = hit.collider.gameObject.transform.rotation;

                bool isDead = hit.collider.gameObject.GetComponentInParent<IDamageable>().TakeDamage(meleeInfo.damage, PV.Owner.NickName);
                if (isDead)
                {
                    playerController.playerManager.AddKill(false);
                    DropManager.Instance.DropHealthpack(_position, _rotation);
                }

                playerController.HitOther();
            }

        }
    }

}
