using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        startButton.onClick.AddListener(PlayButtonHandler);
        quitButton.onClick.AddListener(QuitButtonHandler);
    }

    public void PlayButtonHandler()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadGame()
    {
        
    }
    
    
    public void QuitButtonHandler()
    {
        Application.Quit();
    }    
}