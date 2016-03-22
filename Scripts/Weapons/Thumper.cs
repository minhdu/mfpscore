using UnityEngine;
using System.Collections;


public class Thumper : WeaponBehaviour {
	
	public Transform muzzleLeft;
	public Transform muzzleRight;
	public Transform clipShellLeft;
	public Transform clipShellRight;

	protected override void Reload () {
		if (!anim.isPlaying && canreload && !isreloading) {
			StartCoroutine(EjectShell(shellejectdelay)); 
			StartCoroutine(SetReload (anim[reloadAnim.name].length));
			myAudioSource.clip = reloadSound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play();		

			anim.Play(reloadAnim.name);
		}
	}

	protected override IEnumerator EjectShell (float waitTime) {
		yield return new WaitForSeconds(waitTime);
		Instantiate(shell, shellPos.transform.position,shellPos.transform.rotation);
	}

	protected override IEnumerator SetReload (float waitTime) {
		WeaponHandler selector = player.GetComponent<WeaponHandler>();
		selector.canswitch = false;
		canaim = false;
		isreloading = true;
		ammoToReload = Mathf.Clamp (ammoToReload, ammoToReload, ammo);
		yield return new WaitForSeconds (waitTime * 0.5f);
		ammo -= 1;
		currentammo += ammoToReload;
		yield return new WaitForSeconds (waitTime * 0.2f);
		ammo -= 1;
		currentammo += ammoToReload;
		yield return new WaitForSeconds (waitTime * 0.3f);

		isreloading = false;
		canaim = true;
		selector.canswitch = true;
	}

	protected override IEnumerator FlashMuzzle () {
		if (currentammo == 1) {	
			muzzleRight.transform.localEulerAngles = new Vector3 (0f, 0f, Random.Range (0f, 360f));
			muzzleRight.gameObject.SetActive (true);
			yield return new WaitForSeconds (0.05f);
			muzzleRight.gameObject.SetActive (false);
		} else {
			muzzleLeft.transform.localEulerAngles = new Vector3 (0f, 0f, Random.Range (0f, 360f));
			muzzleLeft.gameObject.SetActive (true);
			yield return new WaitForSeconds (0.05f);
			muzzleLeft.gameObject.SetActive (false);
		}
	}
}

