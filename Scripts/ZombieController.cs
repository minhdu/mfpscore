using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ZombieAnim {
	AWAKE = 0,
	IDLE,
	WALK,
	RUN,
	ATTACK,
	HEAVY_ATTACK,
	HURT,
	DEAD
}

public class ZombieController : MonoBehaviour {

	public Zombie zombie;
	Animation anim;
	NavMeshAgent navAgent;
	Collider collider;

	Waypoint[] wayPoints;
	int curWaypoint = 0;
	float nextAttackTime;
	bool isReachWaypoint = false;

	public AnimationClip[] awakeClips;
	public AnimationClip[] idleClips;
	public AnimationClip[] walkClips;
	public AnimationClip[] runClips;
	public AnimationClip[] attackClips;
	public AnimationClip[] heavyAttackClips;
	public AnimationClip[] hurtClips;
	public AnimationClip[] deadClips;

	Dictionary<ZombieAnim, AnimationClip[]> animClips = new Dictionary<ZombieAnim, AnimationClip[]> ();

	public void Init (Vector3 position, Vector3 rotation, params Waypoint[] waypoints) {
		anim = GetComponent<Animation> ();
		navAgent = GetComponent<NavMeshAgent> ();
		collider = GetComponent<Collider> ();
		wayPoints = waypoints;
		curWaypoint = -1;

		animClips.Add (ZombieAnim.AWAKE, awakeClips);
		animClips.Add (ZombieAnim.IDLE, idleClips);
		animClips.Add (ZombieAnim.WALK, idleClips);
		animClips.Add (ZombieAnim.RUN, idleClips);
		animClips.Add (ZombieAnim.ATTACK, idleClips);
		animClips.Add (ZombieAnim.HEAVY_ATTACK, idleClips);
		animClips.Add (ZombieAnim.HURT, idleClips);
		animClips.Add (ZombieAnim.DEAD, idleClips);

		StartCoroutine (Wakeup ());
	}

	void Update () {
		if (isReachWaypoint) {
			if (wayPoints [curWaypoint].isPlayer) {
				if (Time.time > nextAttackTime + zombie.attackRate) {
					StartCoroutine (Attack ());
					nextAttackTime = Time.time;
				}
			} else {
				StartCoroutine (Rest ());
			}
		}
	}

	public void SetNextTarget () {
		curWaypoint++;
		navAgent.SetDestination (wayPoints[curWaypoint].Position);
		StartCoroutine (Move ());
	}

	public void Hurt (float damage) {
		zombie.hitPoint -= damage;
		if (zombie.hitPoint <= 0) {
			collider.enabled = false;
			StartCoroutine (Dead ());
		} else {
			PlayAnimation (ZombieAnim.HURT);
		}
	}

	float PlayAnimation (ZombieAnim clip, WrapMode wrapMode = WrapMode.Default, bool crossFade=false, float fadeLenght=0.25f) {
		AnimationClip[] clips = animClips [clip];
		AnimationClip animClip = clips [Random.Range (0, clips.Length)];
		if (wrapMode == WrapMode.Loop && anim.IsPlaying (animClip.name))
			return 0;
		if(anim.GetClip (animClip.name) == null)
			anim.AddClip(animClip, animClip.name);
		anim.wrapMode = wrapMode;
		if (animClip != null) {
			if (crossFade) {
				anim.CrossFade (animClip.name, fadeLenght);
			} else {
				anim.clip = animClip;
				anim.Play ();
			}
			return animClip.length;
		}

		return 0;
	}

	IEnumerator Wakeup () {
		yield return new WaitForSeconds (PlayAnimation (ZombieAnim.AWAKE));
		SetNextTarget ();
	}

	IEnumerator Move () {
		var moveTypes = new[] {
			ProportionValue.Create (zombie.walkRate, ZombieAnim.WALK),
			ProportionValue.Create (1 - zombie.walkRate, ZombieAnim.RUN)
		};

		ZombieAnim move = moveTypes.ChooseByRandom ();
		PlayAnimation (move);
		if (move == ZombieAnim.WALK)
			navAgent.speed = zombie.walkSpeed;
		else
			navAgent.speed = zombie.runSpeed;

		var stopMove = new[] {
			ProportionValue.Create (zombie.randomChangeMoveStateRate, true),
			ProportionValue.Create (1 - zombie.randomChangeMoveStateRate, false)
		};
		bool isRndChangeMoveState = stopMove.ChooseByRandom();
		if (isRndChangeMoveState) {
			yield return new WaitForSeconds (Random.Range(2f, 5f));
			StartCoroutine (Move ());
		}
	}

	IEnumerator Dead () {
		yield return new WaitForSeconds(PlayAnimation (ZombieAnim.DEAD, WrapMode.Default, true, 0.25f));
		Destroy (gameObject);
	}

	IEnumerator Attack () {
		var attackTypes = new[] {
			ProportionValue.Create (zombie.heaveAttackRate, ZombieAnim.HEAVY_ATTACK),
			ProportionValue.Create (1 - zombie.heaveAttackRate, ZombieAnim.ATTACK)
		};
		ZombieAnim attack = attackTypes.ChooseByRandom ();
		float attackTime = PlayAnimation (attack);
		yield return new WaitForSeconds (attackTime * 0.75f);
		float damage = attack == ZombieAnim.ATTACK ? zombie.normalDamage : zombie.heavyDamage;
		PlayerController.Instance.Hurt (damage);
	}

	IEnumerator Rest () {
		PlayAnimation (ZombieAnim.IDLE, WrapMode.Loop, true, 0.1f);
		yield return new WaitForSeconds (zombie.restTime);
		SetNextTarget ();
	}

	void Start () {
		Waypoint wp = FindObjectOfType<Waypoint> ();
		Init (wp.Position, wp.transform.rotation.eulerAngles, wp);
	}
}
