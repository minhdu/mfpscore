using UnityEngine;
using System.Collections;


public class shotgun : WeaponBehaviour {

	public Transform handshell;
	

	public AudioClip pumpSound;
	public AudioClip reloadonceSound;
	public AudioClip reloadlastSound;

	public AnimationClip toreloadAnim;
	public AnimationClip reloadonceAnim;
	public AnimationClip reloadlastAnim;
	public AnimationClip pumpAnim;
	
	protected override void OnStart() {
		myAudioSource.Stop();
		fireAudioSource.Stop();
		handshell.gameObject.SetActive (false);

		anim = GetComponent<Animation> ();

		raycastfire weaponfirer = rayfirer.GetComponent<raycastfire>();
		weaponfirer.inaccuracy = inaccuracy;
		weaponfirer.damage = damage;
		weaponfirer.range = range;
		weaponfirer.force = force;
		weaponfirer.projectilecount = projectilecount;
		
		GetComponent<Animation>().Stop();
		if (isreloading) {
			Reload ();
		} 
		else {
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
		if (!GetComponent<Animation>().isPlaying) {
			float randomZ = Random.Range (-0.05f,-0.01f);
			//float randomY = Random.Range (-0.1f,0.1f);

			trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, trans.localPosition.z + randomZ);
			FPSCamera.Instance.DoRecoil(recoil);
			StartCoroutine(setfire());

			if (currentammo <= 0) {
				Reload();
			}
		}
		
		
	}
	
	protected override void Reload() {
		if (!GetComponent<Animation>().isPlaying && canreload && !isreloading ) {
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
		//PlayerControllerPC controller = player.GetComponent<PlayerControllerPC>();
		//controller.canclimb = false;
		
		//GetComponent<Animation>()[reloadAnim.name].time = startTime;
		GetComponent<Animation> ().Play(toreloadAnim.name);
		yield return new WaitForSeconds (GetComponent<Animation> () [toreloadAnim.name].length );

		while(currentammo != ammoToReload)
		{
			GetComponent<Animation> ().Play (reloadonceAnim.name);
			myAudioSource.clip = reloadonceSound;
			myAudioSource.Play ();
			
			//GetComponent<Animation>()[reloadAnim.name].time = startTime;
			handshell.gameObject.SetActive (true);
			yield return new WaitForSeconds (GetComponent<Animation> () [reloadonceAnim.name].length * 0.4f);
			ammo -= 1;
			handshell.gameObject.SetActive (false);
			currentammo += 1;
			
			yield return new WaitForSeconds (GetComponent<Animation> () [reloadonceAnim.name].length * 0.6f);
			
		} 
		
		//reloadlast
		myAudioSource.clip = reloadlastSound;
		myAudioSource.Play();
		//GetComponent<Animation>()[reloadAnim.name].time = startTime;
		GetComponent<Animation> ().Play(reloadlastAnim.name);
		StartCoroutine(EjectShell(shellejectdelay *1.5f));
		yield return new WaitForSeconds (GetComponent<Animation> () [reloadlastAnim.name].length);
		
//		controller.canclimb = true;
		isreloading = false;
		canaim = true;
		inventoryscript.canswitch = true;
		
		
		
	}

	IEnumerator setfire() {
		if (currentammo > 1) {
			StartCoroutine(FlashMuzzle());
			raycastfire weaponfirer = rayfirer.GetComponent<raycastfire> ();
			weaponfirer.SendMessage ("fire", SendMessageOptions.DontRequireReceiver);
			
			fireAudioSource.clip = fireSound;
			fireAudioSource.pitch = 0.9f + 0.1f * Random.value;
			fireAudioSource.Play ();
			
			GetComponent<Animation> ().Play (fireAnim.name);
			currentammo -= 1;
			yield return new WaitForSeconds (GetComponent<Animation> () [fireAnim.name].length);
			myAudioSource.clip = pumpSound;
			myAudioSource.Play();
			
			
			GetComponent<Animation> ().Play(pumpAnim.name);
			
			StartCoroutine (EjectShell (shellejectdelay));
			yield return new WaitForSeconds (GetComponent<Animation> () [pumpAnim.name].length);
			
			
		} 
		else if (currentammo <= 1) {
			if (currentammo <= 0) {
				Reload ();
			}
			StartCoroutine(FlashMuzzle());
			raycastfire weaponfirer = rayfirer.GetComponent<raycastfire> ();
			weaponfirer.SendMessage ("fire", SendMessageOptions.DontRequireReceiver);
			
			fireAudioSource.clip = fireSound;
			fireAudioSource.pitch = 0.9f + 0.1f * Random.value;
			fireAudioSource.Play ();
			
			GetComponent<Animation> ().Play (fireAnim.name);
			
			currentammo -= 1;
			yield return new WaitForSeconds (GetComponent<Animation> () [fireAnim.name].length);
		}
	}
}

