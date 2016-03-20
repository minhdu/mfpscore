using UnityEngine;
using System.Collections;


public class slugger : WeaponBehaviour {
	
	public Transform projectile;
	public Transform projectilePos;

	protected override void OnStart () {
		myAudioSource.Stop();
		fireAudioSource.Stop();
		anim = GetComponent<Animation> ();

		anim.Stop();
		if (isreloading) {
			Reload ();
		} else {
			clipShell.gameObject.SetActive (true);

			myAudioSource.clip = readySound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play ();

			anim.Play (readyAnim.name);
			canaim = true;
			canfire = true;
		}
	}

	protected override void Shoot () {
		if (!anim.isPlaying) {
			float randomZ = Random.Range (-0.05f,-0.01f);
			//float randomY = Random.Range (-0.1f,0.1f);

			trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, trans.localPosition.z + randomZ);

			FPSCamera.Instance.DoRecoil (recoil);

			StartCoroutine(FlashMuzzle());

			Instantiate(projectile, projectilePos.transform.position,projectilePos.transform.rotation);
			StartCoroutine(EjectShell(shellejectdelay));
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
}

