using UnityEngine;
using System.Collections;
using System;

public class CameraShake : Singleton<CameraShake> {
	
	float shakeAmt = 2f; // the degrees to shake the camera
	float shakePeriodTime = 0.42f; // The period of each shake
	float dropOffTime = 1.6f; // How long it takes the shaking to settle down to nothing
	LTDescr shootTween;
	LTDescr idleTween;

	public void GrenadeShake (Action onShakeComplete=null) {
		LTDescr shakeTween = LeanTween.rotateAround(gameObject, Vector3.right, shakeAmt, shakePeriodTime)
			.setEase(LeanTweenType.easeShake) // this is a special ease that is good for shaking
			.setLoopClamp()
			.setRepeat(-1);

		// Slow the camera shake down to zero
		LeanTween.value(gameObject, shakeAmt, 0f, dropOffTime).setOnUpdate(
			(float val)=>{
				if(val > 0)
					shakeTween.setTo(Vector3.right*val);
				else {
					LeanTween.cancel(shakeTween.uniqueId);
					if(onShakeComplete != null)
						onShakeComplete();
				}
			}
		).setEase(LeanTweenType.easeOutQuad);
	}

	public void ShakeLoop (float amount, float preiodTime) {
//		LeanTween.rotateAround(gameObject, Vector3.right, amount, preiodTime)
//			.setEase(LeanTweenType.easeShake) // this is a special ease that is good for shaking
//			.setLoopPingPong()
//			.setRepeat(-1);
	}


	public void ShootingShake (float amount, float time) {
//		LeanTween.moveLocal(gameObject, Vector3.forward*amount, time)
//			.setEase(LeanTweenType.easeShake).setLoopPingPong(-1);
	}

	public void StopAllShake () {
//		LeanTween.cancel (gameObject);
//		mTransform.localPosition = originalPosition;
	}
}