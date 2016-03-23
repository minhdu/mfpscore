using UnityEngine;
using System.Collections;

public class ZombieStateBase : StateMachineBehaviour, IZombieState {

	public AudioClip stateSound;
	protected ZombieBehaviour zombieBehaviour;

	public void Init (ZombieBehaviour behaviour) {
		zombieBehaviour = behaviour;
	}

	public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		
	}
}
