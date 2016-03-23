using UnityEngine;
using System.Collections;

public class HeavyAttackState : ZombieStateBase {

	public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter (animator, stateInfo, layerIndex);
		zombieBehaviour.SetDamage (true);
	}

	public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		zombieBehaviour.ChangeState (ZombieState.Idle);
	}
}
