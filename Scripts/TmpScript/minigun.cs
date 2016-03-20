using UnityEngine;
using System.Collections;

public class minigun : WeaponBehaviour {

	public AudioSource servoAudioSource;
	public AudioClip servosound;
	public Transform barrels;
	public Vector3 barrelrotatedirection;

	[SerializeField]
	float barrelsrotatespeed = 0f;
	[SerializeField]
	float wantedspeed = 0f;
	[SerializeField]
	float wantedpitch = 0f;
	[SerializeField]
	float pitchspeed;

	float nextshot;
	public float shotinterval = 0.2f;

	new void Update () {
		float step = speed * Time.deltaTime;
		
		float newField = Mathf.Lerp(Camera.main.fieldOfView, nextField, Time.deltaTime * 2);
		float newfieldweapon = Mathf.Lerp(weaponcamera.fieldOfView, weaponnextfield, Time.deltaTime * 2);
		Camera.main.fieldOfView = newField;
		weaponcamera.fieldOfView = newfieldweapon;

		wantedspeed = Mathf.Lerp(wantedspeed, barrelsrotatespeed, Time.deltaTime * 2f);
		wantedpitch = Mathf.Lerp(wantedpitch , pitchspeed, Time.deltaTime * 2f);
		barrels.Rotate(barrelrotatedirection  * Time.deltaTime * wantedspeed); 

		#if UNITY_EDITOR
		float Xtilt = Input.GetAxisRaw("Mouse Y") * 20f * Time.smoothDeltaTime;
		float Ytilt = Input.GetAxisRaw("Mouse X") * 20f * Time.smoothDeltaTime;
		#else
		float Xtilt = FPSCamera.Instance.YInput * 20f * Time.smoothDeltaTime;
		float Ytilt = FPSCamera.Instance.XInput * 20f * Time.smoothDeltaTime;
		#endif

		if (retract) {
			canfire = false;
			canaim = false;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, retractPos, 5 * Time.deltaTime);
			weaponnextfield = weaponnormalFOV;
			nextField = normalFOV;
		}

		WeaponHandler inventory = player.GetComponent<WeaponHandler>();
		inventory.currentammo = currentammo;
		inventory.totalammo = ammo;

		if (wantedspeed >= 0.35f) {
			servoAudioSource.pitch = wantedpitch;
			if (!servoAudioSource.isPlaying) {
				
				servoAudioSource.clip = servosound;
				servoAudioSource.loop = true;
				servoAudioSource.Play();
			}
			if (wantedspeed >= 500f) {
				canfire = true;
			} else {
				canfire = false;
			}
		} else {
			servoAudioSource.Stop();
		}

		wantedrotation = new Vector3(-10f + Xtilt, Ytilt, 0f);
		trans.localRotation = Quaternion.Lerp(trans.localRotation, Quaternion.Euler(wantedrotation), 5f * Time.deltaTime);

		if (isAiming && canaim) {
			inaccuracy = spreadAim;
			barrelsrotatespeed = 600f;
			pitchspeed = 1.0f;
			trans.localPosition = Vector3.MoveTowards(trans.localPosition, aimposition, step);
			weaponnextfield = weaponaimFOV;
			nextField = aimFOV;

		} else {
			inaccuracy = spreadNormal;
			barrelsrotatespeed = 0f;
			pitchspeed = 0.3f;
			trans.localPosition = Vector3.MoveTowards(trans.localPosition, normalposition, step);
			weaponnextfield = weaponnormalFOV;
			nextField = normalFOV;
		}

		if (currentammo == 0 || currentammo  <= 0) {	
			if (ammo <= 0) {
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
		
		if (isShooting  && !isreloading && canfire) {
			Shoot();
		}
	}

	protected override void OnStart () {
		myAudioSource.Stop();
		fireAudioSource.Stop();

		anim = GetComponent<Animation> ();

		raycastfire.Instance.inaccuracy = inaccuracy;
		raycastfire.Instance.damage = damage;
		raycastfire.Instance.range = range;
		raycastfire.Instance.force = force;
		raycastfire.Instance.projectilecount = projectilecount;

		anim.Stop();
		if (isreloading) {
			Reload ();
		} else {
			if(clipShell != null)
				clipShell.gameObject.SetActive (true);

			myAudioSource.clip = readySound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play ();

			anim.Play (readyAnim.name);
			canaim = true;
			canfire = false;
		}
	}
}

