using System;

public static class DialogueEvents
{
    public static event Action<DialogueData> OnDialogueTriggered;

    public static void TriggerDialogue(DialogueData dialogue)
    {
        OnDialogueTriggered?.Invoke(dialogue);
    }
}
