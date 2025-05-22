using UnityEngine;

[ExecuteAlways] // ← 이거 중요
public class EditorParticleFreezer : MonoBehaviour
{
    [Range(0f, 5f)]
    public float simulateTime = 1f;

    void OnEnable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            var systems = GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in systems)
            {
                ps.Simulate(simulateTime, true, true, true);
                ps.Pause();
            }
        }
#endif
    }
    void Awake()
    {
        var systems = GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in systems)
        {
            ps.Simulate(simulateTime, true, true, true);
            ps.Pause();
        }
    }
}