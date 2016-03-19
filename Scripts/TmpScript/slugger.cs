using UnityEngine;
using System.Collections;


public class slugger : MonoBehaviour {

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
	
	public AudioClip readySound;
	public AudioClip reloadSound;
	
	public Transform projectile;
	public Transform projectilePos;
	public int ammoToReload = 20;


	public GameObject shell;
	
	public Transform shellPos;
	public Transform clipShell;

	public float shellejectdelay = 0;
	public float smoothdamping  = 2f;
	public float recoil = 5f;


	public AnimationClip fireAnim;
	public float fireAnimSpeed = 1.1f;

	public AnimationClip reloadAnim;
	public AnimationClip readyAnim;

	public AnimationClip hideAnim;





	public int ammo = 200;
	public int currentammo= 20;
	
	public Transform muzzle;



	public Camera weaponcamera;
	public Transform recoilCamera;
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
		weaponselector inventory = player.GetComponent<weaponselector>();
		inventory.currentammo = currentammo;
		inventory.totalammo = ammo;
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



		
		GetComponent<Animation>().Stop();
		if (isreloading) {
			reload ();
		} 
		else 
		{

			clipShell.gameObject.SetActive (true);

			myAudioSource.clip = readySound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play ();

			GetComponent<Animation> ().Play (readyAnim.name);
			canaim = true;
			canfire = true;
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

			Instantiate(projectile, projectilePos.transform.position,projectilePos.transform.rotation);
			StartCoroutine(ejectshell(shellejectdelay));
			fireAudioSource.clip = fireSound;
			fireAudioSource.pitch = 0.9f + 0.1f *Random.value;
			fireAudioSource.Play();
			GetComponent<Animation>()[fireAnim.name].speed = fireAnimSpeed;     
			GetComponent<Animation>().Play(fireAnim.name);
			currentammo -=1;


		

			
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
			StartCoroutine (deactivateShell (GetComponent<Animation> () [reloadAnim.name].length * 0.5f)); 


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
	IEnumerator ejectshell(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		GameObject shellInstance;
		shellInstance = Instantiate(shell, shellPos.transform.position,shellPos.transform.rotation) as GameObject;
		
		shellInstance.GetComponent<Rigidbody>().AddRelativeForce(20,80,0);
		shellInstance.GetComponent<Rigidbody>().AddRelativeTorque(35,45,0);
		
	}
	IEnumerator deactivateShell(float waitTime)
	{
		clipShell.gameObject.SetActive (false);
		yield return new WaitForSeconds(waitTime);
		clipShell.gameObject.SetActive (true);
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

