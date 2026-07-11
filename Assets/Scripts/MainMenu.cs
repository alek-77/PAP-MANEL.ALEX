using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Funcoes ligadas aos botoes do menu principal.
    [Header("Painel de opcoes")]
    public GameObject optionsPanel;

    [Header("Nome da cena do jogo")]
    public string gameSceneName = "GameScene";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOptions();
        }
    }

    public void StartGame()
    {
        // Carrega a cena definida no Inspector.
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenOptions()
    {
        // Mostra o painel de opcoes se ele existir.
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
    }

    public void CloseOptions()
    {
        // Esconde o painel de opcoes se ele existir.
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    public void ToggleOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(!optionsPanel.activeSelf);
        }
    }

    public void ExitGame()
    {
        // No editor apenas aparece o log; numa build fecha a aplicacao.
        Debug.Log("A sair do jogo...");
        Application.Quit();
    }
}
