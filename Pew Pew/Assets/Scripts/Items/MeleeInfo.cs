using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Melee", menuName = "Item/Weapons/Melee")]
public class MeleeInfo : ItemInfo
{
    public float attackRate;
    public float damage;
    public float range;
}
