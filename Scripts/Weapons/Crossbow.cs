using UnityEngine;
using System.Collections;


public class Crossbow : WeaponBehaviour {
	
	public Transform arrow;
	public Transform projectile;
	public Transform projectilePos;

	protected override IEnumerator SetReload (float waitTime) {
		WeaponHandler inventory = player.GetComponent<WeaponHandler>();

		isreloading = true;
		inventory.canswitch = false;
		canaim = false;
		yield return new WaitForSeconds (waitTime * 0.6f);
		arrow.gameObject.SetActive (true);
		yield return new WaitForSeconds (waitTime * 0.4f);
		isreloading = false;
		canaim = true;
		inventory.canswitch = true;
		
	}
}

