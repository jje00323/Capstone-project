using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyCutsceneState : BossEnemyState
{
    public BossEnemyCutsceneState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        Debug.Log("[CutsceneState] 컷씬 시작!");

        // 컷씬 연출 시작 (예: 애니메이션, 카메라 전환, 대사 출력 등)
        boss.StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        // 예: 3초짜리 컷씬
        yield return new WaitForSeconds(3f);

        Debug.Log("[CutsceneState] 컷씬 종료 → Idle 상태 복귀");
        boss.ChangeState(boss.idleState); // 컷씬 이후 다시 Idle로 복귀
    }

    public override void Update() { }
    public override void Exit() { }
}