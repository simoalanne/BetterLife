using UnityEngine;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public class Dialogue
    {
        public string talkerName;
        [TextArea(3, 10)]
        public string sentence;
        public bool isPlayer;
        public bool isQuestion;
        public bool isOptionalSentence;
        public bool overwrittenByOptionalSentence;
        public bool isYesBranch;
        public bool isNoBranch;
    }
    [SerializeField, Tooltip("Does dialog trigger when interacting with the object")] private bool _dialogTriggersByInteraction = true;
    [SerializeField] private Dialogue[] dialogue;
    [SerializeField] private Sprite _playerSprite;
    [SerializeField] private Sprite _NPCSprite;
    private Dialogue[] fullDialogueCopy;

    void Awake()
    {
        RemoveTrailingAndLeadingSpaces();
        fullDialogueCopy = new Dialogue[dialogue.Length];
        dialogue.CopyTo(fullDialogueCopy, 0);

    }

    /// <summary>
    /// Checks for accidental leading and trailing spaces in the dialogue sentences and removes them.
    /// </summary>
    private void RemoveTrailingAndLeadingSpaces()
    {
        for (int i = 0; i < dialogue.Length; i++)
        {
            dialogue[i].sentence = dialogue[i].sentence.Trim();
        }
    }

    public void Interact()
    {
        if (_dialogTriggersByInteraction)
        {
            TriggerDialogue();
        }
        else
        {
            Debug.Log("Dialogue trigger is set to not trigger by interaction.");
        }
    }

    public void TriggerDialogue()
    {
        dialogue = new Dialogue[fullDialogueCopy.Length];
        fullDialogueCopy.CopyTo(dialogue, 0);

        TryGetComponent<IOptionalSentence>(out var optionalSentence);

        List<Dialogue> dialogueList = new List<Dialogue>(dialogue);

        for (int i = dialogueList.Count - 1; i >= 0; i--)
        {
            var dialoguePart = dialogueList[i];
            if (dialoguePart.isOptionalSentence && optionalSentence.DisplayOptionalSentence() == false)
            {
                Debug.Log("Removed sentence because not to be displayed: " + dialoguePart.sentence);
                dialogueList.RemoveAt(i);
            }
            else if (dialoguePart.overwrittenByOptionalSentence && optionalSentence.DisplayOptionalSentence() == true)
            {
                Debug.Log("Removed sentence because overwritten by optional sentence: " + dialoguePart.sentence);
                dialogueList.RemoveAt(i);
            }
        }

        dialogue = dialogueList.ToArray();
        Debug.Log("Dialogue length: " + dialogue.Length);
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue, _playerSprite, _NPCSprite);
    }
}