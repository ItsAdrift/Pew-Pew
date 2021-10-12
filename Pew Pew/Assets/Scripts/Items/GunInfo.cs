using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName ="Item/Weapons/Gun")]
public class GunInfo : ItemInfo
{
    public float fireRate;
    public float damage;
    public float accuracy;
    public float range;

    public float ammo;
    public float maxAmmo;
    public float reloadTime;

}
