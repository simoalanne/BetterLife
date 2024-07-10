
/// <summary>
/// Interface for interactable objects
/// Every object that can be interacted with should implement this interface for the player interaction to work
public interface IInteractable
{
    void Interact(); // The interaction method that will be called by the player when interacting with the object. This script should contain the logic for the interaction
}
