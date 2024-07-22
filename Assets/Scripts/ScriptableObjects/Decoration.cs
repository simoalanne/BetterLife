using UnityEngine;

[CreateAssetMenu(fileName = "NewDecoration", menuName = "ScriptableObjects/Decoration", order = 1)]
public class Decoration : InventoryItem
{
    [Header("Decoration settings")]
    public Vector2 Scale = new(2,2); // How many units (tiles) the decoration takes up when placed in the world.
}
