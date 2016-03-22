using UnityEngine;
using System.Collections;


public class Revolver : WeaponBehaviour {
	
	public AudioClip toreloadSound;
	public AudioClip reloadonceSound;
	public AudioClip reloadlastSound;

	public Transform bullet1;
	public Transform bullet2;
	public Transform bullet3;
	public Transform bullet4;
	public Transform bullet5;
	public Transform bullet6;
	int bulletactivator = 6;

	public AnimationClip toreloadAnim;
	public AnimationClip reloadonceAnim;
	public AnimationClip reloadlastAnim;

	void Update () {
		
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

		trans.localRotation = Quaternion.Lerp(trans.localRotation, Quaternion.Euler(wantedrotation), 5f * Time.deltaTime);

		if (isAiming && canaim) {
			inaccuracy = spreadAim;

			transform.localPosition = Vector3.MoveTowards(transform.localPosition, aimposition, step);
			weaponnextfield = weaponaimFOV;
			nextField = aimFOV;

		}
		else {
			inaccuracy = spreadNormal;
			
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);
			weaponnextfield = weaponnormalFOV;
			nextField = normalFOV;

		}
		
		if (bulletactivator == 0) {
			bullet1.gameObject.SetActive(false);
			bullet2.gameObject.SetActive(false);
			bullet3.gameObject.SetActive(false);
			bullet4.gameObject.SetActive(false);
			bullet5.gameObject.SetActive(false);
			bullet6.gameObject.SetActive(false);
		}
		else if (bulletactivator == 1) {
			bullet1.gameObject.SetActive(true);
			bullet2.gameObject.SetActive(false);
			bullet3.gameObject.SetActive(false);
			bullet4.gameObject.SetActive(false);
			bullet5.gameObject.SetActive(false);
			bullet6.gameObject.SetActive(false);
		}
		else if (bulletactivator == 2) {
			bullet1.gameObject.SetActive(true);
			bullet2.gameObject.SetActive(true);
			bullet3.gameObject.SetActive(false);
			bullet4.gameObject.SetActive(false);
			bullet5.gameObject.SetActive(false);
			bullet6.gameObject.SetActive(false);
		}
		else if (bulletactivator == 3) {
			bullet1.gameObject.SetActive(true);
			bullet2.gameObject.SetActive(true);
			bullet3.gameObject.SetActive(true);
			bullet4.gameObject.SetActive(false);
			bullet5.gameObject.SetActive(false);
			bullet6.gameObject.SetActive(false);
		}
		else if (bulletactivator == 4) {
			bullet1.gameObject.SetActive(true);
			bullet2.gameObject.SetActive(true);
			bullet3.gameObject.SetActive(true);
			bullet4.gameObject.SetActive(true);
			bullet5.gameObject.SetActive(false);
			bullet6.gameObject.SetActive(false);
		}
		else if (bulletactivator == 5) {
			bullet1.gameObject.SetActive(true);
			bullet2.gameObject.SetActive(true);
			bullet3.gameObject.SetActive(true);
			bullet4.gameObject.SetActive(true);
			bullet5.gameObject.SetActive(true);
			bullet6.gameObject.SetActive(false);
		}
		else if (bulletactivator == 6) {
			bullet1.gameObject.SetActive(true);
			bullet2.gameObject.SetActive(true);
			bullet3.gameObject.SetActive(true);
			bullet4.gameObject.SetActive(true);
			bullet5.gameObject.SetActive(true);
			bullet6.gameObject.SetActive(true);
		}

		if (currentammo == 0 || currentammo  <= 0 ) {	
			if (ammo <= 0) {
				canfire = false;
				canreload = false;
				if (isShooting && !myAudioSource.isPlaying) {
					myAudioSource.PlayOneShot(emptySound);
				}
				else {
					canreload = true;
				}
			}
			else {
				Reload();
			}
		}

		
		if (isShooting  && !isreloading && canfire) {
			Shoot();
		}
		
		
	}

	protected override void Shoot () {
		if (!anim.isPlaying) {

			float randomZ = Random.Range (-0.05f,-0.01f);
			//float randomY = Random.Range (-0.1f,0.1f);

			trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y ,trans.localPosition.z + randomZ);

			CameraRotate.Instance.DoRecoil(recoil);

			StartCoroutine(FlashMuzzle());

			RaycastFire.Instance.Fire ();

			fireAudioSource.clip = fireSound;
			fireAudioSource.pitch = 0.9f + 0.1f *Random.value;
			fireAudioSource.Play();
			anim[fireAnim.name].speed = fireAnimSpeed;     
			anim.Play(fireAnim.name);
			currentammo -=1;

			if (currentammo <= 0) {
				Reload();
			}
		}
	}

	protected override void Reload() { 
		if (!anim.isPlaying && canreload && !isreloading) { 
			StartCoroutine(setreload ());
		} 
	}

	IEnumerator setreload() {
		ammoToReload = Mathf.Clamp (ammoToReload, ammoToReload, ammo);
		//reload first
		isreloading = true;
		canaim = false;
		WeaponHandler inventoryscript = player.GetComponent<WeaponHandler>();
		inventoryscript.canswitch = false;
		myAudioSource.clip = toreloadSound;
		myAudioSource.Play();
		
		//GetComponent<Animation>()[reloadAnim.name].time = startTime;
		GetComponent<Animation> ().Play(toreloadAnim.name);
		yield return new WaitForSeconds (GetComponent<Animation> () [toreloadAnim.name].length * 0.6f);
		StartCoroutine(EjectShell(shellejectdelay));
		bulletactivator = 0;
		
		yield return new WaitForSeconds (GetComponent<Animation> () [toreloadAnim.name].length * 0.4f);
		//reloadonce
		
		while(currentammo != ammoToReload) {
			GetComponent<Animation> ().Play (reloadonceAnim.name);
			myAudioSource.clip = reloadonceSound;
			myAudioSource.Play ();
			
			//GetComponent<Animation>()[reloadAnim.name].time = startTime;
			
			yield return new WaitForSeconds (GetComponent<Animation> () [reloadonceAnim.name].length * 0.4f);
			ammo -= 1;
			currentammo += 1;
			bulletactivator += 1;
			yield return new WaitForSeconds (GetComponent<Animation> () [reloadonceAnim.name].length * 0.6f);
		} 
		
		//reloadlast
		myAudioSource.clip = reloadlastSound;
		myAudioSource.Play();
		//GetComponent<Animation>()[reloadAnim.name].time = startTime;
		GetComponent<Animation> ().Play(reloadlastAnim.name);
		yield return new WaitForSeconds (GetComponent<Animation> () [reloadlastAnim.name].length);
		
		isreloading = false;
		canaim = true;
		inventoryscript.canswitch = true;
	}

	protected override IEnumerator EjectShell (float waitTime) {
		yield return new WaitForSeconds(waitTime);
		Instantiate(shell, shellPos.transform.position,shellPos.transform.rotation);
	}
}

