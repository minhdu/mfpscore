using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieBehaviour : CoroutinableMono, IEventListener {

	readonly string[] stateTriggers = new string[] {
		"Wakeup",
		"Idle",
		"Walk",
		"Run",
		"Angry",
		"Attack",
		"HeavyAttack",
		"Hurt",
		"Die"
	};

	public Zombie zombie;
	public Animator animator;
	NavMeshAgent navAgent;
	public Collider[] colliders;

	[SerializeField]
	bool isInited = false;

	[SerializeField]
	bool isActive = false;

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
	}

	public void Init (params Waypoint[] waypoints) {
		isActive = true;

		if (isInited) {
			SetNextTarget ();
			return;
		}

		navAgent = GetComponent<NavMeshAgent> ();
		colliders = GetComponentsInChildren<Collider> ();
		wayPoints = waypoints;
		isReachWaypoint = false;

		animator.SetInteger ("AnimationTemplate", Random.Range(0, 1));
		ZombieStateBase[] states = animator.GetBehaviours<ZombieStateBase> ();
		for (int i = 0; i < states.Length; i++) {
			states [i].Init (this);
		}

		ListenEvents ();
		SetNextTarget ();
	}

	void Update () {
		if (!isInited || !isActive)
			return;

		if (!isReachWaypoint)
			isReachWaypoint = CheckIsReachWaypoint ();

		if (isReachWaypoint && zombie.hitPoint > 0) {
			if (wayPoints [curWaypoint].isPlayer) {
				if (Time.time > nextAttackTime + zombie.attackCooldown) {
					SetAttack();
					nextAttackTime = Time.time;
				}
			} else {
				if (Time.time > nextRestTime + zombie.maxRestTime) {
					ChangeState(ZombieState.Idle);
					nextRestTime = Time.time;
				}
			}
		}
	}

	public void ChangeState (ZombieState state) {
		Debug.Log (state);
		if (zombie.hitPoint <= 0 && state != ZombieState.Die)
			return;
		animator.SetTrigger (stateTriggers[(int)state]);
	}

	public void SetDamage (ZombieState state) {
		if (state == ZombieState.HeavyAttack)
			damage = zombie.heavyDamage;
		else
			damage = zombie.normalDamage;
	}

	public void SetAttack () {
		var attackTypes = new[] {
			ProportionValue.Create (zombie.heavyAttackRate, ZombieState.HeavyAttack),
			ProportionValue.Create (1 - zombie.heavyAttackRate, ZombieState.Attack)
		};
		ZombieState attackType = attackTypes.ChooseByRandom ();
		ChangeState (attackType);
		SetDamage (attackType);
	}

	public void SetMove () {
		if (!enableSetMove)
			return;
		else
			enableSetMove = false;
		var moveTypes = new[] {
			ProportionValue.Create (zombie.angryRate, ZombieState.Angry),
			ProportionValue.Create (1 - zombie.angryRate, ZombieState.Walk)
		};
		ZombieState moveType = moveTypes.ChooseByRandom ();
		ChangeState (moveType);
		if (moveType == ZombieState.Walk) {
			MoveAgent ();
			SetSpeed (ZombieState.Walk);
		}
	}

	public void SetActive () {
		curWaypoint = -1;
		isActive = false;
	}

	public void SetSpeed (ZombieState state) {
		if (state == ZombieState.Walk)
			navAgent.speed = zombie.walkSpeed;
		else
			navAgent.speed = zombie.runSpeed;
	}

	public void MoveAgent () {
		navAgent.SetDestination (wayPoints[curWaypoint].Position);
		navAgent.acceleration = 8;
		navAgent.Resume ();
	}

	bool enableSetMove = false;
	public void Hurt (float damage) {
		enableSetMove = true;
		navAgent.acceleration = 90;
		navAgent.Stop ();
		zombie.hitPoint -= damage;
		if (zombie.hitPoint <= 0) {
			for (int i = 0; i < colliders.Length; i++) {
				colliders[i].enabled = false;
			}
			ChangeState(ZombieState.Die);
		} else {
			ChangeState(ZombieState.Hurt);
		}
	}

	public void CauseDamage () {
		EventDispatcher.TriggerEvent<float> ("PlayerHurt", damage);
	}
	
	public void SetNextTarget () {
		curWaypoint++;
		SetMove ();
		isReachWaypoint = false;
	}
	
	bool CheckIsReachWaypoint () {
		if (!isInited)
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