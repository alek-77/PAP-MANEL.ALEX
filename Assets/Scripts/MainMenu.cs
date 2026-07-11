using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Painel de opções")]
    public GameObject optionsPanel;

    [Header("Nome da cena do jogo")]
    public string gameSceneName = "SYBAU";

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
    }

    public void CloseOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    public void ExitGame()
    {
        Debug.Log("A sair do jogo...");
        Application.Quit();
    }
}