using UnityEngine.InputSystem;
using System;
using Youregone.UI;
using UnityEngine;

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
        private RamButton _ramButton;
        private JumpButton _jumpButton;

        public bool JumpPressed => _jumpPressed;
        public bool RamPressed => _ramPressed;

        public PlayerCharacterInput(JumpButton jumpButton, RamButton ramButton)
        {
            _inputActions = new();

            _jumpButton = jumpButton;
            _ramButton = ramButton;
            EnableInput();
        }

        public void EnableInput()
        {
            if(SystemInfo.deviceType == DeviceType.Handheld)
            {
                _ramButton.Button.interactable = true;
                _jumpButton.Button.interactable = true;
                _ramButton.OnRamButtonPressed += RamButton_OnRamButtonPressed;
                _ramButton.OnRamButtonReleased += RamButton_OnRamButtonReleased;
                _jumpButton.OnJumpButtonPressed += JumpButton_OnJumpButtonPressed;
                _jumpButton.OnJumpButtonReleased += JumpButton_OnJumpButtonReleased;
            }

            _inputActions.Enable();
            _inputActions.CharacterInputActions.Enable();

            if(SystemInfo.deviceType == DeviceType.Desktop)
            {
                _inputActions.CharacterInputActions.Jump.performed += Jump_performed;
                _inputActions.CharacterInputActions.Jump.canceled += Jump_canceled;

                _inputActions.CharacterInputActions.Ram.performed += Ram_performed;
                _inputActions.CharacterInputActions.Ram.canceled += Ram_Canceled;

                _inputActions.CharacterInputActions.Pause.performed += Pause_performed;
            }

            _inputActions.CharacterInputActions.ScreenTap.performed += ScreenTap_performed;
        }

        public void DisableInput()
        {
            if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                _ramButton.Button.interactable = false;
                _jumpButton.Button.interactable = false;
                _ramButton.OnRamButtonPressed -= RamButton_OnRamButtonPressed;
                _ramButton.OnRamButtonReleased -= RamButton_OnRamButtonReleased;
                _jumpButton.OnJumpButtonPressed -= JumpButton_OnJumpButtonPressed;
                _jumpButton.OnJumpButtonReleased -= JumpButton_OnJumpButtonReleased;
            }

            _inputActions.Disable();
            _inputActions.CharacterInputActions.Disable();

            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                _inputActions.CharacterInputActions.Jump.performed -= Jump_performed;
                _inputActions.CharacterInputActions.Jump.canceled -= Jump_canceled;

                _inputActions.CharacterInputActions.Ram.performed -= Ram_performed;
                _inputActions.CharacterInputActions.Ram.canceled -= Ram_Canceled;

                _inputActions.CharacterInputActions.Pause.performed -= Pause_performed;
            }

            _inputActions.CharacterInputActions.ScreenTap.performed -= ScreenTap_performed;
        }

        private void JumpButton_OnJumpButtonPressed()
        {
            _jumpPressed = true;
        }

        private void JumpButton_OnJumpButtonReleased()
        {
            _jumpPressed = false;
        }

        private void RamButton_OnRamButtonPressed()
        {
            _ramPressed = true;
        }

        private void RamButton_OnRamButtonReleased()
        {
            _ramPressed = false;
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