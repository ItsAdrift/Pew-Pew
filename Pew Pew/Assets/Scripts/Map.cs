using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map", menuName = "Map")]
public class Map : ScriptableObject
{
    public string mapName;
    public string mapID;
    public GameObject mapObject;
    public Sprite mapImage;
    public float mapHeight = 0f;
}
