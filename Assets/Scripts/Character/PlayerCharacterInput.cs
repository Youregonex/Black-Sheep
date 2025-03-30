using UnityEngine.InputSystem;
using System;

namespace Youregone.YPlayerController
{
    public class PlayerCharacterInput
    {
        public event Action OnRamButtonReleased;
        public event Action OnPauseButtonPressed;
        public event Action OnScreenTap;

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
            _inputActions.CharacterInputActions.Enable();

            _inputActions.CharacterInputActions.Jump.performed += Jump_performed;
            _inputActions.CharacterInputActions.Jump.canceled += Jump_canceled;

            _inputActions.CharacterInputActions.Ram.performed += Ram_performed;
            _inputActions.CharacterInputActions.Ram.canceled += Ram_Canceled;

            _inputActions.CharacterInputActions.Pause.performed += Pause_performed;

            _inputActions.CharacterInputActions.ScreenTap.performed += ScreenTap_performed;
        }

        public void DisableInput()
        {
            _inputActions.Disable();
            _inputActions.CharacterInputActions.Disable();

            _inputActions.CharacterInputActions.Jump.performed -= Jump_performed;
            _inputActions.CharacterInputActions.Jump.canceled -= Jump_canceled;

            _inputActions.CharacterInputActions.Ram.performed -= Ram_performed;
            _inputActions.CharacterInputActions.Ram.canceled -= Ram_Canceled;

            _inputActions.CharacterInputActions.Pause.performed -= Pause_performed;

            _inputActions.CharacterInputActions.ScreenTap.performed -= ScreenTap_performed;
        }

        private void ScreenTap_performed(InputAction.CallbackContext context)
        {
            OnScreenTap?.Invoke();
        }

        private void Pause_performed(InputAction.CallbackContext context)
        {
            OnPauseButtonPressed?.Invoke();
        }

        private void Jump_performed(InputAction.CallbackContext context)
        {
            _jumpPressed = true;
        }

        private void Jump_canceled(InputAction.CallbackContext context)
        {
            _jumpPressed = false;
        }

        private void Ram_performed(InputAction.CallbackContext context)
        {
            _ramPressed = true;
        }

        private void Ram_Canceled(InputAction.CallbackContext context)
        {
            _ramPressed = false;
            OnRamButtonReleased?.Invoke();
        }
    }
}