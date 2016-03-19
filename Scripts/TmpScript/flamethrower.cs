using UnityEngine;
using System.Collections;


public class flamethrower : MonoBehaviour {

	public Vector3 normalposition;
	public Vector3 aimposition;	
	public Vector3 retractPos;
	
	public float aimFOV = 45f;
	public float normalFOV  = 65f;
	public float weaponnormalFOV = 32f;
	public float weaponaimFOV  = 20f;
	public float speed = 1f;


	public AudioSource myAudioSource;

	
	public AudioSource fireAudioSource;
	public AudioClip emptySound;
	public AudioClip fireSound;
	public Transform flames;
	public AudioClip readySound;
	public AudioClip reloadSound;
	
	
	public float ammoToReload = 100f;

	public float ammo = 200f;
	public float currentammo= 100f;

	public AnimationClip fireAnim;
	public float fireAnimSpeed = 1.1f;

	public AnimationClip reloadAnim;
	public AnimationClip readyAnim;

	public AnimationClip hideAnim;



	public Camera weaponcamera;

	public float runXrotation = 20;
	private float nextField;
	private float weaponnextfield;


	private Vector3 wantedrotation;
	private bool canaim = true;

	private bool canfire = true;
	private bool canreload = true;
	private bool retract = false;	
	private bool isreloading  = false;
	public Transform grenadethrower;
	private AnimationCurve curve;
	public Transform player;
	void Start()
	{
		curve = new AnimationCurve ();
		curve.AddKey(0.0f,0.1f);
		curve.AddKey(0.75f,1.0f);
		nextField = normalFOV ;
		weaponnextfield = weaponnormalFOV;
		GetComponent<Animation>().Stop();
		onstart();

	}
	void Update () 
	{
		

		float step = speed * Time.deltaTime;
		
		float newField = Mathf.Lerp(Camera.main.fieldOfView, nextField, Time.deltaTime * 2);
		float newfieldweapon = Mathf.Lerp(weaponcamera.fieldOfView, weaponnextfield, Time.deltaTime * 2);
		Camera.main.fieldOfView = newField;
		weaponcamera.fieldOfView = newfieldweapon;
		float Xtilt = Input.GetAxisRaw("Mouse Y") * 20f * Time.smoothDeltaTime;
		float Ytilt = Input.GetAxisRaw("Mouse X") * 20f * Time.smoothDeltaTime;

		if (Input.GetButton("ThrowGrenade") && GetComponent<Animation>()[fireAnim.name].speed == 0f)
		{
			StartCoroutine(setThrowGrenade());
		}
		if (retract)
		{
			canfire = false;
			canaim = false;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, retractPos, 5 * Time.deltaTime);
			weaponnextfield = weaponnormalFOV;
			nextField = normalFOV;
		}
		playercontroller playercontrol = player.GetComponent<playercontroller>();
		weaponselector inventory = player.GetComponent<weaponselector>();
		inventory.currentammo = Mathf.RoundToInt(currentammo);
		inventory.totalammo = Mathf.RoundToInt(ammo);
		if (playercontrol.running)
		{
			canfire = false;

			wantedrotation = new Vector3(Xtilt + runXrotation,Ytilt,0f);

		}
		else
		{
			canfire = true;

			wantedrotation = new Vector3(Xtilt,Ytilt,0f);

		}

		transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(wantedrotation),5f * Time.deltaTime);

		if ((Input.GetButton("Aim")|| 	Input.GetAxis("Aim") > 0.1) && canaim && !playercontrol.running)
		{


			transform.localPosition = Vector3.MoveTowards(transform.localPosition, aimposition, step);
			weaponnextfield = weaponaimFOV;
			nextField = aimFOV;

		}
		else
		{
			

			
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);
			weaponnextfield = weaponnormalFOV;
			nextField = normalFOV;

		}
		
		
		
		

		if ((Input.GetButton("Fire1") || Input.GetAxis ("Fire1")>0.1) && canfire && !isreloading && !GetComponent<Animation> ().IsPlaying (readyAnim.name)&& !GetComponent<Animation> ().IsPlaying (hideAnim.name))
		{
			
			currentammo -= 5f * Time.deltaTime;
			ParticleSystem[] particleSystems;
			particleSystems = flames.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particle in particleSystems)
			{
				var em = particle.emission;
				em.rate= new ParticleSystem.MinMaxCurve(100f,curve);
			}
			if (!fireAudioSource.isPlaying)
			{
				fireAudioSource.clip = fireSound;
				fireAudioSource.loop = true;
				fireAudioSource.Play();
			}
			
			GetComponent<Animation>()[fireAnim.name].speed = fireAnimSpeed; 
			
			GetComponent<Animation>().Play(fireAnim.name);
			
			
		}
		else
		{
			ParticleSystem[] particleSystems;
			particleSystems = flames.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particle in particleSystems)
			{
				var em = particle.emission;
				em.rate= new ParticleSystem.MinMaxCurve(0,curve);
			}
			fireAudioSource.Stop();
			GetComponent<Animation>()[fireAnim.name].speed = 0f;
			
		}
		if (currentammo  <= 0f )
		{	
			
			if (ammo <= 0f)
			{
				canfire = false;
				canreload = false;
				if ((Input.GetButton("Fire1") && canfire || Input.GetAxis ("Fire1")>0.1) && !myAudioSource.isPlaying )
				{
					myAudioSource.PlayOneShot(emptySound);
				}
				else
				{
					canreload = true;
				}
			}
			else 
			{
				reload();
			}
			
			
		}
		
		
	}
	
	void doRetract()
	{
		GetComponent<Animation>().Play(hideAnim.name);
	}
	
	void onstart()
	{
		myAudioSource.Stop();
		fireAudioSource.Stop();



		
		GetComponent<Animation>().Stop();
		if (isreloading) {
			reload ();
		} 
		else 
		{



			myAudioSource.clip = readySound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play ();

			GetComponent<Animation> ().Play (readyAnim.name);
			canaim = true;
			canfire = true;
		}
		
	}

	
	void reload()
	{





		if (canreload && !isreloading) {


			StartCoroutine(setreload (GetComponent<Animation>()[reloadAnim.name].length));
			 
			myAudioSource.clip = reloadSound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play();		

			GetComponent<Animation> ().Play(reloadAnim.name);
			


			ammoToReload = Mathf.Clamp (ammoToReload, ammoToReload, ammo);
			
			ammo -= ammoToReload;
			currentammo += ammoToReload;




		} 



	}
	
	

	void doNormal()
	{


		onstart();
	}



	IEnumerator setreload(float waitTime)
	{
		weaponselector selector = player.GetComponent<weaponselector>();
		selector.canswitch = false;
		isreloading = true;
		playercontroller playercontrol = player.GetComponent<playercontroller>();
		playercontrol.canclimb = false;
		canaim = false;
		yield return new WaitForSeconds (waitTime);
		isreloading = false;
		canaim = true;
		selector.canswitch = true;
		playercontrol.canclimb = true;
		
	}

	IEnumerator setThrowGrenade()
	{
		retract = true;
		grenadethrower.gameObject.SetActive(true);
		grenadethrower.gameObject.BroadcastMessage("throwstuff");
		yield return new WaitForSeconds(grenadethrower.GetComponent<Animation>()["throwAnim"].length);
		retract = false;
		canaim = true;
		grenadethrower.gameObject.SetActive(false);
	}



}

