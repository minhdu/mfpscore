using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnuGames.StateMachine;

public class ZombieController : CoroutinableMono, IEventListener {

	public Zombie zombie;
	public Animation anim;
	NavMeshAgent navAgent;
	public Collider[] colliders;

	[SerializeField]
	bool isInited = false;

	[SerializeField]
	bool isReachWaypoint = false;

	[SerializeField]
	Waypoint[] wayPoints;
	[SerializeField]
	int curWaypoint = -1;
	float nextAttackTime;
	float nextRestTime;

	StateMachine<ZombieState> fsm;

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

	public Waypoint[] FindPlayer () {
		return FindObjectsOfType<Waypoint> ();
	}

	public void ListenEvents () {
		for(int i=0; i<colliders.Length; i++) {
			EventDispatcher.AddEventListener<float> (GameEvents.GameplayEvents.DAMAGE + colliders[i].GetInstanceID(), Hurt);
		}
	}

	public void Init (params Waypoint[] waypoints) {
		fsm = StateMachine<ZombieState>.Initialize (this);

		navAgent = GetComponent<NavMeshAgent> ();
		colliders = GetComponentsInChildren<Collider> ();
		wayPoints = waypoints;
		curWaypoint = -1;
		isReachWaypoint = false;

		animClips.Add (ZombieAnim.AWAKE, awakeClips);
		animClips.Add (ZombieAnim.IDLE, idleClips);
		animClips.Add (ZombieAnim.WALK, walkClips);
		animClips.Add (ZombieAnim.RUN, runClips);
		animClips.Add (ZombieAnim.ATTACK, attackClips);
		animClips.Add (ZombieAnim.HEAVY_ATTACK, heavyAttackClips);
		animClips.Add (ZombieAnim.HURT, hurtClips);
		animClips.Add (ZombieAnim.DEAD, deadClips);

		ChangeState (ZombieState.Wakeup);

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
					ChangeState(ZombieState.Attack);
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
		fsm.ChangeState (state, StateTransition.Overwrite);
	}

	public void Hurt (float damage) {
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
	
	public void SetNextTarget () {
		curWaypoint++;
		SetMove ();
		isReachWaypoint = false;
	}
	
	void SetMove () {
		var moveTypes = new[] {
			ProportionValue.Create (zombie.walkRate, ZombieState.Walk),
			ProportionValue.Create (1 - zombie.walkRate, ZombieState.Run)
		};
		
		ChangeState (moveTypes.ChooseByRandom ());
		navAgent.SetDestination (wayPoints[curWaypoint].Position);
		navAgent.Resume ();
	}
	
	IEnumerator SetRandomBehavior () {
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
	
	bool CheckIsReachWaypoint () {
		if (!isInited)
			return false;
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

	#region State

	IEnumerator OnWakeupEnter () {
		yield return new WaitForSeconds (PlayAnimation (ZombieAnim.AWAKE));
		SetNextTarget ();
		isInited = true;
	}

	void OnWalkEnter () {
		PlayAnimation (ZombieAnim.WALK, WrapMode.Loop, true, 0.25f);
		navAgent.speed = zombie.walkSpeed;
		StartCoroutine(SetRandomBehavior ());
	}

	void OnRunEnter () {
		PlayAnimation (ZombieAnim.RUN, WrapMode.Loop, true, 0.25f);
		navAgent.speed = zombie.runSpeed;
		StartCoroutine(SetRandomBehavior ());
	}

	void OnIdleEnter () {
		PlayAnimation (ZombieAnim.IDLE, WrapMode.Loop, true, 0.25f);
	}

	IEnumerator OnHurtEnter () {
		yield return new WaitForSeconds(PlayAnimation (ZombieAnim.HURT));
		SetMove ();
	}

	IEnumerator OnDieEnter () {
		navAgent.Stop ();
		yield return new WaitForSeconds(PlayAnimation (ZombieAnim.DEAD, WrapMode.Once, true, 0.25f));
		Destroy (gameObject);
	}

	IEnumerator OnAttackEnter () {
		var attackTypes = new[] {
			ProportionValue.Create (zombie.heaveAttackRate, ZombieAnim.HEAVY_ATTACK),
			ProportionValue.Create (1 - zombie.heaveAttackRate, ZombieAnim.ATTACK)
		};
		ZombieAnim attack = attackTypes.ChooseByRandom ();
		float attackTime = PlayAnimation (attack, WrapMode.Once, true);
		yield return new WaitForSeconds (attackTime * 0.75f);
		float damage = attack == ZombieAnim.ATTACK ? zombie.normalDamage : zombie.heavyDamage;
		EventDispatcher.TriggerEvent<float> ("PlayerHurt", damage);
	}

	void OnAttackExit () {
		ChangeState (ZombieState.Idle);
	}

	#endregion
}