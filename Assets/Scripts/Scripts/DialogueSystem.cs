// DialogueSystem.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance;

    [Header("UI")]
    public GameObject dialogueBox;
    public Text speakerName;
    public Text dialogueText;

    [Header("Settings")]
    public float typingSpeed = 0.04f;

    private string[] currentLines;
    private int currentIndex;
    private bool isTyping;
    private bool dialogueActive;

    void Awake() => Instance = this;

    public void StartDialogue(string speaker, string[] lines)
    {
        dialogueBox.SetActive(true);
        speakerName.text = speaker;
        currentLines = lines;
        currentIndex = 0;
        dialogueActive = true;
        StartCoroutine(TypeLine(lines[0]));
    }

    void Update()
    {
        if (!dialogueActive) return;

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = currentLines[currentIndex];
                isTyping = false;
            }
            else
            {
                currentIndex++;
                if (currentIndex >= currentLines.Length)
                    EndDialogue();
                else
                    StartCoroutine(TypeLine(currentLines[currentIndex]));
            }
        }
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    void EndDialogue()
    {
        dialogueActive = false;
        dialogueBox.SetActive(false);
    }
}