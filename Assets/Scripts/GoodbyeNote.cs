using Player;
using UnityEngine;

public class GoodbyeNote : MonoBehaviour
{
    public void Exit()
    {
        gameObject.SetActive(false);
        PlayerManager.Instance.HasReadGoodbyeNote = true;
        GameObject.Find("OpenLetter").GetComponent<DialogueTrigger>().TriggerDialogue();
    }
}
