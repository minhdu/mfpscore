using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieBehaviour : CoroutinableMono, IEventListener {

	public Zombie zombie;
	public Animation anim;
	NavMeshAgent navAgent;
	Transform mTransform;
	public Collider[] colliders;
	public Collider headColliders;

	[SerializeField]
	bool isInited = false;

	[SerializeField]
	bool isActive = false;

	[SerializeField]
	bool haveDestination = false;

	[SerializeField]
	bool isReachWaypoint = false;

	[SerializeField]
	Waypoint[] wayPoints;

	[SerializeField]
	int curWaypoint = -1;

	[SerializeField]
	float nextAttackTime;

	[SerializeField]
	float nextRestTime;

	[SerializeField]
	float damage;

	public AnimationClip wakeupAnim;
	public AnimationClip idleAnim;
	public AnimationClip walkAnim;
	public AnimationClip runAnim;
	public AnimationClip angryAnim;
	public AnimationClip hurtAnim;
	public AnimationClip attackAnim;
	public AnimationClip heavyAttackAnim;
	public AnimationClip dieAnim;

	AnimationClip[] animClips;

	void OnEnable () {
		Init (FindPlayer());
	}

	public Waypoint[] FindPlayer () {
		return FindObjectsOfType<Waypoint> ();
	}

	public void ListenEvents () {
		for(int i=0; i<colliders.Length; i++) {
			EventDispatcher.AddEventListener<float> (GameEvents.GameplayEvents.DAMAGE + colliders[i].GetInstanceID(), Hurt);
		}
		EventDispatcher.AddEventListener<float> (GameEvents.GameplayEvents.DAMAGE + headColliders.GetInstanceID(), BeHeadShot);
	}

	public void Init (params Waypoint[] waypoints) {
		isActive = true;

		if (isInited) {
			SetNextTarget ();
			return;
		}

		animClips = new AnimationClip[] {
			wakeupAnim,
			idleAnim,
			walkAnim,
			runAnim,
			angryAnim,
			attackAnim,
			heavyAttackAnim,
			hurtAnim,
			dieAnim
		};

		for (int i = 0; i < animClips.Length; i++) {
			anim.AddClip (animClips [i], animClips[i].name);
		}

		mTransform = GetComponent<Transform> ();
		navAgent = GetComponent<NavMeshAgent> ();
		colliders = GetComponentsInChildren<Collider> ();
		wayPoints = waypoints;
		isReachWaypoint = false;

		ListenEvents ();
		SetNextTarget ();
		isInited = true;
	}

	void Update () {
		if (!isInited || !isActive)
			return;

		if (!isReachWaypoint) {
			isReachWaypoint = CheckIsReachWaypoint ();
			if (isReachWaypoint)
				StopAgent ();
		}

		if (isReachWaypoint && zombie.hitPoint > 0) {
			if (wayPoints [curWaypoint].isPlayer) {
				if (Time.time > nextAttackTime + zombie.attackCooldown) {
					StartCoroutine("SetAttack");
					nextAttackTime = Time.time;
				}
			} else {
				if (Time.time > nextRestTime + zombie.maxRestTime) {
					ChangeState(ZombieState.Idle, WrapMode.Loop);
					nextRestTime = Time.time;
				}
			}
		}
	}

	public float ChangeState (ZombieState state, WrapMode wrapMode = WrapMode.Default, bool isCrossFade = true, float fadeLenght = 0.3f) {
		//Debug.Log (state);
		if (zombie.hitPoint <= 0 && state != ZombieState.Die)
			return 0;

		AnimationClip clip = animClips [(int)state];
		string stateName = clip.name;

		if (anim.IsPlaying (stateName))
			return 0;
		
		anim.wrapMode = wrapMode;

		if (isCrossFade) {
			anim.CrossFade (stateName, fadeLenght);
		} else {
			anim.Play (stateName);
		}

		if (isCrossFade)
			return clip.length - fadeLenght;
		else
			return clip.length;
	}

	IEnumerator SetAttack () {
		var attackTypes = new[] {
			ProportionValue.Create (zombie.heavyAttackRate, ZombieState.HeavyAttack),
			ProportionValue.Create (1 - zombie.heavyAttackRate, ZombieState.Attack)
		};
		ZombieState attackType = attackTypes.ChooseByRandom ();
		if (attackType == ZombieState.HeavyAttack)
			damage = zombie.heavyDamage;
		else
			damage = zombie.normalDamage;
		yield return new WaitForSeconds(ChangeState (attackType, WrapMode.Once));
		if (zombie.hitPoint <= 0)
			yield break;
		ChangeState (ZombieState.Idle, WrapMode.Loop);
	}

	IEnumerator SetMove () {
		if(zombie.hitPoint <=0) {
			yield break;
		}
		var moveTypes = new[] {
			ProportionValue.Create (zombie.angryRate, ZombieState.Angry),
			ProportionValue.Create (1 - zombie.angryRate, ZombieState.Walk)
		};
		ZombieState moveType = moveTypes.ChooseByRandom ();
		if (moveType == ZombieState.Walk) {
			ChangeState (moveType, WrapMode.Loop);
			MoveAgent ();
			navAgent.speed = zombie.walkSpeed;
			yield break;
		} else {
			yield return new WaitForSeconds (ChangeState (moveType, WrapMode.Once));
			if (zombie.hitPoint <= 0)
				yield break;
			MoveAgent ();
			navAgent.speed = zombie.runSpeed;
			ChangeState (ZombieState.Run, WrapMode.Loop);
		}
	}

	public void SetInActive () {
		curWaypoint = -1;
		isActive = false;
		haveDestination = false;
	}

	void MoveAgent () {
		navAgent.SetDestination (wayPoints[curWaypoint].Position);
		navAgent.acceleration = 8;
		navAgent.Resume ();
		haveDestination = true;
	}

	void StopAgent () {
		navAgent.acceleration = 90;
		navAgent.Stop ();
	}

	public void BeHeadShot (float damage) {
		zombie.hitPoint = 0;
		Die ();
		MainGameUI.Instance.SetHeadShot ();
	}

	public void Die () {
		SetInActive ();
		for (int i = 0; i < colliders.Length; i++) {
			colliders[i].enabled = false;
		}
		ChangeState(ZombieState.Die, WrapMode.Once);
		EventDispatcher.TriggerEvent (GameEvents.GameplayEvents.ZOMBIE_DEAD);
		Destroy (gameObject, 2);
	}

	public void Hurt (float damage) {
		StopAgent ();
		zombie.hitPoint -= damage;
		if (zombie.hitPoint <= 0) {
			Die ();
		} else {
			if (!anim.IsPlaying (hurtAnim.name)) {
				StartCoroutine ("HurtThenResume");
			}
		}
	}

	IEnumerator HurtThenResume () {
		yield return new WaitForSeconds(ChangeState (ZombieState.Hurt, WrapMode.Once));
		StartCoroutine ("SetMove");
	}

	public void CauseDamage () {
		EventDispatcher.TriggerEvent<float> ("PlayerHurt", damage);
	}
	
	public void SetNextTarget () {
		curWaypoint++;
		StartCoroutine("SetMove");
		isReachWaypoint = false;
	}
	
	bool CheckIsReachWaypoint () {
		if (!isInited || !isActive || !haveDestination)
			return false;
		if (!navAgent.pathPending) {
			if (navAgent.remainingDistance <= navAgent.stoppingDistance) {
				if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude <= 0.1f) {
					return true;
				}
			}
		}
		return false;
	}
}