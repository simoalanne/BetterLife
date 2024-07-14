using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Player;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public float textSpeed;

    public Animator animator;

    // Stores dialogue, First in, First out
    private Queue<string> sentences;

    void Start()
    {
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false; // Prevents raycasts from hitting the dialogue box
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        GameTimer.Instance.IsPaused = true;
        PlayerManager.Instance.DisablePlayerMovement();
        PlayerManager.Instance.DisablePlayerInteract();
        gameObject.GetComponent<CanvasGroup>().alpha = 1;
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true; // Allows raycasts to hit the dialogue box
        animator.SetBool("IsOpen", true);

        nameText.text = dialogue.name;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0) 
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void EndDialogue()
    {
        PlayerManager.Instance.EnablePlayerMovement();
        PlayerManager.Instance.EnablePlayerInteract();
        Debug.Log("End of conversation.");
        animator.SetBool("IsOpen", false);
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        GameTimer.Instance.IsPaused = false;
    }
}
