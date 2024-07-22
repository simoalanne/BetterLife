using UnityEngine;

public class GoodbyeNote : MonoBehaviour
{
    public void Exit()
    {
        gameObject.SetActive(false);
        GameObject.Find("OpenLetter").GetComponent<DialogueTrigger>().TriggerDialogue();
    }
}
