using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class UIONOFF : MonoBehaviour
{
    [System.Serializable]
    public class ToggleUIElement
    {
        public string name;  // UI 오브젝트 이름
        public InputActionReference toggleAction;
        [NonSerialized] public GameObject uiObject;
        [NonSerialized] public Action<InputAction.CallbackContext> callback;
    }

    [Header("Toggle UI Elements")]
    [SerializeField] private ToggleUIElement[] toggleUIs;

    private void Awake()
    {
        TryReconnectAllUIs(); // 초기 연결 시도 (비활성 포함)
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        foreach (var toggle in toggleUIs)
        {
            if (toggle.toggleAction != null)
            {
                toggle.callback = ctx => ToggleUI(toggle.uiObject);
                toggle.toggleAction.action.performed += toggle.callback;
                toggle.toggleAction.action.Enable();
            }
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        foreach (var toggle in toggleUIs)
        {
            if (toggle.toggleAction != null && toggle.callback != null)
            {
                toggle.toggleAction.action.performed -= toggle.callback;
                toggle.toggleAction.action.Disable();
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[UIONOFF] 씬 로드됨 → UI 자동 재연결 시도");
        TryReconnectAllUIs();
    }

    private void TryReconnectAllUIs()
    {
        foreach (var toggle in toggleUIs)
        {
            if (toggle.uiObject == null && !string.IsNullOrEmpty(toggle.name))
            {
                var found = FindInActiveObjectByName(toggle.name);
                if (found != null)
                {
                    toggle.uiObject = found;
                    Debug.Log($"[UIONOFF] '{toggle.name}' → UI 자동 연결 완료 (비활성 포함)");
                }
                else
                {
                    Debug.LogWarning($"[UIONOFF] UI 오브젝트 '{toggle.name}' 를 찾지 못했습니다.");
                }
            }
        }
    }

    // 비활성화 오브젝트 포함 탐색
    private GameObject FindInActiveObjectByName(string name)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.hideFlags != 0 || !obj.scene.IsValid())
                continue;

            if (obj.name == name)
                return obj;
        }
        return null;
    }

    private void ToggleUI(GameObject ui)
    {
        if (ui != null)
        {
            bool isNowActive = !ui.activeSelf;
            ui.SetActive(isNowActive);

            if (isNowActive)
                ui.transform.SetAsLastSibling();
        }
        else
        {
            Debug.LogWarning("[UIONOFF] Toggle 시도했지만 uiObject가 null입니다.");
        }
    }
}