using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("Referências")]
    public DialogueManager dialogueManager;
    public GameObject exclamationMark;

    private bool playerNearby = false;
    private bool dialogueStarted = false;

    void Start()
    {
        // Esconde o ponto de exclamação ao iniciar o jogo
        if (exclamationMark != null)
            exclamationMark.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && !dialogueStarted && Input.GetKeyDown(KeyCode.E))
        {
            dialogueStarted = true;

            // Esconde o ponto de exclamação quando o diálogo começa
            if (exclamationMark != null)
                exclamationMark.SetActive(false);

            dialogueManager.StartDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            // Mostra o ponto de exclamação quando o jogador entra na zona
            if (exclamationMark != null)
                exclamationMark.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            dialogueStarted = false;

            // Esconde o ponto de exclamação quando o jogador sai da zona
            if (exclamationMark != null)
                exclamationMark.SetActive(false);
        }
    }
}