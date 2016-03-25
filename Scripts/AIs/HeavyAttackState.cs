﻿using UnityEngine;
using System.Collections;

public class HeavyAttackState : ZombieStateBase {
	public override void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (stateInfo.normalizedTime >= 1.0f) {
			zombieBehaviour.ChangeState (ZombieState.Idle);
		}
	}
}
