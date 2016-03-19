using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class weaponselector : MonoBehaviour {
	public int currentWeapon = 0;
	public int numWeapons = 0;

	public Transform[] Weapons;
	public float selectInterval = 2f;
	private float nextselect = 2f;

	private int previousWeapon = 0;
	public  AudioClip switchsound;
	public AudioSource myaudioSource;
	public bool canswitch;

	public bool hideweapons = false;
	bool oldhideweapons = false;
	public Text ammotext;

	public int currentammo = 10;
	public int totalammo = 100;



	// Use this for initialization
	void Start () 
	{
		currentWeapon = 0;
		canswitch = true;
	
	}
	
	// Update is called once per frame
	void Update () 


	{

		string currentammostring = currentammo.ToString();
		string totalammostring = totalammo.ToString();
		ammotext.text = (currentammostring + " / " + totalammostring);

		/*
		if(Input.GetAxis("CycleWeapons")>0 && Time.time > nextselect && canswitch 
		   || 
		   (Input.GetButtonDown ("CycleWeapons") && Time.time > nextselect && canswitch && !hideweapons))

		{
			nextselect = Time.time + selectInterval;
			if (currentWeapon + 1 <= numWeapons)
			{ 
				previousWeapon = currentWeapon;
				currentWeapon++;
			} else 
			{
				previousWeapon = currentWeapon;
				currentWeapon = 0;
			}
			Debug.Log("Subtracted");
			myaudioSource.PlayOneShot(switchsound, 1);
			StartCoroutine(selectWeapon(currentWeapon));

			// ================Previous Weapon========================
		}
		else if(Input.GetAxis("CycleWeapons")<0 && Time.time > nextselect && canswitch && !hideweapons)
		{
			nextselect = Time.time + selectInterval;
			if (currentWeapon - 1 >= 0)
			{ 
				previousWeapon = currentWeapon;
				currentWeapon--;
			}
			else 
			{
				previousWeapon = currentWeapon;
				currentWeapon = numWeapons;
			}
			myaudioSource.PlayOneShot(switchsound, 1);
			Debug.Log("Added");
			StartCoroutine(selectWeapon(currentWeapon));
		}
		if (hideweapons!= oldhideweapons)
		{

			if(hideweapons)
			{

				StartCoroutine(hidecurrentWeapon(currentWeapon));
			}
			else
			{
				StartCoroutine(unhidecurrentWeapon(currentWeapon));
			}



		}
	*/
	}
	IEnumerator hidecurrentWeapon(int index)
	{

		Weapons[index].gameObject.BroadcastMessage("doRetract",SendMessageOptions.DontRequireReceiver);
		yield return new WaitForSeconds (0.15f);
		Weapons[index].gameObject.SetActive(false);
		oldhideweapons = hideweapons;

			
		
	}
	IEnumerator unhidecurrentWeapon(int index)
	{



		yield return new WaitForSeconds (0.15f);
		Weapons[index].gameObject.SetActive(true);
		Weapons[index].gameObject.BroadcastMessage("doNormal",SendMessageOptions.DontRequireReceiver);
		oldhideweapons = hideweapons;
		
	}
	IEnumerator selectWeapon(int index)
	{
		Weapons[previousWeapon].gameObject.BroadcastMessage("doRetract",SendMessageOptions.DontRequireReceiver);
		yield return new WaitForSeconds (0.5f);
		Weapons[previousWeapon].gameObject.SetActive(false);


		Weapons[index].gameObject.SetActive(true);
		Weapons[index].gameObject.BroadcastMessage("doNormal",SendMessageOptions.DontRequireReceiver);

		
		
		
	}

}
