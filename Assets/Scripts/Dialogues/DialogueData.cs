using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 5)]
    public string text;

    [Min(1)]
    public int fontSize = 22;

    [Min(0f)]
    public float textSpeed = 0.05f;
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public string characterName;
    public Sprite characterPortrait;

    [SerializeField]
    public DialogueLine[] dialogueLines;
}
