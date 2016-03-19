using UnityEngine;
using System.Collections;

public class Tester : MonoBehaviour {

	genericShooter shooter;
	WeaponHandler wpSelector;

	// Use this for initialization
	void UpDateData () {
		shooter = FindObjectOfType<genericShooter> ();
		wpSelector = FindObjectOfType<WeaponHandler> ();
	}
	
	public void DoShot (bool shot) {
		UpDateData ();
		//shooter.DoShot(shot);
	}

	public void DoAim () {
		UpDateData ();
		//shooter.DoAim();
	}

	public void DoReload () {
		UpDateData ();
		//shooter.DoReload ();
	}

	public void NextWeapon (bool sel) {
		UpDateData ();
		//wpSelector.SelectNextWeapon (sel);
	}
	public void PreviousWeapon (bool sel) {
		UpDateData ();
		//wpSelector.SelectPreviousWeapon (sel);
	}
	public void ThrowGrenade (bool t) {
		UpDateData ();
		//shooter.DoThrowGrenade (t);
	}
}
