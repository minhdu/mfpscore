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
		MELEE
	}

	string[] animationsName = new string[] {
		"Idle",
		"Shoot",
		"Reload",
		"Hide",
		"Show",
		"ZoomIn",
		"ZoomOut",
		"Melee"
	};


	public LayerMask collisionLayers = -1;
	public GameObject sparkle;
	public List<Weapon> weapons = new List<Weapon>();
	public GameObject crossHair;

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

	Ray bulletRay;
	RaycastHit bulletHit;
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

	bool isZoomingIn = false;
	bool isZoomingOut = false;

	void Start () {
		worldSpaceCenter = new Vector3 (ScreenHelper.HalfScreenSize.x, ScreenHelper.HalfScreenSize.y, 0);
		currentWeapon = weapons [0];
	}

	void Update () {

		// Shooting ray
		float spreadRange = currentWeapon.spreadRange - currentWeapon.spreadRange * currentWeapon.gunAccurary;
		float spreadRangeX = Random.Range (-spreadRange, spreadRange);
		float spreadRangeY = Random.Range (-spreadRange, spreadRange);
		bulletRay = FPSCamera.Instance.PlayerCamera.ScreenPointToRay(worldSpaceCenter+new Vector3(spreadRangeX, spreadRangeY, 0));
		Physics.Raycast(bulletRay, out bulletHit, currentWeapon.shootRange, collisionLayers.value);

		if (isShooting) {
			Shoot ();
			CrossHair.Instance.Zoom ();
		}

		if(currentWeapon.isFirearms && currentWeapon.remainBullet > 0 && currentWeapon.remainBulletInClip <= 0) {
			DoReload ();
		}

		if (!isReloading && !isShooting && !isChanging && !isAiming && !isZoomingIn && !isZoomingOut) {
			PlayAnimation (WeaponAnimation.IDLE, WrapMode.Loop);
		}
	}

#region Action Processor

	void Shoot ()
	{
		if (currentWeapon.isFirearms)
		{
			if (Time.time > currentWeapon.nextFireTime + currentWeapon.fireRate)
			{
				PlayAnimation (WeaponAnimation.SHOOT);

				// Projectile
//				Rigidbody clone = Instantiate(currentWeapon.projectilePrefab, Transform.position, Transform.rotation) as Rigidbody;
//				clone.velocity = Transform.TransformDirection(Vector3.forward * 200);

				currentWeapon.remainBulletInClip -=1;

				// Apply slide effect
				FPSCamera.Instance.zSmooth = currentWeapon.zSmooth;
				FPSCamera.Instance.yPosition = currentWeapon.yPosition;
				FPSCamera.Instance.zPosition = -0.1f;

				// Sight for a collider
				if (bulletHit.collider)
				{
					//if(hit.collider.tag != "Spwan")
					//{
					bulletHit.collider.SendMessage("Hit",currentWeapon.weaponPower,SendMessageOptions.DontRequireReceiver);

					HitEnemy enemy = bulletHit.collider.gameObject.GetComponent<HitEnemy> ();
					// Hit enemy
					if(enemy != null) {
						bulletHit.collider.gameObject.GetComponent<HitEnemy>().Hurt(currentWeapon.baseDamage);
						HitMark.Instance.Hit ();
						//Instantiate  (BloodZombie, hit.point, Quaternion.identity); 
					}
					// Hit environment
					else {
						// Sparkle
						Instantiate  (sparkle, bulletHit.point, Quaternion.identity); 

						// Hole
						Quaternion hitRotation = Quaternion.FromToRotation(Vector3.forward, bulletHit.normal);
						Instantiate(currentWeapon.bulletHole, bulletHit.point+bulletHit.normal*0.1f, hitRotation);
					}
					//}
				} 
				//UpdateGUI(GUIComponent.Bullet);
				if(currentWeapon.muzzleFlash)
					currentWeapon.muzzleFlash.enabled = true;
//				if (currentWeapon.projectileRender)
//					currentWeapon.projectileRender.enabled = true;
//				
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
	}

	IEnumerator Reload()
	{
		isReloading = true;
		crossHair.gameObject.SetActive(false);
		//inClin = 7.5f;
		if (CurrentWeapon.remainBulletInClip < currentWeapon.bulletPerClip) {
			currentWeapon.remainBullet += currentWeapon.remainBulletInClip;
			currentWeapon.remainBulletInClip = 0;

			if (currentWeapon.remainBullet > currentWeapon.bulletPerClip) {

				//audio.Stop();

				PlayAnimation (WeaponAnimation.RELOAD);

				AudioSource.PlayOneShot (CurrentWeapon.audioReload);

				yield return new WaitForSeconds (PlayAnimation (WeaponAnimation.RELOAD));
				//yield return new WaitForSeconds (currentWeapon.audioReload.length - currentWeapon.audioReload.length * (currentWeapon.reloadSpeed - 1));

				currentWeapon.remainBulletInClip = currentWeapon.bulletPerClip;
				currentWeapon.remainBullet -= CurrentWeapon.bulletPerClip;
			}
			else 
			{
					//yield return new WaitForSeconds (2);
					currentWeapon.remainBulletInClip = currentWeapon.remainBullet;
					currentWeapon.remainBullet = 0;
			}

			currentWeapon.currentAmmoClip = (float)currentWeapon.remainBullet/(float)currentWeapon.bulletPerClip;
//			UpdateGUI(GUIComponent.Clip);
		}
		isReloading = false;

		//UpdateLabel ();

//		UpdateGUI(GUIComponent.Bullet);
//		inClin = 0;
      crossHair.gameObject.SetActive(true);
	}

	IEnumerator ChangeWeapon()
	{
		crossHair.gameObject.SetActive(false);

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

		crossHair.gameObject.SetActive(true);
	}

	IEnumerator Aim () {
		if (isAiming) {
			isZoomingIn = true;
			yield return new WaitForSeconds(PlayAnimation(WeaponAnimation.ZOOM_IN));
			isZoomingIn = false;
		} else {
			isZoomingOut = true;
			yield return new WaitForSeconds (PlayAnimation (WeaponAnimation.ZOOM_OUT));
			isZoomingOut = false;
		}
	}

	public float PlayAnimation (WeaponAnimation anim, WrapMode wrapMode = WrapMode.Default) {
		string clipName = animationsName [(int)anim];
		if (wrapMode == WrapMode.Loop && currentWeapon.Anim.IsPlaying (clipName))
			return 0;
		AnimationClip clip = currentWeapon.Anim.GetClip (clipName);
		if (clip != null) {
			currentWeapon.Anim.clip = clip;
			currentWeapon.Anim.wrapMode = wrapMode;
			currentWeapon.Anim.Play();
			return clip.length;
		}

		return 0;
	}

#endregion

#region Input

	public void DoSight()
	{
		crossHair.gameObject.SetActive(!crossHair.gameObject.activeSelf);
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
			isAiming = false;
			StartCoroutine (Reload ());
		}
	}

	public void DoShoot (bool shoot) {
		if (!isReloading && currentWeapon.remainBulletInClip > 0 && !isZoomingIn && !isZoomingOut) {
			isShooting = shoot;
			if (!isShooting) {
				CrossHair.Instance.Reverts();
				if(currentWeapon.muzzleFlash.enabled)
					currentWeapon.muzzleFlash.enabled = false;
			}
		}
	}

	public Animation grenade;
	public void DoThrowGrenade () {
		grenade.Play ();
	}

#endregion
}
