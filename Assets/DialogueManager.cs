using System.Collections; //Biblioteca usada para usar Coroutines (IEnumerator)
using UnityEngine;  //Biblioteca usada para acessar componentes do Unity
using TMPro;    //Biblioteca dedicada para o TextMeshPro, que é uma ferramenta de renderização de texto avançada no Unity
using UnityEngine.UI;  //Biblioteca usada para acessar componentes de UI do Unity, como botões e textos

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

        StartDialogue();
    }

    void Update()
    {
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
            dialoguePanel.SetActive(false);
            return;
        }

        currentLineIndex = conversation[currentLineIndex].nextLine;

        if (currentLineIndex >= 0 && currentLineIndex < conversation.Length)
            StartCoroutine(TypeLine());
        else
            dialoguePanel.SetActive(false);
    }

    public void MakeChoice(int choice)
    {
        choicesContainer.SetActive(false);

        if (choice == 1)
            currentLineIndex = conversation[currentLineIndex].nextLineIfChoice1;
        else
            currentLineIndex = conversation[currentLineIndex].nextLineIfChoice2;

        if (currentLineIndex >= 0 && currentLineIndex < conversation.Length)
            StartCoroutine(TypeLine());
        else
            dialoguePanel.SetActive(false);
    }
}