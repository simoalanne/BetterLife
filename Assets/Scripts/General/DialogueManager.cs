using System.Collections;
using TMPro;
using UnityEngine;
using Player;
using UnityEngine.UI;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
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
    private bool _clickedYesDuringDialogue;
    private bool _clickedNoDuringDialogue;
    private DialogueTrigger.Dialogue[] _dialogue;
    private int _mainBranchIndex = 0; // Index for the main branch
    private int _questionBranchIndex = 0; // Index for the question branch
    private bool _inYesBranch, _inNoBranch;
    public Action OnYesClicked;
    public Action OnNoClicked;
    public Action OnDialogueEnd;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _continueButton.gameObject.SetActive(false);
            _yesButton.gameObject.SetActive(false);
            _noButton.gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
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

    IEnumerator TypeSentence(string sentence, string talkerName, bool isPlayer, bool isQuestion, bool finishesDialogue)
    {
        this.talkerName.text = talkerName;
        _talkerImage.sprite = isPlayer ? playerSprite : talkerSprite;
        dialogueText.text = "";

        if (finishesDialogue)
        {
            EnableButtons(isQuestion, true);
        }
        else
        {
            EnableButtons(isQuestion, false);
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
                _yesButton.onClick.AddListener(YesClicked);
                _noButton.onClick.AddListener(NoClicked);
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

    void YesClicked()
    {
        _clickedYesDuringDialogue = true;
        _inYesBranch = true;
        var part = _dialogue[_mainBranchIndex];
        if (part.yesBranch.Length == 0)
        {
            EndDialogue();
            return;
        }
        HandlePart();
    }

    void NoClicked()
    {
        _clickedNoDuringDialogue = true;
        _inNoBranch = true;
        var part = _dialogue[_mainBranchIndex];
        if (part.noBranch.Length == 0)
        {
            EndDialogue();
            return;
        }
        HandlePart();
    }

    void HandlePart()
    {
        var part = _dialogue[_mainBranchIndex];

        if (_inYesBranch && _questionBranchIndex < part.yesBranch.Length)
        {
            var y = _questionBranchIndex;
            StopAllCoroutines();
            StartCoroutine(TypeSentence(part.yesBranch[y].sentence, part.yesBranch[y].talkerName,
            part.yesBranch[y].isPlayer, false, part.yesBranch[y].finishesDialogue));
            _questionBranchIndex++;
        }

        else if (_inNoBranch && _questionBranchIndex < part.noBranch.Length)
        {
            var n = _questionBranchIndex;
            StopAllCoroutines();
            StartCoroutine(TypeSentence(part.noBranch[n].sentence, part.noBranch[n].talkerName,
            part.noBranch[n].isPlayer, false, part.noBranch[n].finishesDialogue));
            _questionBranchIndex++;
        }
        else
        {
            _questionBranchIndex = 0;
            _inYesBranch = false;
            _inNoBranch = false;
            StopAllCoroutines();
            StartCoroutine(TypeSentence(part.sentence, part.talkerName, part.isPlayer, part.isQuestion, part.finishesDialogue));
            if (_mainBranchIndex < _dialogue.Length - 1)
            {
                _mainBranchIndex++;
            }
        }
    }

    private void EndDialogue()
    {
        _mainBranchIndex = 0;
        StopAllCoroutines();
        PlayerManager.Instance.EnableInputs();
        animator.SetBool("IsOpen", false);
        GameTimer.Instance.IsPaused = false;
        if (_clickedYesDuringDialogue)
        {
            OnYesClicked?.Invoke();
            _clickedYesDuringDialogue = false;
        }
        else if (_clickedNoDuringDialogue)
        {
            OnNoClicked?.Invoke();
            _clickedNoDuringDialogue = false;
        }
        Debug.Log("Dialogue ended");
        OnDialogueEnd?.Invoke();
    }
}