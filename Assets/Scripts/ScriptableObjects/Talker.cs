using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Talker", menuName = "ScriptableObjects/Talker", order = 0)]
    public class Talker : ScriptableObject
    {
        public Sprite sprite;
        public string displayName;
    }
}
