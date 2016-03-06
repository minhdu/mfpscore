using UnityEngine;
using System;

[Serializable]
public class Weapon : IGameModel
{
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
	public float spreadFactor;
	public int bulletPerClip;
	public float reloadSpeed;
	public Renderer muzzleFlash;
	public AudioClip shootSound;
	public AudioClip audioReload;
	public GameObject bloodPrefab;
	public GameObject bulletHole;
	public Renderer projectileRender;

	public float zSmooth = 10;

	public int maxAmmoClip;
	public float currentAmmoClip;
	public int remainBullet;
	public float nextFireTime;
	public int remainBulletInClip;
	public float aimAngle = 358.5694f;
	public Vector3 aimPosition = new Vector3
	{
		x = -0.1615f,
		y = -0.044f,
		z = -0.45f
	};

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
