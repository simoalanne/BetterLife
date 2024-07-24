using System.Collections;
using TMPro;
using UnityEngine;
using Player;
using UnityEngine.UI;
/*
public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI talkerName;
    public TextMeshProUGUI dialogueText;
    public float textSpeed;
    [SerializeField] private Animator animator;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;
    [SerializeField] private Image _talkerImage;
    [Tooltip("Player sprite used by default if none set in DialogueTrigger.cs")]
    [SerializeField] private Sprite defaultPlayerSprite;
    private Sprite playerSprite;
    private Sprite talkerSprite;
    private DialogueTrigger.Dialogue[] _dialogue;
    private int _nonBranchIndex = 0;
    private int _yesOrNoBranchIndex = 0;
    private bool _isInBranch = false;

    void Awake()
    {
        _continueButton.gameObject.SetActive(false);
        _yesButton.gameObject.SetActive(false);
        _noButton.gameObject.SetActive(false);
    }

    public void StartDialogue(DialogueTrigger.Dialogue[] dialogue, Sprite playerSprite, Sprite talkerSprite)
    {
        _dialogue = dialogue;
        InitializeDialogue(playerSprite, talkerSprite);
        HandlePart();
    }

    void InitializeDialogue(Sprite playerSprite, Sprite talkerSprite)
    {
        this.playerSprite = playerSprite != null ? playerSprite : defaultPlayerSprite;
        this.talkerSprite = talkerSprite;
        GameTimer.Instance.IsPaused = true;
        PlayerManager.Instance.DisableInputs();

        _continueButton.gameObject.SetActive(false);
        _yesButton.gameObject.SetActive(false);
        _noButton.gameObject.SetActive(false);
        animator.SetBool("IsOpen", true);
    }

    IEnumerator TypeSentence(string sentence, string talkerName, bool isPlayer)
    {

        talkerName.text = dialoguePart.talkerName;
        _talkerImage.sprite = dialoguePart.isPlayer ? playerSprite : talkerSprite;
        dialogueText.text = "";
        var sentence = dialoguePart.sentence;

        if (dialoguePart.finishesDialogue)
        {
            EnableButtons(dialoguePart.isQuestion, true);
        }
        else
        {
            EnableButtons(dialoguePart.isQuestion, false);
        }

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void EnableButtons(bool isQuestion, bool finishesDialogue = false)
    {
        _yesButton.onClick.RemoveAllListeners();
        _noButton.onClick.RemoveAllListeners();
        _continueButton.onClick.RemoveAllListeners();

        if (isQuestion)
        {
            _yesButton.gameObject.SetActive(true);
            _noButton.gameObject.SetActive(true);
            if (finishesDialogue)
            {
                _yesButton.onClick.AddListener(EndDialogue);
                _noButton.onClick.AddListener(EndDialogue);
            }
            else
            {
                _yesButton.onClick.AddListener(HandlePart);
                _noButton.onClick.AddListener(HandlePart);
            }
            _continueButton.gameObject.SetActive(false);
        }
        else
        {
            if (finishesDialogue)
            {
                _continueButton.onClick.AddListener(EndDialogue);
            }
            else
            {
                _continueButton.onClick.AddListener(HandlePart);
            }
            _continueButton.gameObject.SetActive(true);
            _yesButton.gameObject.SetActive(false);
            _noButton.gameObject.SetActive(false);
        }
    }

    void HandlePart()
    {
        var part = _dialogue[_nonBranchIndex];
        if (part.isQuestion && part.yesBranch.Length > 0 && part.noBranch.Length > 0)
        {
            var yesBranch = part.yesBranch;
            var noBranch = part.noBranch;
            if (answerType == AnswerType.Yes)
            {
                StartCoroutine(TypeSentence(yesBranch[_yesOrNoBranchIndex].sentence, yesBranch[_yesOrNoBranchIndex].talkerName, yesBranch[_yesOrNoBranchIndex].isPlayer));
            }
            else
            {
                StartCoroutine(TypeSentence(noBranch[_yesOrNoBranchIndex].sentence, noBranch[_yesOrNoBranchIndex].talkerName, noBranch[_yesOrNoBranchIndex].isPlayer));
            }
        }
        else
        {
            StartCoroutine(TypeSentence(part.sentence, part.talkerName, part.isPlayer));
        }
    }

    private void EndDialogue()
    {
        StopAllCoroutines();
        PlayerManager.Instance.EnableInputs();
        Debug.Log("End of conversation.");
        animator.SetBool("IsOpen", false);
        GameTimer.Instance.IsPaused = false;
    }
} */