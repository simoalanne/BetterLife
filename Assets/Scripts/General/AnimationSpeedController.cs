using UnityEngine;

[RequireComponent(typeof(Animator))]

public class AnimationSpeedController : MonoBehaviour
{   
    public void SetAnimationSpeed(float newSpeed)
    {
        GetComponent<Animator>().speed = newSpeed;
    }
}
