using UnityEngine;
using System.Collections;

public class CameraShake : Singleton<CameraShake> {
	
	float shakeAmt = 2f; // the degrees to shake the camera
	float shakePeriodTime = 0.42f; // The period of each shake
	float dropOffTime = 1.6f; // How long it takes the shaking to settle down to nothing
	LTDescr shootTween;
	LTDescr idleTween;
	Transform mTransform;
	Vector3 idleVector;
	bool isIdle = false;

	void Awake () {
		mTransform = GetComponent<Transform> ();
		idleVector = new Vector3 (mTransform.localPosition.x, mTransform.localPosition.y + 0.05f, mTransform.localPosition.z);
	}

	public void Idle () {
		if (!isIdle && idleTween != null) {
			LeanTween.resume (idleTween.uniqueId);
			return;
		}
		isIdle = true;
		idleTween = LeanTween.moveLocal(gameObject, idleVector, 1.5f).setEase(LeanTweenType.easeInSine).setLoopPingPong(-1);
	}

	public void StopIdle () {
		if (idleTween != null) {
			LeanTween.pause (idleTween.uniqueId);
			isIdle = false;
		}
	}

	public void GrenadeShake () {
		LTDescr shakeTween = LeanTween.rotateAround(gameObject, Vector3.right, shakeAmt, shakePeriodTime)
			.setEase(LeanTweenType.easeShake) // this is a special ease that is good for shaking
			.setLoopClamp()
			.setRepeat(-1);

		// Slow the camera shake down to zero
		LeanTween.value(gameObject, shakeAmt, 0f, dropOffTime).setOnUpdate(
			(float val)=>{
				if(val > 0)
					shakeTween.setTo(Vector3.right*val);
				else
					LeanTween.cancel(shakeTween.uniqueId);
			}
		).setEase(LeanTweenType.easeOutQuad);
	}

	public void ShakeLoop (float amount, float preiodTime) {
		LeanTween.rotateAround(gameObject, Vector3.right, amount, preiodTime)
			.setEase(LeanTweenType.easeShake) // this is a special ease that is good for shaking
			.setLoopPingPong()
			.setRepeat(-1);
	}

	public void ShootingShake (float amount, float time) {
		shootTween = LeanTween.rotateAroundLocal(gameObject, Vector3.right, amount, time)
			.setEase(LeanTweenType.easeShake).setLoopPingPong(-1);
	}

	public void StopShootingShake () {
		if(shootTween != null)
			LeanTween.cancel (shootTween.uniqueId);
	}
}