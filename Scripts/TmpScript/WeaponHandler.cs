﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class WeaponHandler : Singleton<WeaponHandler> {

	[SerializeField]
	int currentWeaponIndex = 2;
	int numWeapons = 0;

	public Transform[] Weapons;
	public float selectInterval = 2f;
	float nextselect = 2f;

	int previousWeaponIndex = 0;
	public AudioClip switchsound;
	public AudioSource myaudioSource;
	public bool canswitch = true;

	public bool hideweapons = false;
	bool oldhideweapons = false;
	public Text ammotext;

	public int currentammo = 10;
	public int totalammo = 100;

	genericShooter currentWeapon;
	public genericShooter CurrentWeapon {
		get {
			return currentWeapon;
		}
	}

	void Start () {
		currentWeapon = Weapons [currentWeaponIndex].GetComponent<genericShooter> ();
		numWeapons = Weapons.Length - 2;
	}

	public void NextWeapon () {
		string currentammostring = currentammo.ToString();
		string totalammostring = totalammo.ToString();
		ammotext.text = (currentammostring + " / " + totalammostring);
		if (Time.time > nextselect && canswitch) {
			nextselect = Time.time + selectInterval;
			if (currentWeaponIndex + 1 <= numWeapons)
			{ 
				previousWeaponIndex = currentWeaponIndex;
				currentWeaponIndex++;
			} else 
			{
				previousWeaponIndex = currentWeaponIndex;
				currentWeaponIndex = 2;
			}
			Debug.Log("Subtracted");
			myaudioSource.PlayOneShot(switchsound, 1);
			StartCoroutine(SelectWeapon(currentWeaponIndex));
			currentWeapon = Weapons [currentWeaponIndex].GetComponent<genericShooter> ();
		}

		if (hideweapons != oldhideweapons) {
			if (hideweapons) {
				StartCoroutine(HideWeapon(currentWeaponIndex));
			} else {
				StartCoroutine(UnhideWeapon(currentWeaponIndex));
			}
		}
	}

	IEnumerator HideWeapon (int index) {
		Weapons[index].gameObject.BroadcastMessage("doRetract",SendMessageOptions.DontRequireReceiver);
		yield return new WaitForSeconds (0.15f);
		Weapons[index].gameObject.SetActive(false);
		oldhideweapons = hideweapons;
	}

	IEnumerator UnhideWeapon (int index) { 
		yield return new WaitForSeconds (0.15f);
		Weapons[index].gameObject.SetActive(true);
		Weapons[index].gameObject.BroadcastMessage("doNormal",SendMessageOptions.DontRequireReceiver);
		oldhideweapons = hideweapons;
	}

	IEnumerator SelectWeapon (int index) {
		Weapons[previousWeaponIndex].gameObject.BroadcastMessage("doRetract",SendMessageOptions.DontRequireReceiver);
		yield return new WaitForSeconds (0.5f);
		Weapons[previousWeaponIndex].gameObject.SetActive(false);

		Weapons[index].gameObject.SetActive(true);
		Weapons[index].gameObject.BroadcastMessage("doNormal",SendMessageOptions.DontRequireReceiver);
	}

	public void DoShoot () {
		if (currentWeapon != null) {
			currentWeapon.DoShoot ();
		}
	}

	public void StopShoot () {
		if (currentWeapon != null) {
			currentWeapon.StopShoot ();
		}
	}

	public void DoReload () {
		if (currentWeapon != null) {
			currentWeapon.DoReload ();
		}
	}

	public void DoAim () {
		if (currentWeapon != null) {
			currentWeapon.DoAim ();
		}
	}
}
