using UnityEngine;

public class GameEndings : MonoBehaviour
{ 
    private static GameEndings Instance { get; set; }


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }
    /*
    void SubscribeToEvents()
    {
        Needs.Instance.OnHungerDepleted += DeathByHunger;
    }
    void OnDestroy()
    {
        UnsubscribeFromEvents();
    } */
}
