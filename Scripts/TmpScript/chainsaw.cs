using UnityEngine;
using System.Collections;


public class chainsaw : MonoBehaviour {
	

	
	
	public Transform bladecollider;
	public Vector3 normalposition;
	public Vector3 retractPos;
	public float speed = 1f;
	public Transform player;


	public AudioSource myAudioSource;
	public Transform blade;
	
	public AudioSource fireAudioSource;

	public AudioClip fireSound;
	
	public AudioClip readySound;

	
	

	
	
	public float smoothdamping  = 2f;
	
	
	
	public AnimationClip fireAnim;
	public float fireAnimSpeed = 1.0f;
	public AnimationClip idleAnim;
	public float idleAnimSpeed = 0.2f;

	public AnimationClip readyAnim;
	
	public AnimationClip hideAnim;


	
	
	
	

	public Camera weaponcamera;
	

	
	

	private bool retract = false;	
	private bool canfire = true;

	

	public Transform grenadethrower;
	
	
	void Start()
	{
		
		
		GetComponent<Animation>().Stop();
		onstart();
		
	}
	void Update () 
	{

		weaponselector inventory = player.GetComponent<weaponselector>();
		inventory.currentammo = 0;
		inventory.totalammo = 0;
		float step = speed * Time.deltaTime;
		
		if (Input.GetButton("ThrowGrenade") && !GetComponent<Animation> ().IsPlaying (fireAnim.name))
		{
			StartCoroutine(setThrowGrenade());
		}
		
		
	
		
		if (retract)
		{
			canfire = false;
			bladecollider.GetComponent<triggerdamage>().setOn = false;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, retractPos, 5 * Time.deltaTime);

		}
		else
		{
			canfire = true;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);
		}
		
		


		if (((Input.GetButton("Fire1") || Input.GetAxis ("Fire1")>0.1)) && canfire && !GetComponent<Animation> ().IsPlaying (readyAnim.name)&& !GetComponent<Animation> ().IsPlaying (hideAnim.name))
		{

			

			bladecollider.GetComponent<triggerdamage>().setOn = true;
			if (!fireAudioSource.isPlaying)
			{
				fireAudioSource.clip = fireSound;
				fireAudioSource.loop = true;
				fireAudioSource.Play();
			}
			blade.GetComponent<Renderer>().material.SetFloat("_panning", 1f);
			GetComponent<Animation>()[fireAnim.name].speed = fireAnimSpeed; 
			
			GetComponent<Animation>().CrossFade(fireAnim.name);
			
			
		}
		else if (!GetComponent<Animation> ().IsPlaying (readyAnim.name)&& !GetComponent<Animation> ().IsPlaying (hideAnim.name))
		{
			bladecollider.GetComponent<triggerdamage>().setOn = false;
			fireAudioSource.Stop();
			GetComponent<Animation>()[idleAnim.name].speed = idleAnimSpeed; 
			
			GetComponent<Animation>().CrossFade(idleAnim.name);
			blade.GetComponent<Renderer>().material.SetFloat("_panning", 0f);
		}

			

		
		
	}
	
	
	
	void onstart()
	{
		myAudioSource.Stop();
		fireAudioSource.Stop();
		retract = false;

		bladecollider.GetComponent<triggerdamage>().setOn = false;
		GetComponent<Animation>().Stop();

			
		myAudioSource.clip = readySound;
		myAudioSource.loop = false;
		myAudioSource.volume = 1;
		myAudioSource.Play ();
			
		GetComponent<Animation> ().Play (readyAnim.name);

		canfire = true;

		
	}
	
	
	
	void doRetract()
	{
		GetComponent<Animation>().Play(hideAnim.name);
	}
	void doNormal()
	{
		

		onstart();
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
