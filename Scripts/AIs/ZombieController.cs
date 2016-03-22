using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieController : CoroutinableMono, IEventListener {

	public Zombie zombie;
	public Animation anim;
	NavMeshAgent navAgent;
	public Collider[] colliders;

	bool isInited = false;
	bool isReachWaypoint = false;
	Waypoint[] wayPoints;
	int curWaypoint = 0;
	float nextAttackTime;
	float nextRestTime;

	bool isStunning;

	public AnimationClip[] awakeClips;
	public AnimationClip[] idleClips;
	public AnimationClip[] walkClips;
	public AnimationClip[] runClips;
	public AnimationClip[] attackClips;
	public AnimationClip[] heavyAttackClips;
	public AnimationClip[] hurtClips;
	public AnimationClip[] deadClips;

	Dictionary<ZombieAnim, AnimationClip[]> animClips = new Dictionary<ZombieAnim, AnimationClip[]> ();

	void OnEnable () {
		Init (FindPlayer());
	}

	public Waypoint FindPlayer () {
		return FPSCamera.Instance.GetComponentInParent<Waypoint> ();
	}

	public void ListenEvents () {
		for(int i=0; i<colliders.Length; i++) {
			EventDispatcher.AddEventListener<float> (GameEvents.GameplayEvents.DAMAGE + colliders[i].GetInstanceID(), Hurt);
		}
	}

	public void Init (params Waypoint[] waypoints) {
		navAgent = GetComponent<NavMeshAgent> ();
		colliders = GetComponentsInChildren<Collider> ();
		wayPoints = waypoints;
		curWaypoint = -1;

		animClips.Add (ZombieAnim.AWAKE, awakeClips);
		animClips.Add (ZombieAnim.IDLE, idleClips);
		animClips.Add (ZombieAnim.WALK, walkClips);
		animClips.Add (ZombieAnim.RUN, runClips);
		animClips.Add (ZombieAnim.ATTACK, attackClips);
		animClips.Add (ZombieAnim.HEAVY_ATTACK, heavyAttackClips);
		animClips.Add (ZombieAnim.HURT, hurtClips);
		animClips.Add (ZombieAnim.DEAD, deadClips);

		StartCoroutine (Wakeup ());

		ListenEvents ();
	}

	void Update () {
		if (!isInited)
			return;

		if (!isReachWaypoint)
			isReachWaypoint = CheckIsReachWaypoint ();
		
		if (isReachWaypoint && zombie.hitPoint > 0) {
			if (wayPoints [curWaypoint].isPlayer) {
				if (Time.time > nextAttackTime + zombie.attackRate) {
					RunCoroutine ("Attack", Idle);
					nextAttackTime = Time.time;
				}
			} else {
				if (Time.time > nextRestTime + zombie.restTime) {
					StartCoroutine ("Rest");
					nextRestTime = Time.time;
				}
			}
		}
	}

	public void SetNextTarget () {
		curWaypoint++;
		SetMove ();
		StartCoroutine ("Move");
		isReachWaypoint = false;
	}

	public void SetMove () {
		if (zombie.hitPoint <= 0)
			return;
		navAgent.SetDestination (wayPoints[curWaypoint].Position);
		navAgent.Resume ();
		StartCoroutine ("Move");
	}

	public void Hurt (float damage) {
		isStunning = true;
		navAgent.Stop ();
		zombie.hitPoint -= damage;
		if (zombie.hitPoint <= 0) {
			for (int i = 0; i < colliders.Length; i++) {
				colliders[i].enabled = false;
			}
			StartCoroutine (Dead ());
		} else {
			Invoke ("SetMove", PlayAnimation (ZombieAnim.HURT));
		}
	}

	float PlayAnimation (ZombieAnim clip, WrapMode wrapMode = WrapMode.Once, bool crossFade=false, float fadeLenght=0.25f, bool playQueue = false) {
		AnimationClip[] clips = animClips [clip];
		AnimationClip animClip = clips [Random.Range (0, clips.Length)];
		if (wrapMode == WrapMode.Loop && anim.IsPlaying (animClip.name))
			return 0;
		if(anim.GetClip (animClip.name) == null)
			anim.AddClip(animClip, animClip.name);
		anim.wrapMode = wrapMode;
		if (animClip != null) {
			if (crossFade) {
				if(playQueue)
					anim.CrossFadeQueued (animClip.name, fadeLenght, QueueMode.CompleteOthers);
				else
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
		isInited = true;
	}

	IEnumerator Move () {
		var moveTypes = new[] {
			ProportionValue.Create (zombie.walkRate, ZombieAnim.WALK),
			ProportionValue.Create (1 - zombie.walkRate, ZombieAnim.RUN)
		};

		ZombieAnim move = moveTypes.ChooseByRandom ();
		PlayAnimation (move, WrapMode.Loop, true, 0.25f);
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
			StartCoroutine ("Move");
		}
	}

	IEnumerator Dead () {
		StopCoroutine ("Move");
		StopCoroutine ("Rest");
		StopCoroutine ("Attack");
		navAgent.Stop ();
		yield return new WaitForSeconds(PlayAnimation (ZombieAnim.DEAD, WrapMode.Once, true, 0.25f));
		Destroy (gameObject);
	}

	IEnumerator Attack () {
		var attackTypes = new[] {
			ProportionValue.Create (zombie.heaveAttackRate, ZombieAnim.HEAVY_ATTACK),
			ProportionValue.Create (1 - zombie.heaveAttackRate, ZombieAnim.ATTACK)
		};
		ZombieAnim attack = attackTypes.ChooseByRandom ();
		float attackTime = PlayAnimation (attack, WrapMode.Once, true);
		yield return new WaitForSeconds (attackTime * 0.75f);
		float damage = attack == ZombieAnim.ATTACK ? zombie.normalDamage : zombie.heavyDamage;
		EventDispatcher.TriggerEvent<float> ("PlayerHurt", damage);
		//PlayerController.Instance.Hurt (damage);
	}

	IEnumerator Rest () {
		PlayAnimation (ZombieAnim.IDLE, WrapMode.Loop, true, 0.1f);
		yield return new WaitForSeconds (zombie.restTime);
		SetNextTarget ();
	}

	void Idle () {
		PlayAnimation (ZombieAnim.IDLE, WrapMode.Loop, true, 0.25f);
	}

	bool CheckIsReachWaypoint () {
		if (!navAgent.pathPending)
		{
			if (navAgent.remainingDistance <= navAgent.stoppingDistance)
			{
				if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude <= 0.1f)
				{
					return true;
				}
			}
		}

		return false;
	}
}