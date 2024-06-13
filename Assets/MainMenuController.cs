using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameplayPanel;

    private void Awake()
    {
        startButton.onClick.AddListener(PlayButtonHandler);
        quitButton.onClick.AddListener(QuitButtonHandler);
    }

    public void PlayButtonHandler()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void ReturnToMainMenu()
    {
        mainMenuPanel.SetActive(true);
        gameplayPanel.SetActive(false);
    }
    IEnumerator LoadingScene(string nameScene)
    {
     
     
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nameScene, LoadSceneMode.Single);
 
        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / .9f);
            yield return null;
        }
    }
    
    public void QuitButtonHandler()
    {
        Application.Quit();
    }    
}