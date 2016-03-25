﻿using UnityEngine;
using System.Collections;

public class WakeupState : ZombieStateBase {
	public override void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (stateInfo.normalizedTime >= 1.0f) {
			zombieBehaviour.SetMove ();
		}
	}
}
