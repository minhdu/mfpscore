using UnityEngine;
using System.Collections;


public class minigun : MonoBehaviour {

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
	public AudioSource servoAudioSource;
	public AudioClip emptySound;
	public AudioClip fireSound;
	
	public AudioClip servosound;
	public AudioClip readySound;
	public AudioClip reloadSound;

	public Transform barrels;
	public Vector3 barrelrotatedirection;
	
	private float barrelsrotatespeed = 0f;
	private float wantedspeed = 0f;
	private float wantedpitch = 0f;
	private float pitchspeed;
	
	public int ammoToReload = 20;

	public int projectilecount = 1;
	public float inaccuracy = 0.02f;
	public float spreadNormal = 0.08f;
	public float spreadAim = 0.02f;
	public float force  = 500f;
	public float damage = 50f;
	public float range = 100f;
	public float smoothdamping  = 2f;
	public float recoil = 5f;


	public AnimationClip fireAnim;
	public float fireAnimSpeed = 1.1f;

	public AnimationClip reloadAnim;
	public AnimationClip readyAnim;

	public AnimationClip hideAnim;
	public GameObject shell;

	public Transform shellPos;

	public float shellejectdelay = 0;
	public int ammo = 200;
	public int currentammo= 20;
	
	public Transform muzzle;


	private float nextshot;
	public float shotinterval = 0.2f;
	public Camera weaponcamera;
	public Transform recoilCamera;
	public float runXrotation = 20;
	private float nextField;
	private float weaponnextfield;


	private Vector3 wantedrotation;
	private bool canaim = true;

	private bool canfire = false;
	private bool canreload = true;
	private bool retract = false;	
	private bool isreloading  = false;
	public Transform grenadethrower;
	public Transform rayfirer;
	public Transform player;
	void Start()
	{
		
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

		wantedspeed = Mathf.Lerp(wantedspeed, barrelsrotatespeed, Time.deltaTime * 2f);
		wantedpitch = Mathf.Lerp(wantedpitch , pitchspeed, Time.deltaTime * 2f);
		barrels.Rotate(barrelrotatedirection  * Time.deltaTime * wantedspeed); 

		if (Input.GetButton("ThrowGrenade") && !GetComponent<Animation>().isPlaying)
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
		WeaponHandler inventory = player.GetComponent<WeaponHandler>();
		inventory.currentammo = currentammo;
		inventory.totalammo = ammo;


		if (playercontrol.running)
		{
			canfire = false;
			servoAudioSource.pitch = wantedpitch;
			wantedrotation = new Vector3((-10f +Xtilt)+ runXrotation,Ytilt,0f);

		}
		else
		{

			if (wantedspeed >= 0.35f)
			{
				servoAudioSource.pitch = wantedpitch;
				if (!servoAudioSource.isPlaying)
				{
					
					servoAudioSource.clip = servosound;
					servoAudioSource.loop = true;
					servoAudioSource.Play();
				}
				if (wantedspeed >= 500f)
				{
					canfire = true;
				}
				else
				{
					canfire = false;
				}
			}
			else 
			{
				servoAudioSource.Stop();
			}
			wantedrotation = new Vector3((-10f +Xtilt),Ytilt,0f);

		}

		transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(wantedrotation),5f * Time.deltaTime);

		if ((Input.GetButton("Aim")|| 	Input.GetAxis("Aim") > 0.1) && canaim && !playercontrol.running)
		{
			inaccuracy = spreadAim;
			barrelsrotatespeed = 600f;
			pitchspeed = 1.0f;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, aimposition, step);
			weaponnextfield = weaponaimFOV;
			nextField = aimFOV;

		}
		else
		{
			
			inaccuracy = spreadNormal;
			barrelsrotatespeed =0f;
			pitchspeed = 0.3f;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);
			weaponnextfield = weaponnormalFOV;
			nextField = normalFOV;

		}
		
		
		
		

		
		if (currentammo == 0 || currentammo  <= 0 )
		{	
			
			if (ammo <= 0)
			{
				canfire = false;
				canreload = false;
				if ((Input.GetButton("Fire1") || Input.GetAxis ("Fire1")>0.1) && !myAudioSource.isPlaying)
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

		
		if ((Input.GetButton("Fire1")  || Input.GetAxis ("Fire1")>0.1)  && !isreloading && canfire)

		{
			
			fire();
			
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


		raycastfire weaponfirer = rayfirer.GetComponent<raycastfire>();
		weaponfirer.inaccuracy = inaccuracy;
		weaponfirer.damage = damage;
		weaponfirer.range = range;
		weaponfirer.force = force;
		weaponfirer.projectilecount = projectilecount;
		
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

		}
		
	}

	void fire()
	{
		

		if (!GetComponent<Animation>().isPlaying)
		{
			float randomZ = Random.Range (-0.05f,-0.01f);
			//float randomY = Random.Range (-0.1f,0.1f);

			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y ,transform.localPosition.z + randomZ);
			camerarotate cameracontroller = recoilCamera.GetComponent<camerarotate>();

			cameracontroller.SendMessage("dorecoil", recoil,SendMessageOptions.DontRequireReceiver);

			StartCoroutine(flashthemuzzle());

			raycastfire weaponfirer = rayfirer.GetComponent<raycastfire>();
			weaponfirer.SendMessage("fire",SendMessageOptions.DontRequireReceiver);

			fireAudioSource.clip = fireSound;
			fireAudioSource.pitch = 0.9f + 0.1f *Random.value;
			fireAudioSource.Play();
			GetComponent<Animation>()[fireAnim.name].speed = fireAnimSpeed;     
			GetComponent<Animation>().Play(fireAnim.name);
			currentammo -=1;
			StartCoroutine(ejectshell(shellejectdelay));

		

			
			if (currentammo <= 0)
			{
				reload();
			}
			
		}
		
		
	}
	
	void reload()
	{





		if (!GetComponent<Animation>().isPlaying && canreload && !isreloading) {


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

	IEnumerator ejectshell(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		GameObject shellInstance;
		shellInstance = Instantiate(shell, shellPos.transform.position,shellPos.transform.rotation) as GameObject;

		shellInstance.GetComponent<Rigidbody>().AddRelativeForce(-30,80,0);
		shellInstance.GetComponent<Rigidbody>().AddRelativeTorque(-500,20,400);
		
	}

	IEnumerator setreload(float waitTime)
	{
		WeaponHandler selector = player.GetComponent<WeaponHandler>();
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
	IEnumerator flashthemuzzle()
	{
		muzzle.transform.localEulerAngles = new Vector3(0f,0f,Random.Range(0f,360f));
		muzzle.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.05f);
		muzzle.gameObject.SetActive(false);
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

