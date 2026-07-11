// NPCInteract.cs
// Script responsável por permitir que o jogador interaja com um NPC

using UnityEngine; // Biblioteca principal da Unity

// Classe responsável pela interação com um NPC
public class NPCInteract : MonoBehaviour
{
    // Permite escrever várias linhas de texto no Inspector
    [TextArea(3, 6)]

    // Nome do NPC que será mostrado na caixa de diálogo
    public string speakerName = "Alma";

    // Lista das falas (linhas de diálogo) deste NPC
    public string[] dialogueLines;

    // Indica se o jogador está dentro da área de interação
    private bool playerInRange;

    // Executado uma vez por frame
    void Update()
    {
        // Se o jogador estiver na área de interação
        // e pressionar a tecla E
        if (playerInRange && Input.GetKeyDown(KeyCode.E))

            // Inicia o diálogo através do DialogueSystem
            DialogueSystem.Instance.StartDialogue(
                speakerName,   // Nome do NPC
                dialogueLines  // Linhas de diálogo
            );
    }

    // Executado quando outro Collider2D entra no Trigger deste objeto
    void OnTriggerEnter2D(Collider2D other)
    {
        // Se o objeto que entrou tiver a tag "Player"
        if (other.CompareTag("Player"))

            // Permite que o jogador possa iniciar o diálogo
            playerInRange = true;
    }

    // Executado quando outro Collider2D sai do Trigger deste objeto
    void OnTriggerExit2D(Collider2D other)
    {
        // Se o objeto que saiu tiver a tag "Player"
        if (other.CompareTag("Player"))

            // Impede que o jogador continue a interagir
            playerInRange = false;
    }
}