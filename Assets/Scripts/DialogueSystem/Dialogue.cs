using System;
using UnityEngine;

namespace DialogueSystem
{
    [Serializable]
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

    [Serializable]
    public class Branch
    {
        public string talkerName;
        [TextArea(3, 10)]
        public string sentence;
        public bool isPlayer = true;
        public bool finishesDialogue;
    }
}
