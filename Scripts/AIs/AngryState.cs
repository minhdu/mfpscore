using UnityEngine;
using System.Collections;

public class AngryState : ZombieStateBase {
	
	public override void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (stateInfo.normalizedTime >= 1.0f) {
			zombieBehaviour.ChangeState (ZombieState.Run);
			zombieBehaviour.SetSpeed (ZombieState.Run);
			zombieBehaviour.MoveAgent ();
		}
	}
}
