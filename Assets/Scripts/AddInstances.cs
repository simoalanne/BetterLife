using System.Collections.Generic;
using UnityEngine;

/// <summary> Can be used to ensure all instance scripts are present in the scene. </summary>
public class AddInstances : MonoBehaviour
{
    [SerializeField] private List<GameObject> instancePrefabs;

    private void Awake()
    {
        instancePrefabs.ForEach(prefab => Instantiate(prefab));
        Destroy(gameObject);
    }
}
