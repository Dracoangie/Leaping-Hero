using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInfo", menuName = "Scriptable Objects/PlayerInfo")]
public class PlayerInfo : ScriptableObject
{
    public int deathsCount;

    [Header("Spawn")]
    public Vector3 spawnPoint = Vector3.zero;
    public int spawnPointID;
}
