using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Gamemode", menuName = "Gamemode")]
public class Gamemode : ScriptableObject
{
    public string gamemodeName;
    public string gamemodeID;
    public string gamemodeDescription;
    public int gamemodeBaseSceneIndex;
}
