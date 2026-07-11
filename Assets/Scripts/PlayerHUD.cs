using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    // Mostra a vida do jogador e o painel de morte/recomecar.
    [Header("Vida")]
    public Health playerHealth;
    public Slider barraVida;
    public TMP_Text textoVida;

    [Header("Opcoes")]
    public GameObject optionsPanel;

    [Header("Morte")]
    public GameObject painelMorte;
    public GameObject painelOpcoesMorte;
    public TMP_Text mensagemMorte;
    public Button botaoReplay;
    public string textoMorte = "GAME OVER";
    public float atrasoOpcoesMorte = 3f;
    public string cenaInicial = "MainMenu";

    private Coroutine rotinaOpcoesMorte;

    void Start()
    {
        if (playerHealth == null)
        {
            // Tenta encontrar automaticamente a vida do jogador se nao estiver ligada no Inspector.
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<Health>();
                if (playerHealth == null) playerHealth = player.GetComponentInParent<Health>();
                if (playerHealth == null) playerHealth = player.GetComponentInChildren<Health>();
            }
        }

        if (playerHealth != null)
        {
            // A HUD atualiza-se atraves do evento disparado pelo componente Health.
            playerHealth.AoAlterarVida += AtualizarVida;
            AtualizarVida(playerHealth.vidaAtual, playerHealth.vidaMaxima);
        }

        if (painelMorte != null)
            painelMorte.SetActive(false);

        if (painelOpcoesMorte != null)
            painelOpcoesMorte.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
        Time.timeScale = 1f;

        if (mensagemMorte != null)
            mensagemMorte.text = textoMorte;

        if (botaoReplay != null)
            // O botao chama Recomecar sem precisar de configurar o evento manualmente.
            botaoReplay.onClick.AddListener(Recomecar);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !JogadorMorreu())
            ToggleOptions();

        // Permite recomecar tambem com a tecla R quando o painel de morte esta visivel.
        if (painelOpcoesMorte != null && painelOpcoesMorte.activeSelf && Input.GetKeyDown(KeyCode.R))
            Recomecar();
    }

    void OnDestroy()
    {
        if (playerHealth != null)
            // Evita manter eventos ligados quando a HUD e destruida.
            playerHealth.AoAlterarVida -= AtualizarVida;

        if (botaoReplay != null)
            botaoReplay.onClick.RemoveListener(Recomecar);
    }

    public void AtualizarVida(int vidaAtual, int vidaMaxima)
    {
        // Atualiza simultaneamente a barra e o texto, se existirem.
        if (barraVida != null)
        {
            barraVida.maxValue = vidaMaxima;
            barraVida.value = vidaAtual;
        }

        if (textoVida != null)
            textoVida.text = vidaAtual + " / " + vidaMaxima;
    }

    public void MostrarMorte()
    {
        if (mensagemMorte != null)
            mensagemMorte.text = textoMorte;

        if (painelMorte != null)
            painelMorte.SetActive(true);

        if (painelOpcoesMorte != null)
            painelOpcoesMorte.SetActive(false);

        if (rotinaOpcoesMorte != null)
            StopCoroutine(rotinaOpcoesMorte);

        rotinaOpcoesMorte = StartCoroutine(MostrarOpcoesMorteDepoisDoAtraso());
    }

    public void Recomecar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SairDoJogo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(cenaInicial);
    }

    public void ToggleOptions()
    {
        if (optionsPanel != null)
            SetOptionsActive(!optionsPanel.activeSelf);
    }

    public void OpenOptions()
    {
        if (optionsPanel != null)
            SetOptionsActive(true);
    }

    public void CloseOptions()
    {
        if (optionsPanel != null)
            SetOptionsActive(false);
    }

    private bool JogadorMorreu()
    {
        return playerHealth != null && playerHealth.isDead;
    }

    private void SetOptionsActive(bool active)
    {
        if (optionsPanel == null)
            return;

        optionsPanel.SetActive(active);
        Time.timeScale = active ? 0f : 1f;
    }

    private IEnumerator MostrarOpcoesMorteDepoisDoAtraso()
    {
        yield return new WaitForSeconds(atrasoOpcoesMorte);

        if (painelOpcoesMorte != null)
            painelOpcoesMorte.SetActive(true);
    }
}
