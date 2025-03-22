using UnityEngine.InputSystem;
using System;
using Youregone.GameSystems;

namespace Youregone.PlayerControls
{
    public class PlayerCharacterInput
    {
        //public event Action OnJumpButtonPressed;
        //public event Action OnRamButtonPressed;
        public event Action OnRamButtonReleased;
        public event Action OnPauseButtonPressed;

        private CharacterMainInputActions _inputActions;

        private bool _jumpPressed;
        private bool _ramPressed;

        public bool JumpPressed => _jumpPressed;
        public bool RamPressed => _ramPressed;

        public PlayerCharacterInput()
        {
            _inputActions = new();
            EnableInput();
        }

        public void EnableInput()
        {
            _inputActions.Enable();
            _inputActions.CharacterInputActions.Jump.performed += Jump_performed;
            _inputActions.CharacterInputActions.Jump.canceled += Jump_canceled;
            _inputActions.CharacterInputActions.Ram.performed += Ram_performed;
            _inputActions.CharacterInputActions.Ram.canceled += Ram_Canceled;
            _inputActions.CharacterInputActions.Pause.performed += Pause_performed;
        }

        public void DisableInput()
        {
            _inputActions.Disable();
            _inputActions.CharacterInputActions.Jump.performed -= Jump_performed;
            _inputActions.CharacterInputActions.Jump.canceled -= Jump_canceled;
            _inputActions.CharacterInputActions.Ram.performed -= Ram_performed;
            _inputActions.CharacterInputActions.Ram.canceled -= Ram_Canceled;
            _inputActions.CharacterInputActions.Pause.performed -= Pause_performed;
        }

        private void Pause_performed(InputAction.CallbackContext context)
        {
            OnPauseButtonPressed?.Invoke();
        }

        private void Jump_performed(InputAction.CallbackContext context)
        {
            _jumpPressed = true;
            //OnJumpButtonPressed?.Invoke();
        }

        private void Jump_canceled(InputAction.CallbackContext context)
        {
            _jumpPressed = false;
            //OnJumpButtonPressed?.Invoke();
        }

        private void Ram_performed(InputAction.CallbackContext context)
        {
            _ramPressed = true;
            //OnRamButtonPressed?.Invoke();
        }

        private void Ram_Canceled(InputAction.CallbackContext context)
        {
            _ramPressed = false;
            OnRamButtonReleased?.Invoke();
        }
    }
}