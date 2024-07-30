using UnityEngine;

public class CardScript : MonoBehaviour
{
    [SerializeField] private int value = 0;
    private Sprite sprite;

    private Sprite back;

    public int Value { get => value; set => this.value = value; }

    void Awake()
    {
        back = GetComponent<SpriteRenderer>().sprite;
    }

    public void ResetCard()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = back;
        value = 0;
    }

    public void SetSprite(Sprite sprite) => gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
}
