using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private UnityEngine.UI.Button playButton;
    [SerializeField] private UnityEngine.UI.Button exitButton;

    private void Start()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        menuPanel.SetActive(true);

        playButton.onClick.AddListener(OnPlay);
        exitButton.onClick.AddListener(OnExit);
    }

    public void OnPlay()
    {
        menuPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void OnExit()
    {
        Application.Quit();
    }
}