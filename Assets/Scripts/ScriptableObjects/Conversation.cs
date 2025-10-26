using System;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem;
using Helpers;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

namespace ScriptableObjects
{
    public enum PartType
    {
        Statement,
        Question,
        YesBranch,
        NoBranch
    }

    [Serializable]
    public class ConversationWithTokens
    {
        public Conversation conversation;
        [SerializeField, AllowNesting, ReadOnly, UsedImplicitly, TextArea(3, 5)]
        private string availableTokens;

        public IReadOnlyList<string> Tokens;

        public ConversationWithTokens(List<string> tokens, Conversation conversation = null)
        {
            this.conversation = conversation;
            Tokens = tokens.AsReadOnly();
            availableTokens = string.Join("\n", tokens.Select(t => "{" + t + "}"));
        }

        public ConversationWithTokens(string token, Conversation conversation = null)
            : this(new List<string> { token }, conversation)
        {
        }
    }

    [Serializable]
    public struct DialoguePart
    {
        public Talker talker;
        public PartType partType;
        [AllowNesting, ShowIf(nameof(CanEndEarly))] public bool finishesDialogueEarly;
        private bool CanEndEarly => partType is PartType.YesBranch or PartType.NoBranch;

        [TextArea(5, 15)]
        public string sentence;
    }

    [CreateAssetMenu(fileName = "New Conversation", menuName = "ScriptableObjects/Conversation", order = 1)]
    public class Conversation : ScriptableObject
    {
        public List<DialoguePart> dialogueParts;
    }

    public static class ConversationExtensions
    {
        public static List<DialoguePart> InjectTokenValues(this ConversationWithTokens conv, params object[] values)
        {
            if (values.Length != conv.Tokens.Count)
                throw new ArgumentException("Number of provided values does not match number of tokens.");

            var dict = conv.Tokens
                .Select((token, index) => (token, values[index]))
                .ToDictionary(pair => pair.token.ToLower(), pair => pair.Item2);

            return conv.conversation.dialogueParts.Select(part =>
            {
                var newPart = part;
                newPart.sentence = newPart.sentence.InterpolateStrict(dict);
                return newPart;
            }).ToList();
        }
        
        public static void Start(this List<DialoguePart> dialogueParts, Action<DialogueState> onStateChange = null,
            List<ConversationChainItem> conversationChain = null) =>
            Services.DialogueHandler.StartDialogue(dialogueParts, onStateChange, conversationChain);

        public static void Start(this Conversation conversation, Action<DialogueState> onStateChange = null,
            List<ConversationChainItem> conversationChain = null) =>
            Services.DialogueHandler.StartDialogue(conversation.dialogueParts, onStateChange, conversationChain);
    }
}
