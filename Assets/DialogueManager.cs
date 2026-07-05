using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class DialogueLine
{
    [TextArea(3, 10)]
    public string text;

    public bool hasChoices;

    public string choice1Text;
    public string choice2Text;

    public int nextLineIfChoice1;
    public int nextLineIfChoice2;

    public bool endDialogueAfterThis;
    public int nextLine;
}

public class DialogueManager : MonoBehaviour
{
    [Header("Referências da UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("Referências dos Botões")]
    public GameObject choicesContainer;
    public Button choice1Button;
    public TextMeshProUGUI choice1Text;
    public Button choice2Button;
    public TextMeshProUGUI choice2Text;

    [Header("Configurações")]
    public float typingSpeed = 0.2f;
    public DialogueLine[] conversation;

    [Header("Áudio")]
    public AudioClip typingSoundClip;

    private AudioSource audioSource;
    private int currentLineIndex = 0;
    private bool isTyping = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (choice1Button != null)
            choice1Button.onClick.AddListener(() => MakeChoice(1));

        if (choice2Button != null)
            choice2Button.onClick.AddListener(() => MakeChoice(2));

        if (choicesContainer != null)
            choicesContainer.SetActive(false);

        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (!dialoguePanel.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = conversation[currentLineIndex].text;
                isTyping = false;
                ShowChoicesIfAny();
            }
            else if (!conversation[currentLineIndex].hasChoices)
            {
                NextLine();
            }
        }
    }

    public void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        currentLineIndex = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";

        if (choicesContainer != null)
            choicesContainer.SetActive(false);

        string[] words = conversation[currentLineIndex].text.Split(' ');

        for (int i = 0; i < words.Length; i++)
        {
            if (i == 0)
                dialogueText.text = words[i];
            else
                dialogueText.text += " " + words[i];

            if (audioSource != null && typingSoundClip != null)
                audioSource.PlayOneShot(typingSoundClip);

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        ShowChoicesIfAny();
    }

    void ShowChoicesIfAny()
    {
        if (!conversation[currentLineIndex].hasChoices)
            return;

        if (choicesContainer != null)
            choicesContainer.SetActive(true);

        choice1Text.text = conversation[currentLineIndex].choice1Text;
        choice2Text.text = conversation[currentLineIndex].choice2Text;

        choice1Button.gameObject.SetActive(!string.IsNullOrEmpty(choice1Text.text));
        choice2Button.gameObject.SetActive(!string.IsNullOrEmpty(choice2Text.text));
    }

    void NextLine()
    {
        if (conversation[currentLineIndex].endDialogueAfterThis)
        {
            EndDialogue();
            return;
        }

        currentLineIndex = conversation[currentLineIndex].nextLine;

        if (currentLineIndex >= 0 && currentLineIndex < conversation.Length)
            StartCoroutine(TypeLine());
        else
            EndDialogue();
    }

    public void MakeChoice(int choice)
    {
        if (choicesContainer != null)
            choicesContainer.SetActive(false);

        int nextIndex = (choice == 1)
            ? conversation[currentLineIndex].nextLineIfChoice1
            : conversation[currentLineIndex].nextLineIfChoice2;

        // Se o próximo índice for -1, fecha o diálogo
        if (nextIndex == -1)
        {
            EndDialogue();
            return;
        }

        currentLineIndex = nextIndex;

        if (currentLineIndex >= 0 && currentLineIndex < conversation.Length)
            StartCoroutine(TypeLine());
        else
            EndDialogue();
    }

    void EndDialogue()
    {
        StopAllCoroutines();

        if (choicesContainer != null)
            choicesContainer.SetActive(false);

        dialoguePanel.SetActive(false);

        isTyping = false;
        currentLineIndex = 0;
    }
}