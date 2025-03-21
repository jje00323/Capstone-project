using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestMainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("TestScene"); // 게임 씬으로 이동
    }

    public void OpenOptions()
    {
        Debug.Log("옵션 메뉴는 아직 구현되지 않았습니다.");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // 유니티 에디터에서 게임 종료
        #else
            Application.Quit(); // 빌드된 게임에서 종료
        #endif
    }

}
