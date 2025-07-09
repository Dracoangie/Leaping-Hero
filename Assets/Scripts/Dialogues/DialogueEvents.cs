using System;

public static class DialogueEvents
{
    public static event Action<DialogueData> OnDialogueTriggered;
    public static event Action OnDialogueEnded;

    public static void TriggerDialogue(DialogueData dialogue)
    {
        OnDialogueTriggered?.Invoke(dialogue);
    }

    public static void EndDialogue()
    {
        OnDialogueEnded?.Invoke();
    }
}
