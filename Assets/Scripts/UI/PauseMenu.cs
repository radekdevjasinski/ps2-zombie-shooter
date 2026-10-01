using UnityEngine;
using UnityEngine.Serialization;

public class PauseMenu : MonoBehaviour
{
    public static bool IsGamePaused { get; private set; }

    public GameObject pauseMenu;
    [FormerlySerializedAs("x")]
    public GameObject crosshair;

    void Awake()
    {
        ApplyPauseState(false);
    }

    void OnDestroy()
    {
        ApplyPauseState(false);
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }

        if (IsGamePaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        ApplyPauseState(true);
        pauseMenu.SetActive(true);
        crosshair.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        ApplyPauseState(false);
        pauseMenu.SetActive(false);
        crosshair.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private static void ApplyPauseState(bool isPaused)
    {
        IsGamePaused = isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        AudioListener.pause = isPaused;
    }
}
