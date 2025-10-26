using NaughtyAttributes;
using UnityEngine;

/// <summary> Add gameobjects containing this script to open world scenes to specify spawn points for the player </summary>
public class SpawnPoint : MonoBehaviour
{
    private enum PlayerFacing
    {
        FrontLeft,
        FrontRight,
        BackLeft,
        BackRight,
    }
    
    // Only show this field if there is more than one SpawnPoint in the scene
    [field: SerializeField, Scene, ShowIf(nameof(MoreThanOneInstance))]
    public string UseWhenPreviousSceneIs { get; private set; }
    
    private bool MoreThanOneInstance => FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None).Length > 1;

    [SerializeField, Tooltip("Direction the player will face when spawned")]
    private PlayerFacing facingDirection;
    
    public Vector2 FacingDirection => facingDirection switch
    {
        PlayerFacing.FrontLeft => new Vector2(-1, -1).normalized,
        PlayerFacing.FrontRight => new Vector2(1, -1).normalized,
        PlayerFacing.BackLeft => new Vector2(-1, 1).normalized,
        PlayerFacing.BackRight => new Vector2(1, 1).normalized,
        _ => Vector2.zero
    };
}
