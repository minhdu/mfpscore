using UnityEngine;
using System.Collections;

public class HurtState : ZombieStateBase {

	public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		zombieBehaviour.SetMove ();
	}
}
