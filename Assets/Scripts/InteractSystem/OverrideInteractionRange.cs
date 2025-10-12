using UnityEngine;

/// <summary>
/// Can be used to override the global interaction range set in PlayerInteract.cs.
/// </summary>
public class OverrideInteractionRange : MonoBehaviour
{
   [SerializeField] private float _interactionRange;
   public float InteractionRange => _interactionRange;
}
