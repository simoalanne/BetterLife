using System;
using UnityEngine;

namespace Home
{
    public class BlockHomeExit : MonoBehaviour
    {
        private DialogueTrigger _cantExitHomeDialogueTrigger;
        [SerializeField]
        private DialogueTrigger goodbyeNoteDialogueTrigger;
        [SerializeField] private SceneLoadTrigger normalExitSceneLoadTrigger;

        private void Awake()
        {
            if (Player.PlayerManager.Instance.HasReadGoodbyeNote) return;
            _cantExitHomeDialogueTrigger = GetComponent<DialogueTrigger>();
            normalExitSceneLoadTrigger.gameObject.SetActive(false);
            Action onGoodbyeNoteReadAction = null;
            onGoodbyeNoteReadAction = () =>
            {
                normalExitSceneLoadTrigger.gameObject.SetActive(true);
                goodbyeNoteDialogueTrigger.onDialogueTrigger -= onGoodbyeNoteReadAction;
                Destroy(this);
            };
            goodbyeNoteDialogueTrigger.onDialogueTrigger += onGoodbyeNoteReadAction;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _cantExitHomeDialogueTrigger?.TriggerDialogue();
            }
        }
    }
}