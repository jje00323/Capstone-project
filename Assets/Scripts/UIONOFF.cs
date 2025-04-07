using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

public class UIONOFF : MonoBehaviour
{
    [System.Serializable]
    public class ToggleUIElement
    {
        public string name;
        public InputActionReference toggleAction;
        public GameObject uiObject;

        [NonSerialized] public Action<InputAction.CallbackContext> callback; // 콜백 저장
    }

    [Header("Toggle UI Elements")]
    [SerializeField] private ToggleUIElement[] toggleUIs;

    private void OnEnable()
    {
        foreach (var toggle in toggleUIs)
        {
            if (toggle.toggleAction != null && toggle.uiObject != null)
            {
                toggle.callback = ctx => ToggleUI(toggle.uiObject); // 개별 callback 저장
                toggle.toggleAction.action.performed += toggle.callback;
                toggle.toggleAction.action.Enable();
            }
        }
    }

    private void OnDisable()
    {
        foreach (var toggle in toggleUIs)
        {
            if (toggle.toggleAction != null && toggle.callback != null)
            {
                toggle.toggleAction.action.performed -= toggle.callback;
                toggle.toggleAction.action.Disable();
            }
        }
    }

    private void ToggleUI(GameObject ui)
    {
        if (ui != null)
        {
            ui.SetActive(!ui.activeSelf);
        }
    }
}