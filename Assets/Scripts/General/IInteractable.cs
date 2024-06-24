using UnityEngine;

/// <summary>
/// Interface for interactable objects
/// Every object that can be interacted with should implement this interface for the player interaction to work
public interface IInteractable
{
    Vector2 InteractMinDistance { get; set; } // Player must be within this distance to interact with the object
    bool IsInteractable { get; set; } // Whether the object is in an interactable state
    void Interact(); // The interaction method that will be called by the player when interacting with the object. This script should contain the logic for the interaction
}
