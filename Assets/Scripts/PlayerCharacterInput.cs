using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Youregone.PlayerControls
{
    public class PlayerCharacterInput : MonoBehaviour
    {
        public event Action OnJumpButtonPressed;
        public event Action OnRamButtonPressed;
        public event Action OnRamButtonReleased;

        private CharacterMainInputActions _inputActions;


        private void OnEnable()
        {
            _inputActions = new();
            _inputActions.Enable();

            _inputActions.CharacterInputActions.Jump.performed += Jump_performed;
            _inputActions.CharacterInputActions.Ram.performed += Ram_performed;
            _inputActions.CharacterInputActions.Ram.canceled += Ram_Canceled;
        }

        private void OnDisable()
        {
            _inputActions.CharacterInputActions.Jump.performed -= Jump_performed;
            _inputActions.CharacterInputActions.Ram.performed -= Ram_performed;
            _inputActions.CharacterInputActions.Ram.canceled -= Ram_Canceled;
        }

        private void Ram_performed(InputAction.CallbackContext context)
        {
            OnRamButtonPressed?.Invoke();
        }

        private void Jump_performed(InputAction.CallbackContext context)
        {
            OnJumpButtonPressed?.Invoke();
        }

        private void Ram_Canceled(InputAction.CallbackContext context)
        {
            OnRamButtonReleased?.Invoke();
        }
    }
}