using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject playButton;
    public GameObject settingsCloseButton;

    public string gameSceneName = "GameScene";

    void Start()
    {
        SelectButton(playButton);
    }

    void SelectButton(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);

        SelectButton(settingsCloseButton);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);

        SelectButton(playButton);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}