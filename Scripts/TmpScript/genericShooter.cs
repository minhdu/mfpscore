using UnityEngine;
using System.Collections;


public class genericShooter : MonoBehaviour {

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
	public Transform clipShell;


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
	public Transform rayfirer;
	public Transform player;

	Animation anim;
	Transform trans;

	[SerializeField]
	bool isShooting = false;
	bool isAiming = false;

	void Start()
	{
		anim = GetComponent<Animation>();
		trans = GetComponent<Transform> ();
		nextField = normalFOV ;
		weaponnextfield = weaponnormalFOV;
		anim.Stop();
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

		if (Input.GetButton("ThrowGrenade") && !anim.isPlaying)
		{
			StartCoroutine(setThrowGrenade());
		}
		if (retract)
		{
			canfire = false;
			canaim = false;
			trans.localPosition = Vector3.MoveTowards(trans.localPosition, retractPos, 5 * Time.deltaTime);
			weaponnextfield = weaponnormalFOV;
			nextField = normalFOV;
		}
		playercontroller playercontrol = playercontroller.Instance;
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

		trans.localRotation = Quaternion.Lerp(trans.localRotation,Quaternion.Euler(wantedrotation),5f * Time.deltaTime);

		if (isAiming && canaim && !playercontrol.running)
		{
			inaccuracy = spreadAim;

			trans.localPosition = Vector3.MoveTowards(trans.localPosition, aimposition, step);
			weaponnextfield = weaponaimFOV;
			nextField = aimFOV;

		}
		else
		{
			
			inaccuracy = spreadNormal;
			
			trans.localPosition = Vector3.MoveTowards(trans.localPosition, normalposition, step);
			weaponnextfield = weaponnormalFOV;
			nextField = normalFOV;

		}
		
		
		
		

		
		if (currentammo == 0 || currentammo  <= 0 )
		{	
			
			if (ammo <= 0)
			{
				canfire = false;
				canreload = false;
				if (isShooting && !myAudioSource.isPlaying)
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
				Reload();
			}
			
			
		}

		
		if (isShooting  && !isreloading && canfire)

		{
			
			Shoot();
			
		}
		
		
	}
	
	void doRetract()
	{
		anim.Play(hideAnim.name);
	}
	
	void onstart()
	{
		myAudioSource.Stop();
		fireAudioSource.Stop();

		anim = GetComponent<Animation> ();

		raycastfire.Instance.inaccuracy = inaccuracy;
		raycastfire.Instance.damage = damage;
		raycastfire.Instance.range = range;
		raycastfire.Instance.force = force;
		raycastfire.Instance.projectilecount = projectilecount;
		
		anim.Stop();
		if (isreloading) {
			Reload ();
		} 
		else 
		{
			clipShell.gameObject.SetActive (true);


			myAudioSource.clip = readySound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play ();

			anim.Play (readyAnim.name);
			canaim = true;
			canfire = true;
		}
		
	}

	void Shoot () {
		if (!anim.isPlaying)
		{
			float randomZ = Random.Range (-0.05f,-0.01f);
			//float randomY = Random.Range (-0.1f,0.1f);

			trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y ,trans.localPosition.z + randomZ);
			camerarotate cameracontroller = recoilCamera.GetComponent<camerarotate>();

			cameracontroller.SendMessage("dorecoil", recoil,SendMessageOptions.DontRequireReceiver);

			StartCoroutine(flashthemuzzle());

			raycastfire.Instance.SendMessage("fire",SendMessageOptions.DontRequireReceiver);

			fireAudioSource.clip = fireSound;
			fireAudioSource.pitch = 0.9f + 0.1f *Random.value;
			fireAudioSource.Play();
			anim[fireAnim.name].speed = fireAnimSpeed;     
			anim.Play(fireAnim.name);
			currentammo -=1;
			StartCoroutine(ejectshell(shellejectdelay));

			if (currentammo <= 0) {
				Reload();
			}
			
		}
	}
	
	void Reload() {

		if (!anim.isPlaying && canreload && !isreloading) {


			StartCoroutine(setreload (anim[reloadAnim.name].length));
			StartCoroutine (deactivateShell (anim [reloadAnim.name].length * 0.5f)); 
			myAudioSource.clip = reloadSound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play();		

			anim.Play(reloadAnim.name);
			


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

		shellInstance.GetComponent<Rigidbody>().AddRelativeForce(30,80,0);
		shellInstance.GetComponent<Rigidbody>().AddRelativeTorque(500,20,800);
		
	}
	IEnumerator deactivateShell(float waitTime)
	{
		clipShell.gameObject.SetActive (false);
		yield return new WaitForSeconds(waitTime);
		clipShell.gameObject.SetActive (true);
	}
	IEnumerator setreload(float waitTime)
	{
		playercontroller playercontrol = playercontroller.Instance;
		playercontrol.canclimb = false;
		weaponselector selector = player.GetComponent<weaponselector>();
		selector.canswitch = false;

		isreloading = true;

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


	public void DoShoot () {
		isShooting = true;
	}

	public void StopShoot () {
		isShooting = false;
	}

	public void DoReload () {
		Reload ();
	}

	public void DoAim () {
		isAiming = !isAiming;
	}
}

