using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FakeEnemy : MonoBehaviour {
	public float health = 1000;
	List<LTDescr> tweens = new List<LTDescr>();

	public void Hurt (float damage, LTDescr tween = null) {
		if (tween != null && !tweens.Contains (tween))
			tweens.Add (tween);
		health -= damage;
		if (health <= 0 && tween != null) {

			for (int i = 0; i < tweens.Count; i++) {
				LeanTween.resume (tweens[i].uniqueId);
			}

			Destroy(gameObject);
		}
	}
}
