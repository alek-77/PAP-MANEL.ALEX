// NPCInteract.cs
using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    // Guarda os dados do dialogo que este NPC vai enviar ao DialogueSystem.
    [TextArea(3, 6)]
    public string speakerName = "Alma";
    public string[] dialogueLines;

    private bool playerInRange;

    void Update()
    {
        // O dialogo comeca quando o jogador esta perto e carrega em E.
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
            DialogueSystem.Instance.StartDialogue(speakerName, dialogueLines);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // O trigger define se o jogador esta dentro da zona de interacao.
        if (other.CompareTag("Player")) playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Fora da zona, o jogador deixa de poder iniciar este dialogo.
        if (other.CompareTag("Player")) playerInRange = false;
    }
}
