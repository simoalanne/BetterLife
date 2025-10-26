using System.Linq;
using UnityEngine;

/// <summary> Add all instances as children to game object with this script then add this game object to each scene. </summary>
[DefaultExecutionOrder(-1000)]
public class InstanceHolder : MonoBehaviour
{
    private void OnValidate() => transform.localScale = Vector3.zero;

    private void Awake()
    {
        transform.Cast<Transform>().ToList().ForEach(t => t.SetParent(null, worldPositionStays: false));
        Destroy(gameObject);
    }
}
