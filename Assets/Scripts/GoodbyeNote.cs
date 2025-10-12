using DialogueSystem;
using UnityEngine;

public class GoodbyeNote : MonoBehaviour
{
    public void Exit()
    {
        gameObject.SetActive(false);
        Services.PlayerManager.HasReadGoodbyeNote = true;
        GameObject.Find("OpenLetter").GetComponent<DialogueTrigger>().TriggerDialogue();
    }
}
