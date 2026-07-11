// DialogueSystem.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    // Singleton simples para os NPCs iniciarem dialogos.
    public static DialogueSystem Instance;

    [Header("UI")]
    public GameObject dialogueBox;
    public Text speakerName;
    public Text dialogueText;

    [Header("Definicoes")]
    public float typingSpeed = 0.04f;

    private string[] currentLines;
    private int currentIndex;
    private bool isTyping;
    private bool dialogueActive;

    void Awake() => Instance = this;

    public void StartDialogue(string speaker, string[] lines)
    {
        // Mostra a caixa, guarda as falas atuais e comeca pela primeira linha.
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
                // Se ainda esta a escrever, mostra logo a linha completa.
                StopAllCoroutines();
                dialogueText.text = currentLines[currentIndex];
                isTyping = false;
            }
            else
            {
                // Se a linha ja acabou, avanca para a proxima ou termina o dialogo.
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
        // Escreve a fala letra a letra para criar efeito de maquina de escrever.
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
        // Fecha a caixa e marca o dialogo como inativo.
        dialogueActive = false;
        dialogueBox.SetActive(false);
    }
}
