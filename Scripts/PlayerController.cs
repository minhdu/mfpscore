using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : UISingleton<PlayerController> {
	
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

	[SerializeField]
	Weapon currentWeapon;
	public Weapon CurrentWeapon {
		get {
			return currentWeapon;
		}
	}

	int currentWeaponIndex = 0;

	Ray gunRay;
	RaycastHit gunHit;
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

	void Start () {
		worldSpaceCenter = new Vector3 (ScreenHelper.HalfScreenSize.x, ScreenHelper.HalfScreenSize.y, 0);
	}

	void Update () {
		gunRay = FPSCamera.Instance.PlayerCamera.ScreenPointToRay(worldSpaceCenter);
		Physics.Raycast(gunRay, out gunHit, 1000, collisionLayers.value);

		if (isShooting) {
			Shoot ();
		}

		if(currentWeapon.isFirearms && currentWeapon.remainBullet > 0 && currentWeapon.remainBulletInClip <= 0) {
			DoReload ();
		}
	}

	void Shoot ()
	{
		if (currentWeapon.isFirearms)
		{
			if (Time.time > currentWeapon.nextFireTime + currentWeapon.fireRate)
			{
				currentWeapon.remainBulletInClip -=1;

				//Apply Slide Effect
				FPSCamera.Instance.zSmooth = currentWeapon.zSmooth;
				FPSCamera.Instance.yPosition = currentWeapon.yPosition;
				FPSCamera.Instance.zPosition = -0.1f;

				//sight for a collider
				if (gunHit.collider)
				{
					//if(hit.collider.tag != "Spwan")
					//{
					gunHit.collider.SendMessage("Hit",currentWeapon.weaponPower,SendMessageOptions.DontRequireReceiver);

					if(gunHit.collider.gameObject.GetComponent<HitEnemy>())
					{
						gunHit.collider.gameObject.GetComponent<HitEnemy>().Hurt(currentWeapon.baseDamage);

						//Instantiate  (BloodZombie, hit.point, Quaternion.identity); 
					}
					else
					{

						Instantiate  (sparkle, gunHit.point, Quaternion.identity); 
					}
					//}
				} 
				//UpdateGUI(GUIComponent.Bullet);
				if(currentWeapon.muzzleFlash)
					currentWeapon.muzzleFlash.enabled = true;
				//audio.PlayOneShot(CurrentWeapon.shootSound);
				if(!GetComponent<AudioSource>().isPlaying)
				{
					GetComponent<AudioSource>().PlayOneShot(currentWeapon.shootSound);
				}

				muzzleRotate +=90;
				currentWeapon.muzzleFlash.gameObject.transform.localRotation = Quaternion.AngleAxis(muzzleRotate, Vector3.forward);
				currentWeapon.nextFireTime = Time.time;

				//Instantiate BulletUp


			}else
			{
				if(currentWeapon.muzzleFlash)
					currentWeapon.muzzleFlash.enabled = false;
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
		//        crossHair.gameObject.SetActive(false);
		inClin = 7.5f;
		if (CurrentWeapon.remainBulletInClip < currentWeapon.bulletPerClip) {
			currentWeapon.remainBullet += (int)currentWeapon.remainBulletInClip;
			currentWeapon.remainBulletInClip = 0;

			if (currentWeapon.remainBullet > currentWeapon.bulletPerClip) {

				//audio.Stop();
				GetComponent<AudioSource> ().PlayOneShot (CurrentWeapon.audioReload);

				yield return new WaitForSeconds (currentWeapon.audioReload.length - currentWeapon.audioReload.length * (currentWeapon.reloadSpeed - 1));

				currentWeapon.remainBulletInClip = currentWeapon.bulletPerClip;
				currentWeapon.remainBullet -= CurrentWeapon.bulletPerClip;
			}
			else 
			{
				yield return new WaitForSeconds (2);
					currentWeapon.remainBulletInClip = currentWeapon.remainBullet;
					currentWeapon.remainBullet = 0;
			}

			currentWeapon.currentAmmoClip = (float)currentWeapon.remainBullet/(float)currentWeapon.bulletPerClip;
//		UpdateGUI(GUIComponent.Clip);
		}
		isReloading = false;

		//UpdateLabel ();

//		UpdateGUI(GUIComponent.Bullet);
		inClin = 0;
		//        crossHair.gameObject.SetActive(true);
	}

	IEnumerator ChangeWeapon()
	{
		//        crossHair.gameObject.SetActive(false);

		yield return new WaitForSeconds(0.5f);

		currentWeaponIndex++;

		currentWeapon.weaponPrefab.SetActive(false);

		if(currentWeaponIndex > weapons.Count - 1)
		{
			currentWeaponIndex = 0;
		}

		currentWeapon = weapons[currentWeaponIndex];

		currentWeapon.weaponPrefab.SetActive(true);

		//UpdateLabel();

		isChanging = false;

		inClin = 0;

		isReloading = false;

		//        crossHair.gameObject.SetActive(true);
	}

	public void DoSight()
	{
		//        crossHair.gameObject.SetActive(!crossHair.gameObject.activeSelf);
		isAiming = !isAiming;
	}

	public void DoChange()
	{
		//if(isChanging && !isReloading && WeaponList.Count > 1)
		if(!isChanging && !isReloading)
		{
			isAiming = false;
			isReloading = true;
			inClin = 15f;
			StartCoroutine(ChangeWeapon());
		}
	}

	public void DoReload()
	{
		//if(!isReloading && CurrentWeapon.bulletinMagasine < CurrentWeapon.bulletperClip)
		if(!isReloading)
		{
			isAiming = false;
			StartCoroutine (Reload ());
		}
	}

	public void DoShoot (bool shoot) {
		if (!isReloading && currentWeapon.remainBulletInClip > 0) {
			isShooting = shoot;
		}
	}

}
