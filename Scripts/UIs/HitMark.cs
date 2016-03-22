using UnityEngine;
using System.Collections;

public class HitMark : Singleton<HitMark> {

	RectTransform rectTransform;

	void Awake () {
		rectTransform = GetComponent<RectTransform> ();
	}

	public void Hit () {
		LeanTween.cancel (gameObject);
		LeanTween.alpha (rectTransform, 1, 0);
		LeanTween.alpha (rectTransform, 0, 0.5f);
	}
}
