using System;
using UnityEngine;

namespace Player
{
    [DefaultExecutionOrder(-1)]
    public class InputManager : MonoBehaviour
    {
        public PlayerControls Controls { get; private set; }
        public Action<bool> OnInputActiveChange;
        public bool IsInputActive => Controls.Player.enabled;

        private void Awake()
        {
            if (!Services.Register(this, persistent: true)) return;
            Controls = new PlayerControls();
            Controls.Enable();
        }

        private void OnDisable()
        {
            Controls?.Disable();
        }
        
        public void EnablePlayerInput(bool enable)
        {
            OnInputActiveChange?.Invoke(enable);
            if (!enable)
            {
                Controls.Player.Disable();
                return;
            }
            Controls.Player.Enable();
        }
    }
}
