using UnityEngine;
using System.Collections;

public class ZombieStateBase : StateMachineBehaviour, IZombieState {

	public AudioClip stateSound;
	public ZombieState stateType;
	protected ZombieBehaviour zombieBehaviour;

	public void Init (ZombieBehaviour behaviour) {
		zombieBehaviour = behaviour;
	}

	public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		
	}
}
