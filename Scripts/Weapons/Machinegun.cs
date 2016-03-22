using UnityEngine;
using System.Collections;


public class Machinegun : WeaponBehaviour {
	
	public Transform bullet1;
	public Transform bullet2;
	public Transform bullet3;
	public Transform bullet4;
	public Transform bullet5;
	public Transform bullet6;
	public Transform bullet7;
	public Transform bullet8;
	private Transform nextbullet;

	void Update ()  {
		float step = speed * Time.deltaTime;
		
		float newField = Mathf.Lerp(Camera.main.fieldOfView, nextField, Time.deltaTime * 2);
		float newfieldweapon = Mathf.Lerp(weaponcamera.fieldOfView, weaponnextfield, Time.deltaTime * 2);
		Camera.main.fieldOfView = newField;
		weaponcamera.fieldOfView = newfieldweapon;
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
		canfire = true;

		trans.localRotation = Quaternion.Lerp(trans.localRotation,Quaternion.Euler(wantedrotation),5f * Time.deltaTime);

		if (isAiming && canaim) {
			inaccuracy = spreadAim;

			trans.localPosition = Vector3.MoveTowards(trans.localPosition, aimposition, step);
			weaponnextfield = weaponaimFOV;
			nextField = aimFOV;

		}
		else {
			
			inaccuracy = spreadNormal;
			
			trans.localPosition = Vector3.MoveTowards(trans.localPosition, normalposition, step);
			weaponnextfield = weaponnormalFOV;
			nextField = normalFOV;

		}

		if (currentammo < 8) {
			if (currentammo == 6) {
				bullet8.gameObject.SetActive(false);
				nextbullet = bullet7;
			} else if (currentammo == 5) {
				bullet7.gameObject.SetActive(false);
				bullet8.gameObject.SetActive(false);
				nextbullet = bullet6;
			} else if (currentammo == 4) {
				nextbullet = bullet5;
				bullet8.gameObject.SetActive(false);
				bullet7.gameObject.SetActive(false);
				bullet6.gameObject.SetActive(false);
			} else if (currentammo == 3) {
				nextbullet = bullet4;
				bullet8.gameObject.SetActive(false);
				bullet7.gameObject.SetActive(false);
				bullet6.gameObject.SetActive(false);
				bullet5.gameObject.SetActive(false);
			} else if (currentammo == 2) {
				nextbullet = bullet3;
				bullet8.gameObject.SetActive(false);
				bullet7.gameObject.SetActive(false);
				bullet6.gameObject.SetActive(false);
				bullet5.gameObject.SetActive(false);
				bullet4.gameObject.SetActive(false);
			} else if (currentammo == 1) {
				nextbullet = bullet2;
				bullet8.gameObject.SetActive(false);
				bullet7.gameObject.SetActive(false);
				bullet6.gameObject.SetActive(false);
				bullet5.gameObject.SetActive(false);
				bullet4.gameObject.SetActive(false);
				bullet3.gameObject.SetActive(false);
			} else if (currentammo == 0 || currentammo  <= 0 ) {	
				nextbullet = bullet1;
				
				bullet8.gameObject.SetActive(false);
				bullet7.gameObject.SetActive(false);
				bullet6.gameObject.SetActive(false);
				bullet5.gameObject.SetActive(false);
				bullet4.gameObject.SetActive(false);
				bullet3.gameObject.SetActive(false);
				bullet2.gameObject.SetActive(false);
				bullet1.gameObject.SetActive(false);
				if (ammo <= 0) {
					canfire = false;
					canreload = false;
					if (isShooting && canfire) {
						myAudioSource.PlayOneShot(emptySound);
					} else {
						canreload = true;
					}
				} else {
					Reload();
				}
			}
		} else {
			
			bullet8.gameObject.SetActive(true);
			bullet7.gameObject.SetActive(true);
			bullet6.gameObject.SetActive(true);
			bullet5.gameObject.SetActive(true);
			bullet4.gameObject.SetActive(true);
			bullet3.gameObject.SetActive(true);
			bullet2.gameObject.SetActive(true);
			bullet1.gameObject.SetActive(true);
			nextbullet = null;
		}

		if (isShooting && !isreloading && canfire) {
			Shoot();
		}
	}
	
	protected override void Shoot() {
		if (!anim.isPlaying) {
			if (nextbullet != null) {
				nextbullet.gameObject.SetActive (false);
			}
			float randomZ = Random.Range (-0.05f,0.05f);
			//float randomY = Random.Range (-0.1f,0.1f);
			
			trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, trans.localPosition.z + randomZ);
			StartCoroutine(FlashMuzzle());
			CameraRotate.Instance.DoRecoil (recoil);

			RaycastFire.Instance.Fire ();

			fireAudioSource.clip = fireSound;
			fireAudioSource.pitch = 0.9f + 0.2f * Random.value;
			fireAudioSource.Play ();
			anim[fireAnim.name].speed = fireAnimSpeed;     
			anim.Play(fireAnim.name);
			currentammo -= 1;
			StartCoroutine (EjectShell (shellejectdelay));
			
			if (currentammo <= 0) {
				Reload();
			}
			
		} else {
			if (nextbullet != null) {
				nextbullet.gameObject.SetActive (true);
			}
		}

		
		
	}
	
	protected override void Reload() {
		if (!anim.isPlaying && canreload && !isreloading) {
			
			StartCoroutine (SetAmmo (anim [reloadAnim.name].length * 0.35f ));
			StartCoroutine(SetReload (anim [reloadAnim.name].length));
			
			myAudioSource.clip = reloadSound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play();		
			
			anim.Play(reloadAnim.name);
		} 
	}
	

	IEnumerator SetAmmo(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		ammoToReload = Mathf.Clamp(ammoToReload, ammoToReload, ammo);
		
		ammo -= ammoToReload;
		currentammo += ammoToReload;
	}

	protected override IEnumerator SetReload(float waitTime) {
		WeaponHandler selector = player.GetComponent<WeaponHandler>();
		selector.canswitch = false;
		isreloading = true;
		canaim = false;
		yield return new WaitForSeconds (waitTime);
		isreloading = false;
		canaim = true;
		selector.canswitch = true;
	}
}

