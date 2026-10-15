using UnityEngine;

public class SPawnPoint : MonoBehaviour
{
    [SerializeField] private Vector3 SpawnPoint = Vector3.zero;
    [SerializeField] private bool useTransformPosition;
    [SerializeField] private int id = 0;

    void Awake()
    {
        if(useTransformPosition)
            SpawnPoint = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerInfo playerInfo = other.GetComponent<PlayerMovement>().getPlayerInfo();

        playerInfo.spawnPointID = id;
        playerInfo.spawnPoint = SpawnPoint;
        playerInfo.spawnScenePath = other.gameObject.scene.path;

        other.GetComponent<PlayerMovement>().setPlayerInfo(playerInfo);
    }
}
