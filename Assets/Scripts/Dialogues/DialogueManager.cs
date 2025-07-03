using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    private DialogueData currentDialogue;
    private int currentLine = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentFullLine;

    void OnEnable()
    {
        DialogueEvents.OnDialogueTriggered += StartDialogue;
    }

    void OnDisable()
    {
        DialogueEvents.OnDialogueTriggered -= StartDialogue;
    }

    public void StartDialogue(DialogueData dialogue)
    {
        currentDialogue = dialogue;
        currentLine = 0;
        dialoguePanel.SetActive(true);
        ShowLine(currentLine);
    }

    void ShowLine(int index)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        DialogueLine line = currentDialogue.dialogueLines[index];
        dialogueText.fontSize = line.fontSize;
        currentFullLine = line.text;
        typingCoroutine = StartCoroutine(TypeLine(currentFullLine, line.textSpeed));
    }

    System.Collections.IEnumerator TypeLine(string text, float speed)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(speed);
        }
        isTyping = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!dialoguePanel.activeSelf) return;

            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentFullLine;
                isTyping = false;
            }
            else
            {
                currentLine++;
                if (currentLine < currentDialogue.dialogueLines.Length)
                {
                    ShowLine(currentLine);
                }
                else
                {
                    dialoguePanel.SetActive(false);
                }
            }
        }
    }
}
