using UnityEngine;
using System.Collections;

public class BulletHole : MonoBehaviour {

	public float delayTime = 0;
	public float fadeTime = 0;
	public Renderer renderer;

	IEnumerator Start () {
		yield return new WaitForSeconds (delayTime);
		LeanTween.value (gameObject, 1f, 0f, fadeTime).setOnUpdate (UpdateAlpha).setOnComplete (OnFadeComplete);;
	}

	void UpdateAlpha (float alpha) {
		Color color = renderer.material.color;
		renderer.sharedMaterial.color = new Color (color.r, color.g, color.b, alpha);
	}

	void OnFadeComplete () {
		PoolManager.Instance.Release (gameObject, PoolTag.BULLET_HOLE);
		Destroy (gameObject);
	}
}
