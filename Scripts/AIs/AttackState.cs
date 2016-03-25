using UnityEngine;
using System.Collections;

public class AttackState : ZombieStateBase {

	public override void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (stateInfo.normalizedTime >= 1) {
			zombieBehaviour.ChangeState (ZombieState.Idle);
		}
	}
}
