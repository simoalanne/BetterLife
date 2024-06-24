using UnityEngine;

[RequireComponent(typeof(Animator))]

public class AnimationSpeedController : MonoBehaviour
{   
    [Range(0.1f, 10f)]
    [SerializeField] private float _animSpeed = 1f;
    void Update()
    {
        GetComponent<Animator>().speed = _animSpeed;
    }
}
