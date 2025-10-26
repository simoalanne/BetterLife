using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using Helpers;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public enum DialogueState
    {
        YesClicked,
        NoClicked,
        DialogueFinished
    }

    public record ConversationChainItem(Func<bool> Condition, List<DialoguePart> ConversationParts);

    public class DialogueHandler : MonoBehaviour
    {
        private enum InputState
        {
            AwaitingInput,
            YesClicked,
            NoClicked,
            ContinueClicked
        }

        [Header("References")]
        [SerializeField] private HideableElement dialoguePanel;
        [SerializeField] private Image talkerImage;
        [SerializeField] private TMP_Text talkerName;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private HideableElement continueButton;
        [SerializeField] private HideableElement yesButton;
        [SerializeField] private HideableElement noButton;

        [Header("Customization")]
        [SerializeField] private float textTypeSpeed = 0.02f;

        private InputState _inputState;
        private bool _isDialogueActive;


        private void Awake()
        {
            Services.Register(this, persistent: true);
            dialoguePanel.InitialVisibility(false);
            continueButton.InitialVisibility(false);
            yesButton.InitialVisibility(false);
            noButton.InitialVisibility(false);
            yesButton.As<Button>().onClick.AddListener(() => _inputState = InputState.YesClicked);
            noButton.As<Button>().onClick.AddListener(() => _inputState = InputState.NoClicked);
            continueButton.As<Button>().onClick.AddListener(() => _inputState = InputState.ContinueClicked);
        }

        public void StartDialogue(List<DialoguePart> dialogue, Action<DialogueState> onStateChange = null,
            List<ConversationChainItem> conversationChain = null)
        {
            if (_isDialogueActive) return;
            var firstPart = dialogue.First();
            talkerImage.sprite = firstPart.talker.sprite;
            talkerName.text = firstPart.talker.displayName;
            dialogueText.text = string.Empty;
            continueButton.InitialVisibility(false);
            yesButton.InitialVisibility(false);
            noButton.InitialVisibility(false);

            StartCoroutine(HandleDialogue(dialogue, onStateChange, conversationChain));
        }

        private IEnumerator HandleDialogue(List<DialoguePart> dialogue, Action<DialogueState> onStateChange = null,
            List<ConversationChainItem> conversationChain = null, bool finalDialogue = true)
        {
            Services.InputManager.EnablePlayerInput(false);
            Services.GameTimer.IsPaused = true;

            if (!_isDialogueActive)
            {
                _isDialogueActive = true;
                yield return dialoguePanel.ToggleElement(true, dialoguePanel.Duration);
            }

            var branchMap = new Dictionary<InputState, (DialogueState state, PartType branch)>
            {
                { InputState.YesClicked, (DialogueState.YesClicked, PartType.YesBranch) },
                { InputState.NoClicked, (DialogueState.NoClicked, PartType.NoBranch) }
            };

            for (var i = 0; i < dialogue.Count; i++)
            {
                var part = dialogue[i];
                var talkerData = part.talker;
                talkerImage.sprite = talkerData.sprite;
                talkerName.text = talkerData.displayName;

                var isQuestion = part.partType is PartType.Question;
                continueButton.Toggle(!isQuestion);
                yesButton.Toggle(isQuestion);
                noButton.Toggle(isQuestion);

                // Don't yield type message so player can skip the part if they want to
                StartCoroutine(dialogueText.TypeMessage(part.sentence, textTypeSpeed));

                _inputState = InputState.AwaitingInput;
                yield return new WaitUntil(() => _inputState is not InputState.AwaitingInput);

                if (isQuestion)
                {
                    if (branchMap.TryGetValue(_inputState, out var branch))
                    {
                        onStateChange?.Invoke(branch.state);
                        var branchIndex = dialogue.FindIndex(i + 1, p => p.partType == branch.branch);
                        if (branchIndex != -1)
                        {
                            i = branchIndex - 1;
                            continue;
                        }
                    }
                }

                if (part.finishesDialogueEarly) break;

                // Skip remaining parts of a branch when it ends
                var nextIndex = i + 1;
                if (nextIndex >= dialogue.Count) break;

                var nextPart = dialogue[nextIndex];
                var stillInBranch = (part.partType is PartType.YesBranch && nextPart.partType is PartType.YesBranch) ||
                                    (part.partType is PartType.NoBranch && nextPart.partType is PartType.NoBranch);

                if (stillInBranch) continue;

                var jumpIndex =
                    dialogue.FindIndex(nextIndex, p => p.partType is PartType.Statement or PartType.Question);
                if (jumpIndex == -1) break;
                i = jumpIndex - 1;
            }

            foreach (var item in (conversationChain ?? Enumerable.Empty<ConversationChainItem>())
                     .Where(x => x.Condition()))
            {
                yield return HandleDialogue(item.ConversationParts, onStateChange, conversationChain!.Skip(1).ToList(),
                    finalDialogue: false);
            }

            if (!finalDialogue) yield break;

            Services.InputManager.EnablePlayerInput(true);
            Services.GameTimer.IsPaused = false;
            yield return dialoguePanel.ToggleElement(false, dialoguePanel.Duration);
            onStateChange?.Invoke(DialogueState.DialogueFinished);
            _isDialogueActive = false;
        }
    }
}
