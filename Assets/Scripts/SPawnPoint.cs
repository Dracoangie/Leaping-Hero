using UnityEditor.Build.Content;
using UnityEngine;

public class SPawnPoint : MonoBehaviour
{
    [SerializeField] private Vector3 SpawnPoint = Vector3.zero;
    [SerializeField] private int id = 0;

    void Awake()
    {
        if(SpawnPoint == Vector3.zero)
            SpawnPoint = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerInfo playerInfo = other.GetComponent<PlayerMovement>().getPlayerInfo();

        if(playerInfo.spawnPointID < id)
        {
            playerInfo.spawnPointID = id;
            playerInfo.spawnPoint = SpawnPoint;
        }

        other.GetComponent<PlayerMovement>().setPlayerInfo(playerInfo);
    }
}
