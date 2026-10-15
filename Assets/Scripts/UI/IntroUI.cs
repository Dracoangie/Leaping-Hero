using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private CanvasGroup menuGroup;
    [SerializeField] private string levelScene = "First_Level";
    [Header("Intro animation")]
    [SerializeField] private Animator introAnimator;
    [SerializeField] private string playTrigger = "Play";
    [SerializeField, Min(0.01f)] private float toStartVanish = 0.8f;
    [SerializeField, Min(0.01f)] private float flashDuration = 0.2f;
    [SerializeField, Min(0f)] private float flashHaloSize = 3f;
    [SerializeField, Min(0.01f)] private float vanishDuration = 0.8f;
    [Tooltip("Enable when your animation calls FinishIntro through an Animation Event.")]
    [SerializeField] private bool waitForAnimationEvent;
    [SerializeField] private UnityEvent onIntroStarted = new UnityEvent();

    private bool starting;
    private bool loading;
    private Graphic[] flashGraphics;
    private Color[] originalColors;
    private Outline[] flashHalos;

    public void PlayGame()
    {
        if (starting) return;
        starting = true;
        playButton.interactable = false;
        exitButton.interactable = false;
        menuGroup.blocksRaycasts = false;
        if (introAnimator != null && introAnimator.runtimeAnimatorController != null)
        {
            introAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            introAnimator.SetTrigger(playTrigger);
        }
        StartCoroutine(PlayIntro());
        onIntroStarted.Invoke();
    }

    private IEnumerator PlayIntro()
    {
        float elapsed = 0f;
        while (elapsed < toStartVanish)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        BeginFlash();
        elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / flashDuration);
            SetFlash(Mathf.Sin(progress * Mathf.PI));
            yield return null;
        }
        ClearFlash();
        elapsed = 0f;
        while (elapsed < vanishDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            menuGroup.alpha = 1f - Mathf.Clamp01(elapsed / vanishDuration);
            yield return null;
        }
        if (!waitForAnimationEvent) FinishIntro();
    }

    private void BeginFlash()
    {
        flashGraphics = menuGroup.GetComponentsInChildren<Graphic>();
        originalColors = new Color[flashGraphics.Length];
        flashHalos = new Outline[flashGraphics.Length];
        for (int i = 0; i < flashGraphics.Length; i++)
        {
            originalColors[i] = flashGraphics[i].color;
            // A separate effect preserves any existing outline on the graphic.
            Outline halo = flashGraphics[i].gameObject.AddComponent<Outline>();
            halo.effectColor = new Color(1f, 1f, 1f, 0f);
            halo.effectDistance = new Vector2(flashHaloSize, flashHaloSize);
            halo.useGraphicAlpha = true;
            flashHalos[i] = halo;
        }
    }

    private void SetFlash(float strength)
    {
        for (int i = 0; i < flashGraphics.Length; i++)
        {
            if (flashGraphics[i] == null) continue;
            Color white = new Color(1f, 1f, 1f, originalColors[i].a);
            flashGraphics[i].color = Color.Lerp(originalColors[i], white, strength);
            if (flashHalos[i] != null)
                flashHalos[i].effectColor = new Color(1f, 1f, 1f, strength);
        }
    }

    private void ClearFlash()
    {
        if (flashGraphics == null) return;
        for (int i = 0; i < flashGraphics.Length; i++)
        {
            if (flashGraphics[i] != null) flashGraphics[i].color = originalColors[i];
            if (flashHalos[i] != null)
            {
                flashHalos[i].enabled = false;
                Destroy(flashHalos[i]);
            }
        }
        flashGraphics = null;
        originalColors = null;
        flashHalos = null;
    }

    private void OnDisable()
    {
        ClearFlash();
    }

    public void FinishIntro()
    {
        if (!starting || loading) return;
        if (!Application.CanStreamedLevelBeLoaded(levelScene))
        {
            Debug.LogError($"Add {levelScene} to the Build Profile scene list.", this);
            StopAllCoroutines();
            ClearFlash();
            starting = false;
            menuGroup.alpha = 1f;
            menuGroup.blocksRaycasts = true;
            playButton.interactable = true;
            exitButton.interactable = true;
            return;
        }
        loading = true;
        PlayerInfo.StartNewSession();
        SceneManager.LoadScene(levelScene);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
