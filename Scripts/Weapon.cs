using UnityEngine;
using System;

[Serializable]
public class Weapon {
	#region General
	public string weaponName;
	public bool isFirearms;
	public int weaponPower;
	public int baseDamage = 40;
	public GameObject weaponPrefab;
	public float yPosition = 0.4f;
	#endregion

	#region For Gun
	public float fireRate;
	public float shootRange;
	public float spreadRange;
	public int bulletBallNum = 1;
	public float gunAccurary;
	public int bulletPerClip; 
	public float reloadSpeed;
	public float swayAmount;
	public float shakeAmount;
	public float zoomAmout = 20;


	public GameObject sniperScope;
	public Renderer muzzleFlash;
	public AudioClip shootSound;
	public AudioClip audioReload;
	public GameObject bloodPrefab;
	public GameObject bulletHole;
	public Renderer projectileRender;
	public GameObject crossHair;


	public float zSmooth = 10;

	public int maxAmmoClip;
	public float currentAmmoClip;
	public int remainBullet;
	public float nextFireTime;
	public int remainBulletInClip;
	public float aimAngleX = 0f;
	public float aimAngleY = 0f;
	public float aimAngleZ = 0f;
	public Vector3 aimPosition;

	Transform _muzzleFlashTransform;
	public Transform MuzzleFlashTransform {
		get {
			if (_muzzleFlashTransform == null)
				_muzzleFlashTransform = muzzleFlash.GetComponent<Transform> ();
			return _muzzleFlashTransform;
		}
	}

	Animation _anim;
	public Animation Anim {
		get {
			if (_anim == null)
				_anim = weaponPrefab.GetComponent<Animation> ();
			return _anim;
		}
	}

	#endregion

	#region Serialization
	public void Deserialize (string json) {
		JsonUtility.FromJsonOverwrite(json, this);
	}

	public string Serialize () {
		return JsonUtility.ToJson (this);
	}
	#endregion
}
