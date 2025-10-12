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
    public bool CanInteract { get; set; } = true;
    private Camera _mainCamera;
    private InputAction _clickAction;

    private void Awake()
    {
        _interactableLayer = LayerMask.GetMask("Interactable");
        _clickAction = Services.InputManager.Controls.Player.Click;
    }

    private void Update()
    {
        if (_mainCamera == null || _clickAction is null || !CanInteract)
        {
            _mainCamera = Camera.main;
            SetCursor();
            return;
        }

        var mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var col = Physics2D.OverlapPoint(mousePos, _interactableLayer);
        var cols = col?.GetComponents<Collider2D>() ?? Array.Empty<Collider2D>();
        var target = cols.Length is 1 ? cols.First() : cols.FirstOrDefault(c => c.isTrigger && c == col);

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

        var interactables = target.GetComponents<IInteractable>().ToList();
        interactables.ForEach(i =>
        {
            if (i.CanInteract)
            {
                i.Interact();
            }
        });
    }


    private static void SetCursor(Texture2D cursorTexture = null)
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }

    private bool IsPlayerWithinInteractionRange(Collider2D target)
    {
        var range = target.GetComponent<OverrideInteractionRange>()?.InteractionRange ?? interactionRange;
        return Physics2D.OverlapCircleAll(transform.position, range, _interactableLayer).Contains(target);
    }
}
