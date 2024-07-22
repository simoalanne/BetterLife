using UnityEngine;

/// <summary>
/// This script should be in a prefab and the prefab should be in all the scenes of the game.
/// </summary>
public class AddInstances : MonoBehaviour
{
    [SerializeField] private GameObject[] _instancePrefabs;
    
    void Awake()
    {
        for (int i = 0; i < _instancePrefabs.Length; i++)
        {
            if (GameObject.Find(_instancePrefabs[i].name) == null) 
            {
                Instantiate(_instancePrefabs[i]);
            }
        }
        Destroy(gameObject);
    }
}
