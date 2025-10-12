using UnityEngine;

namespace Player
{
    [DefaultExecutionOrder(-1)]
    public class InputManager : MonoBehaviour
    {
        public PlayerControls Controls { get; private set; }

        private void Awake()
        {
            if (!Services.Register(this, dontDestroyOnLoad: true)) return;
            Controls = new PlayerControls();
            Controls.Enable();
        }

        private void OnDisable()
        {
            Controls?.Disable();
        }
        
        public void BlockInput()
        {
            Controls.Player.Disable();
        }
        
        public void UnblockInput()
        {
            Controls.Player.Enable();
        }
    }
}
