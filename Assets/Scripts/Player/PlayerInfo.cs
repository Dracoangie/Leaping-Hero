using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInfo", menuName = "Scriptable Objects/PlayerInfo")]
public class PlayerInfo : ScriptableObject
{
    public int deathsCount;

    [Header("Abilities")]
    public bool doubleJumpUnlocked;
    public bool dashUnlocked;
    public int chestsOpened;

    [Header("Spawn")]
    public Vector3 spawnPoint = Vector3.zero;
    public int spawnPointID = -1;
    public string spawnScenePath;

    private static int sessionVersion;
    [System.NonSerialized] private int initializedSessionVersion = -1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void StartNewSession()
    {
        sessionVersion++;
    }

    public void InitializeSession()
    {
        if (initializedSessionVersion == sessionVersion) return;

        deathsCount = 0;
        doubleJumpUnlocked = false;
        dashUnlocked = false;
        chestsOpened = 0;
        ResetCheckpoint();
        initializedSessionVersion = sessionVersion;
    }

    public void ResetCheckpoint()
    {
        spawnPoint = Vector3.zero;
        spawnPointID = -1;
        spawnScenePath = string.Empty;
    }
}
