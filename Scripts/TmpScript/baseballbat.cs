using UnityEngine;
using System.Collections;

public class baseballbat : MonoBehaviour {
	
	
	public Vector3 normalposition;
	public float speed = 2f;
	public Transform player;
	public Transform weaponRoot;
	public bool twohanded;
	public AudioSource myAudioSource;
	
	
	public AudioSource fireAudioSource;
	
	public AudioClip[] fireSounds;
	
	public AudioClip readySound;
	
	public AnimationClip[] fireAnimsA;



	public float fireAnimSpeed = 1.1f;
	public float inaccuracy = 0.02f;
	public float force  = 500f;
	public float damage = 50f;
	public float range = 2f;
	public AnimationClip readyAnim;
	public AnimationClip hideAnim;
	
	public Vector3 retractPos;
	
	
	private bool retract = false;	
	private bool canfire = true;
	public Transform rayfirer;
	public Transform grenadethrower;
	public float runXrotation = 20;


	
	private Vector3 wantedrotation;
	
	
	
	void Start()
	{
		
		
		GetComponent<Animation>().Stop();
		onstart();
		
	}
	void Update () 
	{
		float step = speed * Time.deltaTime;
		





		WeaponHandler inventory = player.GetComponent<WeaponHandler>();
		inventory.currentammo = 0;
		inventory.totalammo = 0;

				

		if (retract)
		{
			canfire = false;

			transform.localPosition = Vector3.MoveTowards(transform.localPosition, retractPos, 5 * Time.deltaTime);

		}
		else 
		{

			transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);
		}
		
		
		
		if (Input.GetButton("ThrowGrenade") && !GetComponent<Animation>().isPlaying)
		{
			StartCoroutine(setThrowGrenade());
		}

	
		if (Input.GetButton("Fire1") && canfire || Input.GetAxis ("Fire1")>0.1 && canfire )
		{
			
			fire();
			
		}
		
		
	}
	
	
	
	void onstart()
	{
		myAudioSource.Stop();
		fireAudioSource.Stop();
		retract = false;

		
		GetComponent<Animation>().Stop();
		raycastfire weaponfirer = rayfirer.GetComponent<raycastfire>();
		weaponfirer.inaccuracy = inaccuracy;
		weaponfirer.damage = damage;
		weaponfirer.range = range;
		weaponfirer.force = force;
		weaponfirer.projectilecount = 1;
		
		myAudioSource.clip = readySound;
		myAudioSource.loop = false;
		myAudioSource.volume = 1;
		myAudioSource.Play ();
		
		GetComponent<Animation> ().Play (readyAnim.name);

		canfire = true;
		
		
	}
	
	void fire()
	{
		
		
		if (!GetComponent<Animation>().isPlaying )
		{
			
			fireAudioSource.clip = fireSounds[Random.Range(0,fireSounds.Length)];
			fireAudioSource.pitch = 0.98f + 0.1f *Random.value;
			fireAudioSource.Play();
			GetComponent<Animation>().clip = fireAnimsA[Random.Range(0,fireAnimsA.Length)];
			GetComponent<Animation>().Play();  
			StartCoroutine(firedelayed(0.3f));
			
		}

		
		
	}
	
	
	
	void doRetract()
	{
		GetComponent<Animation>().Play(hideAnim.name);
	}
	void doNormal()
	{
		
		retract = false;
		onstart();
	}
	IEnumerator firedelayed(float waitTime)
	{
		yield return new WaitForSeconds (waitTime);
		raycastfire weaponfirer = rayfirer.GetComponent<raycastfire>();
		weaponfirer.SendMessage("fireMelee",SendMessageOptions.DontRequireReceiver);
	}

	IEnumerator setThrowGrenade()
	{
		retract = true;
		grenadethrower.gameObject.SetActive(true);
		grenadethrower.gameObject.BroadcastMessage("throwstuff");
		yield return new WaitForSeconds(grenadethrower.GetComponent<Animation>()["throwAnim"].length);
		retract = false;
		
		grenadethrower.gameObject.SetActive(false);
	}
	
}

