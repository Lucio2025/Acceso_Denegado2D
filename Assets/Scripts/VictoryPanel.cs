using UnityEngine;

public class VictoryPanel : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanelObject;
    [SerializeField] private UnityEngine.UI.Button restartButton;
    [SerializeField] private UnityEngine.UI.Button exitButton;

    private void Awake()
    {
        victoryPanelObject.SetActive(false);

        restartButton.onClick.AddListener(OnRestart);
        exitButton.onClick.AddListener(OnExit);
    }

    public void Show()
    {
        victoryPanelObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnRestart()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void OnExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}