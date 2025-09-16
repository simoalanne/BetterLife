using UnityEngine;
using System;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public class Dialogue
    {
        public string talkerName;
        [TextArea(3, 10)]
        public string sentence;
        public bool isPlayer = true;
        public bool isQuestion;
        public bool finishesDialogue;
        [Header("For questions only")]
        public Branch[] yesBranch;
        public Branch[] noBranch;

    }

    [System.Serializable]
    public class Branch
    {
        public string talkerName;
        [TextArea(3, 10)]
        public string sentence;
        public bool isPlayer = true;
        public bool finishesDialogue;
    }


    [SerializeField, Tooltip("Does dialog trigger when interacting with the object")] private bool _dialogTriggersByInteraction = true;
    [SerializeField] private Dialogue[] dialogue;
    [Tooltip("Leave empty if don't want to change the default sprite")]
    [SerializeField] private Sprite _playerSprite;
    [SerializeField] private Sprite _NPCSprite;
    
    public event Action onDialogueTrigger;
    
    public bool CanInteract { get; set; } = true;

    void Awake()
    {
        RemoveTrailingAndLeadingSpaces();
    }

    /// <summary>
    /// Checks for accidental leading and trailing spaces in the dialogue sentences and removes them.
    /// </summary>
    private void RemoveTrailingAndLeadingSpaces()
    {
        for (int i = 0; i < dialogue.Length; i++)
        {
            dialogue[i].sentence = dialogue[i].sentence.Trim();
            if (dialogue[i].isQuestion)
            {
                foreach (Branch branch in dialogue[i].yesBranch)
                {
                    branch.sentence = branch.sentence.Trim();
                }
                foreach (Branch branch in dialogue[i].noBranch)
                {
                    branch.sentence = branch.sentence.Trim();
                }
            }
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

    public void TriggerDialogue(Action onDialogueFinished = null)
    {
        onDialogueTrigger?.Invoke();
        DialogueManager.Instance.StartDialogue(dialogue, _playerSprite, _NPCSprite, onDialogueFinished);
    } 
} 
