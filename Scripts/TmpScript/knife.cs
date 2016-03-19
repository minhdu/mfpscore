using UnityEngine;
using System.Collections;

public class knife : MonoBehaviour {
	
	
	public Vector3 normalposition;
	public float speed = 2f;
	public Transform player;


	public AudioSource myAudioSource;
	
	
	public AudioSource fireAudioSource;
	
	public AudioClip[] fireSounds;
	
	public AudioClip readySound;

	public AnimationClip[] fireAnimsA;
	public AnimationClip[] fireAnimsB;
	public AnimationClip hideA;
	public AnimationClip hideB;
	public AnimationClip readytoA;
	public AnimationClip readytoB;
	public AnimationClip switchA;
	public AnimationClip switchB;

	private bool isA = true;
	public float fireAnimSpeed = 1.1f;
	public float inaccuracy = 0.02f;
	public float force  = 500f;
	public float damage = 50f;
	public float range = 2f;


	
	public Vector3 retractPos;

	
	private bool retract = false;	

	public Transform rayfirer;
	public Transform grenadethrower;
	
	
	
	
	void Start()
	{
		
		
		GetComponent<Animation>().Stop();
		onstart();
		
	}
	void Update () 
	{
		
		float step = speed * Time.deltaTime;

		weaponselector inventory = player.GetComponent<weaponselector>();
		inventory.currentammo = 0;
		inventory.totalammo = 0;

		
	

		if (retract) {

			
			transform.localPosition = Vector3.MoveTowards (transform.localPosition, retractPos, 5 * Time.deltaTime);
			
		}
		else 
		{
			
			
			
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);
		}
		
		
		
		if (Input.GetButton("ThrowGrenade") && !GetComponent<Animation>().isPlaying)
		{
			StartCoroutine(setThrowGrenade());
		}

		if (Input.GetButton("Aim") || 	Input.GetAxis("Aim") > 0.1)
		{
			doswitch();
		}
		if (Input.GetButton("Fire1") || Input.GetAxis ("Fire1")>0.1 )
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
		
		GetComponent<Animation> ().Play (readytoA.name);
		isA = true;

		
		
	}
	
	void fire()
	{
		
		
		if (!GetComponent<Animation>().isPlaying && isA)
		{

			fireAudioSource.clip = fireSounds[Random.Range(0,fireSounds.Length)];
			fireAudioSource.pitch = 0.98f + 0.1f *Random.value;
			fireAudioSource.Play();
			GetComponent<Animation>().clip = fireAnimsA[Random.Range(0,fireAnimsA.Length)];
			GetComponent<Animation>().Play();  
			StartCoroutine(firedelayed(0.3f));
			
		}
		else if (!GetComponent<Animation>().isPlaying)
		{
			fireAudioSource.clip = fireSounds[Random.Range(0,fireSounds.Length)];
			fireAudioSource.pitch = 0.98f + 0.1f *Random.value;
			fireAudioSource.Play();
			GetComponent<Animation>().clip = fireAnimsB[Random.Range(0,fireAnimsB.Length)];
			GetComponent<Animation>().Play();  
			StartCoroutine(firedelayed(0.3f));
		}
		
		
	}
	

	
	void doRetract()
	{
		if( isA)

		{
			GetComponent<Animation>().Play(hideA.name);
		}
		else
		{
			GetComponent<Animation>().Play(hideB.name);
		}
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
	void doswitch()
	{
		if (isA && !GetComponent<Animation>().isPlaying )
		{
			GetComponent<Animation>().clip = switchB;
			GetComponent<Animation>().Play();  
			myAudioSource.clip = readySound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play ();
			isA = false;
		}
		else if (!GetComponent<Animation>().isPlaying)
		{
			GetComponent<Animation>().clip = switchA;
			GetComponent<Animation>().Play();
			myAudioSource.clip = readySound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play ();
			isA = true;
		}
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
