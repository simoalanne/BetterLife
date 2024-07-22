using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Player;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI talkerName;
    public TextMeshProUGUI dialogueText;
    public float textSpeed;
    private bool _isYesResponse;
    [SerializeField] private Animator animator;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;
    [SerializeField] private Image _talkerImage;
    private Sprite playerSprite;
    private Sprite talkerSprite;
    
    private Queue<DialogueTrigger.Dialogue> dialogueParts;

    void Awake()
    {
        dialogueParts = new Queue<DialogueTrigger.Dialogue>();
    }

    public void StartDialogue(DialogueTrigger.Dialogue[] dialogue, Sprite playerSprite, Sprite talkerSprite)
    {
        this.playerSprite = playerSprite;
        this.talkerSprite = talkerSprite;
        GameTimer.Instance.IsPaused = true;
        PlayerManager.Instance.DisablePlayerMovement();
        PlayerManager.Instance.DisablePlayerInteract();
        _continueButton.gameObject.SetActive(false);
        _yesButton.gameObject.SetActive(false);
        _noButton.gameObject.SetActive(false);
        animator.SetBool("IsOpen", true);

        foreach (var dialoguePart in dialogue)
        {
            dialogueParts.Enqueue(dialoguePart);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (dialogueParts.Count == 0)
        {
            EndDialogue();
            return;
        }

        var dialoguePart = dialogueParts.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(dialoguePart));
    }

    private IEnumerator TypeSentence(DialogueTrigger.Dialogue dialoguePart)
    {
        if (dialoguePart.isYesBranch && !_isYesResponse)
        {
            HandleResponse(null);
            yield break;
        }

        if (dialoguePart.isNoBranch && _isYesResponse)
        {
            HandleResponse(null);
            yield break;
        }
        _talkerImage.sprite = dialoguePart.isPlayer ? playerSprite : talkerSprite;
        talkerName.text = dialoguePart.talkerName;
        dialogueText.text = "";
        foreach (char letter in dialoguePart.sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        if (dialoguePart.isQuestion)
        {
            _yesButton.gameObject.SetActive(true);
            _noButton.gameObject.SetActive(true);
            _continueButton.gameObject.SetActive(false);

            _yesButton.onClick.RemoveAllListeners();
            _noButton.onClick.RemoveAllListeners();
            _yesButton.onClick.AddListener(() => YesClicked());
            _noButton.onClick.AddListener(() => NoClicked());
        }
        else
        {
            _continueButton.gameObject.SetActive(true);
            _yesButton.gameObject.SetActive(false);
            _noButton.gameObject.SetActive(false);
        }
    }

    private void HandleResponse(DialogueTrigger.Dialogue response)
    {
        _yesButton.gameObject.SetActive(false);
        _noButton.gameObject.SetActive(false);
        _continueButton.gameObject.SetActive(true);
        if (response != null)
        {
            dialogueParts.Enqueue(response);
        }

        DisplayNextSentence();
    }

    void YesClicked()
    {
        _isYesResponse = true;
        DisplayNextSentence();
    }

    void NoClicked()
    {
        _isYesResponse = false;
        DisplayNextSentence();
    }

    private void EndDialogue()
    {
        PlayerManager.Instance.EnablePlayerMovement();
        PlayerManager.Instance.EnablePlayerInteract();
        Debug.Log("End of conversation.");
        animator.SetBool("IsOpen", false);
        GameTimer.Instance.IsPaused = false;
    }
}