// NPCInteract.cs
using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    [TextArea(3, 6)]
    public string speakerName = "Alma";
    public string[] dialogueLines;

    private bool playerInRange;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
            DialogueSystem.Instance.StartDialogue(speakerName, dialogueLines);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }
}