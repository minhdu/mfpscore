using UnityEngine;
using System.Collections;

public class ScreamState : ZombieStateBase {
	
	public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		zombieBehaviour.ChangeState (ZombieState.Run);
		zombieBehaviour.MoveAgent ();
	}
}
