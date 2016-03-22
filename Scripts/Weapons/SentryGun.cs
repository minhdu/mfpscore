using UnityEngine;
using System.Collections;

public class SentryGun : MonoBehaviour {

	public LayerMask collisionLayers = -1;
	public GameObject[] guns;
	public Transform[] gunMuzzles;
	public float[] maxAngle;
	public float shootRange;
	public float fireRate;
	public int bulletPerShoot = 6;
	public float damage = 10;
	public GameObject impactEffect;
	public Vector2 spreadRange;
	public Lookable[] projectiles;

	LTDescr[] leanTweens;
	float[] nextFire;
	GameObject[] targets;

	void Start () {
		Active (transform.position, transform.rotation);
	}

	public void Active (Vector3 position, Quaternion rotation) {
		nextFire = new float[guns.Length];
		targets = new GameObject[guns.Length];
		leanTweens = new LTDescr[guns.Length];

		leanTweens [0] = LeanTween.rotateLocal (guns [0], new Vector3(0, maxAngle[0], 0), 3).setEase (LeanTweenType.linear).setLoopPingPong ();
		
		if(leanTweens.Length > 1)
			leanTweens [1] = LeanTween.rotateLocal (guns [1], new Vector3(0, maxAngle[1], 0), 3).setEase (LeanTweenType.linear).setLoopPingPong ();
	}

	void FixedUpdate () {
		for (int i = 0; i < guns.Length; i++) {
			Ray gunRay = new Ray (gunMuzzles[i].position, gunMuzzles[i].forward);
			RaycastHit gunHit = new RaycastHit();
			if (Physics.Raycast (gunRay, out gunHit, shootRange, collisionLayers.value)) {
				if (targets [i] == null) {
					LeanTween.pause (leanTweens [i].uniqueId);
					targets [i] = gunHit.collider.gameObject;
				}
				if (Time.time > nextFire [i] + fireRate) {
					for (int j = 0; j < bulletPerShoot; j++) {
						Vector3 target = new Vector3 (gunHit.point.x + Random.Range (-fireRate, fireRate), gunHit.point.y + Random.Range (-fireRate, fireRate), gunHit.point.z);
						projectiles [(i * bulletPerShoot) + j].Look (target);
						Ray bulletRay = new Ray (gunMuzzles [i].position, target - gunMuzzles [i].position);
						RaycastHit bulletHit = new RaycastHit ();
						if (Physics.Raycast (bulletRay, out bulletHit, shootRange, collisionLayers.value)) {
							Quaternion hitRotation = Quaternion.FromToRotation (Vector3.forward, bulletHit.normal);
							Instantiate (impactEffect, bulletHit.point, hitRotation); //Impact
							FakeEnemy fakeEnemy = bulletHit.collider.GetComponent<FakeEnemy> ();
							if (fakeEnemy != null)
								fakeEnemy.Hurt (damage, leanTweens [i]);
						}
					}
					nextFire [i] = Time.time;
				}
			}
		}
	}
}
