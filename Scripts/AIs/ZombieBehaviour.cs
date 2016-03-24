using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieBehaviour : CoroutinableMono, IEventListener {

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
		if (zombie.hitPoint <= 0 && state != ZombieState.Die)
			return;
		animator.SetInteger ("CurrentState", (int)state);
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
		var moveTypes = new[] {
			ProportionValue.Create (zombie.angryRate, ZombieState.Walk),
			ProportionValue.Create (1 - zombie.angryRate, ZombieState.Angry)
		};
		ZombieState moveType = moveTypes.ChooseByRandom ();
		if(moveType == ZombieState.Walk)
			MoveAgent();
		StartCoroutine (SetRandomBehavior ());
	}

	public void SetActive () {
		curWaypoint = -1;
		isActive = false;
	}

	public void MoveAgent () {
		navAgent.SetDestination (wayPoints[curWaypoint].Position);
		navAgent.acceleration = 8;
		navAgent.Resume ();
	}

	public void Hurt (float damage) {
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

	IEnumerator SetRandomBehavior () {
		yield return new WaitForSeconds (zombie.randomBehaviourTime);
		var randomRest = new[] {
			ProportionValue.Create (zombie.randomRestRate, true),
			ProportionValue.Create (1 - zombie.randomRestRate, false)
		};
		bool isRandomRest = randomRest.ChooseByRandom();
		if (isRandomRest) {
			ChangeState (ZombieState.Idle);
			yield return new WaitForSeconds (Random.Range (zombie.minRestTime, zombie.maxRestTime));
			SetMove ();
		} else {
			yield return null;
		}
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