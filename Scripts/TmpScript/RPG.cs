using UnityEngine;
using System.Collections;


public class RPG : WeaponBehaviour {
	
	public Transform rocket;
	public Transform projectile;
	public Transform projectilePos;
	public Transform muzzle;

	protected override void OnStart ()
	{
		myAudioSource.Stop();
		fireAudioSource.Stop();

		anim = GetComponent<Animation> ();
		anim.Stop();
		if (isreloading) {
			Reload ();
		} else {
			rocket.gameObject.SetActive (true);
			myAudioSource.clip = readySound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play ();

			GetComponent<Animation> ().Play (readyAnim.name);
			canaim = true;
			canfire = true;
		}
	}

	protected override void Shoot () {
		if (!anim.isPlaying)
		{
			float randomZ = Random.Range (-0.05f,-0.01f);
			//float randomY = Random.Range (-0.1f,0.1f);
			StartCoroutine(FlashMuzzle());
			trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, trans.localPosition.z + randomZ);

			FPSCamera.Instance.DoRecoil (recoil);

			rocket.gameObject.SetActive (false);
			Instantiate(projectile, projectilePos.transform.position,projectilePos.transform.rotation);

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

	protected override void Reload () {
		if (!anim.isPlaying && canreload && !isreloading && currentammo == 0) {
			StartCoroutine(SetReload (anim[reloadAnim.name].length));

			myAudioSource.clip = reloadSound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play();		

			anim.Play(reloadAnim.name);

			ammoToReload = Mathf.Clamp (ammoToReload, ammoToReload, ammo);

			ammo -= ammoToReload;
			currentammo += ammoToReload;
		}
	}

	protected override IEnumerator SetReload (float waitTime)
	{
		WeaponHandler inventory = player.GetComponent<WeaponHandler>();
		isreloading = true;
		inventory.canswitch = false;
		canaim = false;
		yield return new WaitForSeconds (waitTime * 0.4f);
		rocket.gameObject.SetActive (true);
		yield return new WaitForSeconds (waitTime * 0.6f);
		isreloading = false;
		canaim = true;
		inventory.canswitch = true;
	}
}

