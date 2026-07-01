using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Vida")]
    public Health playerHealth;
    public Slider barraVida;
    public TMP_Text textoVida;

    [Header("Morte")]
    public GameObject painelMorte;
    public TMP_Text mensagemMorte;
    public Button botaoReplay;
    public string textoMorte = "GAME OVER";

    void Start()
    {
        if (playerHealth == null)
        {
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
            playerHealth.AoAlterarVida += AtualizarVida;
            AtualizarVida(playerHealth.vidaAtual, playerHealth.vidaMaxima);
        }

        if (painelMorte != null)
            painelMorte.SetActive(false);

        if (mensagemMorte != null)
            mensagemMorte.text = textoMorte;

        if (botaoReplay != null)
            botaoReplay.onClick.AddListener(Recomecar);
    }

    void Update()
    {
        if (painelMorte != null && painelMorte.activeSelf && Input.GetKeyDown(KeyCode.R))
            Recomecar();
    }

    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.AoAlterarVida -= AtualizarVida;

        if (botaoReplay != null)
            botaoReplay.onClick.RemoveListener(Recomecar);
    }

    public void AtualizarVida(int vidaAtual, int vidaMaxima)
    {
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
    }

    public void Recomecar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
