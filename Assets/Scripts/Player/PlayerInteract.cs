using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player interaction with interactable objects. Supports displaying custom cursors based on object tags.
/// </summary>
public class PlayerInteract : MonoBehaviour
{
    [Serializable]
    public struct CursorType
    {
        public Texture2D texture;
        private Texture2D _halfTransparent;

        public Texture2D GetCursorTexture(bool asHalfTransparent)
        {
            if (texture is null) return null;
            _halfTransparent ??= CreateHalfTransparentTexture(texture);
            return asHalfTransparent ? _halfTransparent : texture;
        }

        private static Texture2D CreateHalfTransparentTexture(Texture2D original)
        {
            var copy = new Texture2D(original.width, original.height, TextureFormat.RGBA32, mipChain: false);
            var adjusted = original.GetPixels().Select(c => new Color(c.r, c.g, c.b, c.a * 0.5f)).ToArray();
            copy.SetPixels(adjusted);
            copy.Apply();
            return copy;
        }
    }

    [Header("Interaction settings")]
    [SerializeField, Tooltip("Base interact range. Interactable can override this value")]
    private float interactionRange = 2.5f;

    [Header("Cursor types")]
    [SerializedDictionary("Tag", "Cursor Type")]
    public SerializedDictionary<string, CursorType> customCursors = new();
    [SerializeField] private CursorType generalInteractCursor;
    private int _interactableLayer;
    private Camera _mainCamera;
    private InputAction _clickAction;

    private bool _inputActive = true;

    private void Awake()
    {
        _interactableLayer = LayerMask.GetMask("Interactable");
        var inputManager = Services.InputManager;
        _clickAction = inputManager.Controls.Player.Click;
        inputManager.OnInputActiveChange += SetInputActive;
    }

    private void SetInputActive(bool active) => _inputActive = active;
    private void OnDestroy() => Services.InputManager.OnInputActiveChange -= SetInputActive;

    private void Update()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
            return;
        }

        if (!_inputActive)
        {
            SetCursor();
            return;
        }

        var mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // Target is valid if the gameobject has only one collider or if the mouse is hovering a trigger collider
        var hoveredColliders = Physics2D.OverlapPointAll(mousePos, _interactableLayer);
        var allGameObjectColliders = hoveredColliders.FirstOrDefault()?.GetComponents<Collider2D>();
        var target = allGameObjectColliders?.Length is 1
            ? hoveredColliders.First()
            : allGameObjectColliders?.FirstOrDefault(c => c.isTrigger && hoveredColliders.Contains(c));

        if (target is null)
        {
            SetCursor();
            return;
        }

        var inRange = IsPlayerWithinInteractionRange(target);
        var customCursor = customCursors.GetValueOrDefault(target.tag);
        var texture = customCursor.GetCursorTexture(!inRange) ?? generalInteractCursor.GetCursorTexture(!inRange);
        SetCursor(texture);

        if (!_clickAction.WasPressedThisFrame() || !inRange) return;

        var interactables = target.GetComponents<IInteractable>();
        interactables.ToList().ForEach(interactable => interactable.Interact());
    }


    private static void SetCursor(Texture2D cursorTexture = null) =>
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);

    private bool IsPlayerWithinInteractionRange(Collider2D target) =>
        Physics2D.OverlapCircleAll(transform.position, interactionRange, _interactableLayer).Contains(target);
}
