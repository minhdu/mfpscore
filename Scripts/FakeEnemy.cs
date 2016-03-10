using UnityEngine;
using System.Collections;

public class FakeEnemy : MonoBehaviour {
	public float health = 1000;

	public void Hurt (float damage, LTDescr tween = null) {
		health -= damage;
		if (health <= 0 && tween != null) {
			LeanTween.resume (tween.uniqueId);
			//gameObject.SetActive (false);
			Destroy(gameObject);
		}
	}
}
