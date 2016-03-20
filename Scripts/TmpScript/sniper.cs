using UnityEngine;
using System.Collections;


public class sniper : WeaponBehaviour {
	
	public AnimationClip ejectshellAnim;

	protected override void Shoot () {
		if (!anim.isPlaying) {
			StartCoroutine(SetFire());
		}
		if (currentammo <= 0) {
			Reload();
		}
	}

	protected override void Reload() {
		if (!anim.isPlaying && canreload && !isreloading) {
			StartCoroutine(SetReload ());
			StartCoroutine (HideShell (anim [reloadAnim.name].length * 0.5f)); 
		} 
	}
	

	IEnumerator SetFire() {
		if (currentammo > 1) {
			StartCoroutine(FlashMuzzle());
			raycastfire weaponfirer = rayfirer.GetComponent<raycastfire> ();
			weaponfirer.SendMessage ("fire", SendMessageOptions.DontRequireReceiver);
			
			fireAudioSource.clip = fireSound;
			fireAudioSource.pitch = 0.9f + 0.1f * Random.value;
			fireAudioSource.Play ();
			float randomZ = Random.Range (-0.05f,-0.01f);
			//float randomY = Random.Range (-0.1f,0.1f);
			
			trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, trans.localPosition.z + randomZ);
			FPSCamera.Instance.DoRecoil (recoil);
			anim.Play (fireAnim.name);
			currentammo -= 1;
			
			yield return new WaitForSeconds (anim [fireAnim.name].length) ;
			
			GetComponent<Animation> ().Play(ejectshellAnim.name);
			myAudioSource.clip = readySound;
			
			myAudioSource.Play ();
			StartCoroutine (EjectShell (shellejectdelay));
			yield return new WaitForSeconds (anim [ejectshellAnim.name].length) ;
			
			
		} 
		else if (currentammo <= 1) {
			if (currentammo <= 0) {
				Reload ();
			}
			StartCoroutine(FlashMuzzle());
			raycastfire weaponfirer = rayfirer.GetComponent<raycastfire> ();
			weaponfirer.SendMessage ("fire", SendMessageOptions.DontRequireReceiver);
			float randomZ = Random.Range (-0.05f,-0.01f);
			//float randomY = Random.Range (-0.1f,0.1f);
			
			transform.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, trans.localPosition.z + randomZ);

			FPSCamera.Instance.DoRecoil (recoil);

			fireAudioSource.clip = fireSound;
			fireAudioSource.pitch = 0.9f + 0.1f * Random.value;
			fireAudioSource.Play ();
			
			anim.Play (fireAnim.name);
			
			currentammo -= 1;
			yield return new WaitForSeconds (fireAnim.name.Length);
		}
	}

	IEnumerator SetReload() {
		WeaponHandler selector = player.GetComponent<WeaponHandler>();
		selector.canswitch = false;
		isreloading = true;
		myAudioSource.clip = reloadSound;
		myAudioSource.loop = false;
		myAudioSource.volume = 1;
		myAudioSource.Play();		
		anim.Play(reloadAnim.name);
		
		ammoToReload = Mathf.Clamp (ammoToReload, ammoToReload, ammo);
		
		ammo -= ammoToReload;
		currentammo += ammoToReload;
		canaim = false;
		yield return new WaitForSeconds (anim [reloadAnim.name].length) ;
		myAudioSource.clip = readySound;
		myAudioSource.loop = false;
		myAudioSource.volume = 1;
		myAudioSource.Play();		
		anim.Play(ejectshellAnim.name);
		yield return new WaitForSeconds (anim [ejectshellAnim.name].length) ;

		isreloading = false;
		canaim = true;
		selector.canswitch = true;
	}
}

