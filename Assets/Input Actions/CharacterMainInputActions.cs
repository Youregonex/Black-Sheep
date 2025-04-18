//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.11.2
//     from Assets/Input Actions/CharacterMainInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @CharacterMainInputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @CharacterMainInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""CharacterMainInputActions"",
    ""maps"": [
        {
            ""name"": ""CharacterInputActions"",
            ""id"": ""452ebfbb-9852-4a27-aeb3-82045392ade6"",
            ""actions"": [
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""40f34934-3307-4191-a16d-33381293f4cd"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Ram"",
                    ""type"": ""Button"",
                    ""id"": ""5a568f75-31a1-4f5c-9eb1-d8c4949045a9"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""1bff7aa9-df8e-45b6-836d-49f9f56c128e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ScreenTap"",
                    ""type"": ""Button"",
                    ""id"": ""2f546475-ce4a-49e8-b0d2-9faa7ed1d418"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f28b75f2-3538-4c31-95e5-262a4b5f03bd"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7fff68ff-3bde-489a-82cc-bd3efcc0be60"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ram"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cf0dc81e-526f-4ff5-9cc4-5fe13ccf548c"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fa7d4d24-0ae4-462e-8979-da44fbabccbd"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ScreenTap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""07e657c0-efcd-455e-b929-b750b9e81779"",
                    ""path"": ""<Touchscreen>/primaryTouch/tap"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ScreenTap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // CharacterInputActions
        m_CharacterInputActions = asset.FindActionMap("CharacterInputActions", throwIfNotFound: true);
        m_CharacterInputActions_Jump = m_CharacterInputActions.FindAction("Jump", throwIfNotFound: true);
        m_CharacterInputActions_Ram = m_CharacterInputActions.FindAction("Ram", throwIfNotFound: true);
        m_CharacterInputActions_Pause = m_CharacterInputActions.FindAction("Pause", throwIfNotFound: true);
        m_CharacterInputActions_ScreenTap = m_CharacterInputActions.FindAction("ScreenTap", throwIfNotFound: true);
    }

    ~@CharacterMainInputActions()
    {
        UnityEngine.Debug.Assert(!m_CharacterInputActions.enabled, "This will cause a leak and performance issues, CharacterMainInputActions.CharacterInputActions.Disable() has not been called.");
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // CharacterInputActions
    private readonly InputActionMap m_CharacterInputActions;
    private List<ICharacterInputActionsActions> m_CharacterInputActionsActionsCallbackInterfaces = new List<ICharacterInputActionsActions>();
    private readonly InputAction m_CharacterInputActions_Jump;
    private readonly InputAction m_CharacterInputActions_Ram;
    private readonly InputAction m_CharacterInputActions_Pause;
    private readonly InputAction m_CharacterInputActions_ScreenTap;
    public struct CharacterInputActionsActions
    {
        private @CharacterMainInputActions m_Wrapper;
        public CharacterInputActionsActions(@CharacterMainInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Jump => m_Wrapper.m_CharacterInputActions_Jump;
        public InputAction @Ram => m_Wrapper.m_CharacterInputActions_Ram;
        public InputAction @Pause => m_Wrapper.m_CharacterInputActions_Pause;
        public InputAction @ScreenTap => m_Wrapper.m_CharacterInputActions_ScreenTap;
        public InputActionMap Get() { return m_Wrapper.m_CharacterInputActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CharacterInputActionsActions set) { return set.Get(); }
        public void AddCallbacks(ICharacterInputActionsActions instance)
        {
            if (instance == null || m_Wrapper.m_CharacterInputActionsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_CharacterInputActionsActionsCallbackInterfaces.Add(instance);
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @Ram.started += instance.OnRam;
            @Ram.performed += instance.OnRam;
            @Ram.canceled += instance.OnRam;
            @Pause.started += instance.OnPause;
            @Pause.performed += instance.OnPause;
            @Pause.canceled += instance.OnPause;
            @ScreenTap.started += instance.OnScreenTap;
            @ScreenTap.performed += instance.OnScreenTap;
            @ScreenTap.canceled += instance.OnScreenTap;
        }

        private void UnregisterCallbacks(ICharacterInputActionsActions instance)
        {
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @Ram.started -= instance.OnRam;
            @Ram.performed -= instance.OnRam;
            @Ram.canceled -= instance.OnRam;
            @Pause.started -= instance.OnPause;
            @Pause.performed -= instance.OnPause;
            @Pause.canceled -= instance.OnPause;
            @ScreenTap.started -= instance.OnScreenTap;
            @ScreenTap.performed -= instance.OnScreenTap;
            @ScreenTap.canceled -= instance.OnScreenTap;
        }

        public void RemoveCallbacks(ICharacterInputActionsActions instance)
        {
            if (m_Wrapper.m_CharacterInputActionsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ICharacterInputActionsActions instance)
        {
            foreach (var item in m_Wrapper.m_CharacterInputActionsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_CharacterInputActionsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public CharacterInputActionsActions @CharacterInputActions => new CharacterInputActionsActions(this);
    public interface ICharacterInputActionsActions
    {
        void OnJump(InputAction.CallbackContext context);
        void OnRam(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnScreenTap(InputAction.CallbackContext context);
    }
}
