using UnityEngine;
using UnityEditor;

public class FreezeParticleWindow : EditorWindow
{
    [MenuItem("Tools/Particle/Freeze Selected Particle")]
    public static void FreezeSelectedParticle()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("아무 오브젝트도 선택되지 않았습니다.");
            return;
        }

        var ps = Selection.activeGameObject.GetComponent<ParticleSystem>();
        if (ps == null)
        {
            Debug.LogWarning("선택한 오브젝트에 ParticleSystem이 없습니다.");
            return;
        }

        // 원하는 시간까지 시뮬레이션 (여기선 1초)
        ps.Simulate(0.8f, true, true, true);
        ps.Pause();

        Debug.Log($"[FreezeParticle] '{ps.gameObject.name}' 파티클 1.0초 상태로 고정됨.");
    }
}