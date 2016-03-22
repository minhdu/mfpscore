using UnityEngine;
using System.Collections;


public class FlameThrower : WeaponBehaviour {
	
	public Transform flames;

	private AnimationCurve curve;

	float curAmmo;

	new void Start()
	{
		curve = new AnimationCurve ();
		curve.AddKey(0.0f,0.1f);
		curve.AddKey(0.75f,1.0f);
		anim = GetComponent<Animation>();
		trans = GetComponent<Transform> ();
		nextField = normalFOV ;
		weaponnextfield = weaponnormalFOV;
		anim.Stop();
		OnStart();
	}

	new void Update () {
		float step = speed * Time.deltaTime;
		
		float newField = Mathf.Lerp(Camera.main.fieldOfView, nextField, Time.deltaTime * 2);
		float newfieldweapon = Mathf.Lerp(weaponcamera.fieldOfView, weaponnextfield, Time.deltaTime * 2);
		Camera.main.fieldOfView = newField;
		weaponcamera.fieldOfView = newfieldweapon;

		if (retract)
		{
			canfire = false;
			canaim = false;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, retractPos, 5 * Time.deltaTime);
			weaponnextfield = weaponnormalFOV;
			nextField = normalFOV;
		}
		WeaponHandler inventory = player.GetComponent<WeaponHandler>();
		inventory.currentammo = Mathf.RoundToInt(currentammo);
		inventory.totalammo = Mathf.RoundToInt(ammo);
		canfire = true;

		trans.localRotation = Quaternion.Lerp(trans.localRotation, Quaternion.Euler(wantedrotation), 5f * Time.deltaTime);

		if (isAiming && canaim) {
			trans.localPosition = Vector3.MoveTowards(trans.localPosition, aimposition, step);
			weaponnextfield = weaponaimFOV;
			nextField = aimFOV;

		} else {
			trans.localPosition = Vector3.MoveTowards(trans.localPosition, normalposition, step);
			weaponnextfield = weaponnormalFOV;
			nextField = normalFOV;
		}

		if (isShooting && canfire && !isreloading && !anim.IsPlaying (readyAnim.name)&& !anim.IsPlaying (hideAnim.name)) {
			
			curAmmo -= 5f * Time.deltaTime;
			ParticleSystem[] particleSystems;
			particleSystems = flames.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particle in particleSystems) {
				var em = particle.emission;
				em.rate= new ParticleSystem.MinMaxCurve(100f,curve);
			}
			if (!fireAudioSource.isPlaying) {
				fireAudioSource.clip = fireSound;
				fireAudioSource.loop = true;
				fireAudioSource.Play();
			}
			
			anim[fireAnim.name].speed = fireAnimSpeed; 
			anim.Play(fireAnim.name);
		}
		else {
			ParticleSystem[] particleSystems;
			particleSystems = flames.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particle in particleSystems)
			{
				var em = particle.emission;
				em.rate= new ParticleSystem.MinMaxCurve(0,curve);
			}
			fireAudioSource.Stop();
			GetComponent<Animation>()[fireAnim.name].speed = 0f;
			
		}

		if (curAmmo  <= 0f ) {	
			if (ammo <= 0f) {
				canfire = false;
				canreload = false;
				if (isShooting && !myAudioSource.isPlaying) {
					myAudioSource.PlayOneShot(emptySound);
				} else {
					canreload = true;
				}
			} else {
				Reload();
			}
		}
	}

	protected override void Reload () {
		if (canreload && !isreloading) {
			StartCoroutine(SetReload (anim [reloadAnim.name].length));

			myAudioSource.clip = reloadSound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play();		

			anim.Play(reloadAnim.name);

			ammoToReload = (int)Mathf.Clamp (ammoToReload, ammoToReload, ammo);

			ammo -= ammoToReload;
			curAmmo += ammoToReload;
		} 
	}
}

