using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : Singleton<PlayerController> {

	public enum WeaponAnimation {
		IDLE   = 0,
		SHOOT,
		RELOAD,
		HIDE,
		SHOW,
		ZOOM_IN,
		ZOOM_OUT,
		MELEE,
		RELOAD_START,
		RELOAD_STOP
	}

	string[] animationsName = new string[] {
		"Idle",
		"Shoot",
		"Reload",
		"Hide",
		"Show",
		"ZoomIn",
		"ZoomOut",
		"Melee",
		"ReloadStart",
		"ReloadStop"
	};



	public Animation grenadeAnim;
	public LayerMask collisionLayers = -1;
	public GameObject sparkle;
	public List<Weapon> weapons = new List<Weapon>();

	Transform _transform;
	public Transform Transform {
		get {
			if (_transform == null)
				_transform = GetComponent<Transform> ();
			return _transform;
		}
	}

	AudioSource _audioSource;
	public AudioSource AudioSource {
		get {
			if (_audioSource == null)
				_audioSource = GetComponent<AudioSource> ();
			return _audioSource;
		}
	}
		
	Weapon currentWeapon;
	public Weapon CurrentWeapon {
		get {
			return currentWeapon;
		}
	}

	int currentWeaponIndex = 0;

	Ray[] bulletRay = new Ray[10];
	RaycastHit[] bulletHit = new RaycastHit[10];
	int muzzleRotate = 45;
	Vector3 worldSpaceCenter = Vector3.zero;

	bool isShooting = false;
	public bool IsShooting {
		get {
			return isShooting;
		}
	}

	bool isReloading = false;
	public bool IsReloading {
		get {
			return isReloading;
		}
	}

	bool isAiming = false;
	public bool IsAiming {
		get {
			return isAiming;
		}
	}

	bool isChanging = false;
	public bool IsChange {
		get {
			return isChanging;
		}
	}

	float inClin;
	public float InClin {
		get {
			return inClin;
		}
	}

	bool delayShoot = false;

	bool isZoomingIn = false;
	bool isZoomingOut = false;

	void Start () {
		worldSpaceCenter = new Vector3 (ScreenHelper.HalfScreenSize.x, ScreenHelper.HalfScreenSize.y, 0);
		//currentWeapon = weapons [0];
		CameraShake.Instance.Idle ();
	}

	void Update () {

		// Shooting ray
//		for (int i = 0; i < currentWeapon.bulletBallNum; i++) {
//			float spreadRange = currentWeapon.spreadRange - currentWeapon.spreadRange * currentWeapon.gunAccurary;
//			float spreadRangeX = Random.Range (-spreadRange, spreadRange);
//			float spreadRangeY = Random.Range (-spreadRange, spreadRange);
//			bulletRay[i] = FPSCamera.Instance.PlayerCamera.ScreenPointToRay (worldSpaceCenter + new Vector3 (spreadRangeX, spreadRangeY, 0));
//			Physics.Raycast (bulletRay[i], out bulletHit[i], currentWeapon.shootRange, collisionLayers.value);
//		}
//			
//		if (isShooting && !delayShoot) {
//			StartCoroutine(Shoot ());
//			CrossHair.Instance.Zoom ();
//		}
//
//		if(currentWeapon.isFirearms && currentWeapon.remainBullet > 0 && currentWeapon.remainBulletInClip <= 0) {
//			DoReload ();
//		}
//
//		if (!isReloading && !isShooting && !isChanging && !isAiming && !isZoomingIn && !isZoomingOut && !delayShoot) {
//			PlayAnimation (WeaponAnimation.IDLE, WrapMode.Loop);
//		}
	}

#region Action Processor

	IEnumerator Shoot ()
	{
		if (currentWeapon.isFirearms)
		{
			if (Time.time > currentWeapon.nextFireTime + currentWeapon.fireRate)
			{
				if (currentWeapon.bulletBallNum > 1)
					delayShoot = true;
				AudioSource.PlayOneShot(CurrentWeapon.shootSound);
				float shotTime = PlayAnimation (WeaponAnimation.SHOOT);

				// Projectile
//				Rigidbody clone = Instantiate(currentWeapon.projectilePrefab, Transform.position, Transform.rotation) as Rigidbody;
//				clone.velocity = Transform.TransformDirection(Vector3.forward * 200);

				currentWeapon.remainBulletInClip -=1;

				// Apply slide effect
				FPSCamera.Instance.zSmooth = currentWeapon.zSmooth;
				FPSCamera.Instance.yPosition = currentWeapon.yPosition;
				FPSCamera.Instance.zPosition = -0.1f;

				for (int i = 0; i < currentWeapon.bulletBallNum; i++) {
					// Sight for a collider
					if (bulletHit [i].collider) {
						//if(hit.collider.tag != "Spwan")
						//{
						bulletHit [i].collider.SendMessage ("Hit", currentWeapon.weaponPower, SendMessageOptions.DontRequireReceiver);

						FakeEnemy enemy = bulletHit [i].collider.gameObject.GetComponent<FakeEnemy> ();
						// Hit enemy
						if (enemy != null) {
							bulletHit [i].collider.gameObject.GetComponent<FakeEnemy> ().Hurt (currentWeapon.baseDamage);
							HitMark.Instance.Hit ();
							Instantiate (currentWeapon.bloodPrefab, bulletHit [i].point, Quaternion.identity); 
						}
					// Hit environment
					else {
							// Sparkle
							Instantiate (sparkle, bulletHit [i].point, Quaternion.identity); 

							// Hole
							Quaternion hitRotation = Quaternion.FromToRotation (Vector3.forward, bulletHit [i].normal);
							Instantiate (currentWeapon.bulletHole, bulletHit [i].point + bulletHit [i].normal * 0.1f, hitRotation);
						}
						//}
					}
				}

				if (currentWeapon.bulletBallNum > 1) {
					yield return new WaitForSeconds (shotTime);
					delayShoot = false;
				}
				
				//UpdateGUI(GUIComponent.Bullet);
				if(currentWeapon.muzzleFlash)
					currentWeapon.muzzleFlash.enabled = true;
//				if (currentWeapon.projectileRender)
//					currentWeapon.projectileRender.enabled = true;
//				
				if(currentWeapon.bulletBallNum == 1)
					AudioSource.PlayOneShot(CurrentWeapon.shootSound);
//				if(!AudioSource.isPlaying)
//				{
//					AudioSource.PlayOneShot(currentWeapon.shootSound);
//				}

				//muzzleRotate +=Random.Range(45,90);
				muzzleRotate += 90;

				//currentWeapon.MuzzleFlashTransform.localRotation = Quaternion.AngleAxis(muzzleRotate, Vector3.forward);
				Vector3 muzzleAngle = currentWeapon.MuzzleFlashTransform.localRotation.eulerAngles;
				currentWeapon.MuzzleFlashTransform.localRotation = Quaternion.Euler (muzzleAngle.x, muzzleAngle.y, muzzleRotate);
				currentWeapon.nextFireTime = Time.time;

				//Instantiate BulletUp


			}else {
				if(currentWeapon.muzzleFlash)
					currentWeapon.muzzleFlash.enabled = false;
//				if (currentWeapon.projectileRender)
//					currentWeapon.projectileRender.enabled = false;
				FPSCamera.Instance.zPosition = 0;
				FPSCamera.Instance.zSmooth = 8f;
			}

//			isShooting = false;
		}
		else
		{
			//Put here your code for no fire weapon			
		}

		//UpdateLabel ();
		yield break;
	}

	IEnumerator Reload()
	{
		isReloading = true;
		CrossHair.Instance.Show (false);
		//inClin = 7.5f;
		if (CurrentWeapon.remainBulletInClip < currentWeapon.bulletPerClip) {
			if (currentWeapon.bulletBallNum == 1) {
				currentWeapon.remainBullet += currentWeapon.remainBulletInClip;
				currentWeapon.remainBulletInClip = 0;
			} else {
				yield return new WaitForSeconds(PlayAnimation(WeaponAnimation.RELOAD_START));
			}

				//audio.Stop();

			int reloadTimes = 1;
			if (currentWeapon.bulletBallNum > 1)
				reloadTimes = currentWeapon.bulletPerClip - currentWeapon.remainBulletInClip;
			for (int i = 0; i < reloadTimes; i++) {
				AudioSource.PlayOneShot (CurrentWeapon.audioReload);
				yield return new WaitForSeconds (PlayAnimation (WeaponAnimation.RELOAD, WrapMode.Default, true));
				//yield return new WaitForSeconds (currentWeapon.audioReload.length - currentWeapon.audioReload.length * (currentWeapon.reloadSpeed - 1));

				if (currentWeapon.bulletBallNum == 1) {
					currentWeapon.remainBulletInClip = currentWeapon.bulletPerClip;
					currentWeapon.remainBullet -= CurrentWeapon.bulletPerClip;
					currentWeapon.currentAmmoClip = (float)currentWeapon.remainBullet / (float)currentWeapon.bulletPerClip;
				} else {
					currentWeapon.remainBulletInClip += 1;
					currentWeapon.remainBullet -= 1;
					currentWeapon.currentAmmoClip = (float)currentWeapon.remainBullet / (float)currentWeapon.bulletPerClip;
				}

			}
//			UpdateGUI(GUIComponent.Clip);

			if (currentWeapon.bulletBallNum > 1) {
				yield return new WaitForSeconds (PlayAnimation (WeaponAnimation.RELOAD_STOP));
			}

		}
		isReloading = false;

		//UpdateLabel ();

//		UpdateGUI(GUIComponent.Bullet);
//		inClin = 0;
		CrossHair.Instance.Show (true);
		CameraShake.Instance.Idle ();
	}

	IEnumerator ChangeWeapon()
	{
		CrossHair.Instance.Show (false);
		if (currentWeapon.sniperScope != null)
			currentWeapon.sniperScope.SetActive (false);

		isChanging = true;

		yield return new WaitForSeconds(PlayAnimation (WeaponAnimation.HIDE));

		currentWeaponIndex++;
		currentWeapon.weaponPrefab.SetActive(false);

		if(currentWeaponIndex > weapons.Count - 1)
		{
			currentWeaponIndex = 0;
		}

		currentWeapon = weapons[currentWeaponIndex];
		currentWeapon.weaponPrefab.SetActive(true);
		yield return new WaitForSeconds(PlayAnimation (WeaponAnimation.SHOW));

		//UpdateLabel();

		isChanging = false;

		//inClin = 0;

		CrossHair.Instance.Show (true);
		CameraShake.Instance.Idle ();
	}

	IEnumerator Aim () {
		if (isAiming) {
			isZoomingIn = true;
			yield return new WaitForSeconds(PlayAnimation(WeaponAnimation.ZOOM_IN));
			if (currentWeapon.sniperScope != null) {
				currentWeapon.sniperScope.SetActive (true);
				currentWeapon.weaponPrefab.SetActive (false);
			}
			isZoomingIn = false;
		} else {
			isZoomingOut = true;
			if (currentWeapon.sniperScope != null) {
				currentWeapon.sniperScope.SetActive (false);
				currentWeapon.weaponPrefab.SetActive (true);
			}
			yield return new WaitForSeconds (PlayAnimation (WeaponAnimation.ZOOM_OUT));
			isZoomingOut = false;
		}
		CameraShake.Instance.Idle ();
	}

	public float PlayAnimation (WeaponAnimation anim, WrapMode wrapMode = WrapMode.Default, bool crossFade=false, float fadeLenght=0.25f) {
		string clipName = animationsName [(int)anim];
		if (wrapMode == WrapMode.Loop && currentWeapon.Anim.IsPlaying (clipName))
			return 0;
		AnimationClip clip = currentWeapon.Anim.GetClip (clipName);
		if (clip != null) {
			if (crossFade) {
				currentWeapon.Anim.CrossFade (clipName, fadeLenght);
			} else {
				currentWeapon.Anim.clip = clip;
				currentWeapon.Anim.wrapMode = wrapMode;
				currentWeapon.Anim.Play ();
			}
			return clip.length;
		}

		return 0;
	}

	IEnumerator ThrowGrenade () {
		isChanging = true;
		yield return new WaitForSeconds (PlayAnimation (WeaponAnimation.HIDE));
		grenadeAnim.Play ();
		GrenadeHandler.Instance.Throw ();
		yield return new WaitForSeconds (2.5f);
		yield return new WaitForSeconds (PlayAnimation (WeaponAnimation.SHOW));
		isChanging = false;
	}

#endregion

#region Input

	public void DoSight()
	{
		CrossHair.Instance.Show (isAiming);
		isAiming = !isAiming;
		StartCoroutine (Aim ());
	}

	public void DoChange()
	{
		//if(isChanging && !isReloading && WeaponList.Count > 1)
		if(!isChanging && !isReloading && !isZoomingIn && !isZoomingOut)
		{
			isAiming = false;
			//inClin = 15f;
			StartCoroutine(ChangeWeapon());
		}
	}

	public void DoReload()
	{
		//if(!isReloading && CurrentWeapon.bulletinMagasine < CurrentWeapon.bulletperClip)
		if(!isReloading && !isZoomingIn && !isZoomingOut)
		{
			isShooting = false;
			isAiming = false;
			CameraShake.Instance.StopShootingShake ();
			if (currentWeapon.sniperScope != null) {
				currentWeapon.sniperScope.SetActive (false);
				currentWeapon.weaponPrefab.SetActive (true);
			}
			StartCoroutine (Reload ());
		}
	}

	public void DoShoot (bool shoot) {
		if (!isReloading && currentWeapon.remainBulletInClip > 0 && !isZoomingIn && !isZoomingOut) {
			isShooting = shoot;
			if (shoot) {
				CameraShake.Instance.ShootingShake (currentWeapon.shakeAmount, currentWeapon.fireRate);
				CameraShake.Instance.StopIdle ();
			} else {
				CrossHair.Instance.Reverts();
				CameraShake.Instance.Idle ();
				CameraShake.Instance.StopShootingShake ();
				if(currentWeapon.muzzleFlash.enabled)
					currentWeapon.muzzleFlash.enabled = false;
			}
		}
	}

	public void DoThrowGrenade () {
		StartCoroutine (ThrowGrenade ());
	}

	public void Hurt (float damage) {
		
	}
#endregion
}
