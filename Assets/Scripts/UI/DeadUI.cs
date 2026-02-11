using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadUI : MonoBehaviour
{
    [SerializeField] private GameObject Button;

    void Start()
    {
        Button.SetActive(false);
        DeadEvent.OnPlayerDead += DeadGame;
    }

    protected virtual void OnDestroy()
    {
        DeadEvent.OnPlayerDead -= DeadGame;
    }


    public void DeadGame()
    {
        Button.SetActive(true);
    }

    public void RestartGame()
    {
        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.buildIndex);
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {

    }
}
