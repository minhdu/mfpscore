using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PlayerControllerPC : Singleton<PlayerControllerPC> {


	public Transform mycamera;
	private Transform reference;

	public float jumpHeight = 2.0f;
	public float jumpinterval = 1.5f;
	private float nextjump = 1.2f;
	private float maxhitpoints = 1000f;
	private float hitpoints = 1000f;
	public float regen = 100f;
	public Text healthtext;
	public AudioClip[] hurtsounds;
	public RawImage painflashtexture;
	private float alpha;
	public Transform recoilCamera;

	public float gravity = 20.0f;
	public float rotatespeed = 4.0f;
	private float speed;
	public float normalspeed = 4.0f;
	public float runspeed = 8.0f;
	public float crouchspeed = 1.0f;
	public float crouchHeight = 1;
	private bool crouching = false;
	public float normalHeight = 2.0f;
	public float camerahighposition = 1.75f;
	public float cameralowposition = 0.9f;
	private float cameranewpositionY;
	private Vector3 cameranewposition;
	private float cameranextposition;
	public float dampTime = 2.0f;



	private float moveAmount;
	public float smoothSpeed = 2.0f;

	private Vector3 forward = Vector3.forward;
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 right;

	private float movespeed;
	public Vector3 localvelocity;


	public bool climbladder = false;
	public Quaternion ladderRotation;
	public Vector3 ladderposition;
	public Vector3 ladderforward;
	public Vector3 climbdirection;




	public float climbspeed = 2.0f;


	public bool canclimb = true;
	private Vector3 addVector = Vector3.zero;


	public bool running = false;
	public bool canrun = true;

	public AudioSource myAudioSource;
	Vector3 targetDirection = Vector3.zero;
	public Transform playermesh;
	public Vector3 playermeshNormalPosition;
	public Vector3 playermeshForwardPosition;
	public Transform playerskinnedmesh;
	private bool canrun2 = true;
	public bool hideselectedweapon = false;
	Vector3 targetVelocity;
	public float falldamage;
	private float airTime;
	public float falltreshold = 2f;
	private bool prevGrounded;
	public Transform Deadplayer;
	void Awake ()
	{
		reference = new GameObject().transform;

	}

	void Start () 
	{
		speed = normalspeed;
		painflashtexture.CrossFadeAlpha(0f,0f,true);
		cameranextposition = camerahighposition;
	}
	

	#if UNITY_EDITOR
	void Update () 
	{
		Animator meshanimator = playermesh.GetComponent<Animator>();
			
		reference.eulerAngles = new Vector3(0, mycamera.eulerAngles.y, 0);
		forward = reference.forward;
		right = new Vector3(forward.z, 0, -forward.x);
		float hor = Input.GetAxisRaw("Horizontal");
		float ver = Input.GetAxisRaw("Vertical");


		CharacterController controller = GetComponent<CharacterController>();
//		playerrotate rotatescript = GetComponent<playerrotate>();
		WeaponHandler inventory = GetComponent<WeaponHandler>();	
		Vector3 velocity = controller.velocity;
		localvelocity = transform.InverseTransformDirection(velocity);

		bool ismovingforward =localvelocity.z > .5f;


		if (climbladder && !controller.isGrounded && canclimb ) 
		{

			playermesh.transform.localPosition = Vector3.MoveTowards(playermesh.transform.localPosition,playermeshForwardPosition, Time.deltaTime * 2f);

			inventory.hideweapons = true;
			airTime = 0f;


			crouching = false;
			//playerskinnedmesh.GetComponent<Renderer>().material.SetFloat("_cutoff", 0f)



			Vector3 wantedPosition = (ladderposition - transform.position);
			if( wantedPosition.magnitude > 0.05f)
			{
				addVector = wantedPosition.normalized;


			}
			else
			{
				addVector = Vector3.zero;
			}
			meshanimator.SetBool ("climbladder", true);
			meshanimator.SetFloat("ver", Input.GetAxis("Vertical"));
//			rotatescript.enabled = false;

			targetDirection = (ver * climbdirection);
			targetDirection = targetDirection.normalized;
			targetDirection += addVector;

			moveDirection = targetDirection * climbspeed;
			Quaternion wantedrotation = Quaternion.LookRotation(ladderforward,Vector3.up);

			transform.rotation = Quaternion.Lerp(transform.rotation,wantedrotation,Time.deltaTime * 8f);
		} 
		else 
		{

			inventory.hideweapons = false;

			playermesh.transform.localPosition = Vector3.MoveTowards(playermesh.transform.localPosition,playermeshNormalPosition, Time.deltaTime * 2f);
			meshanimator.SetBool ("climbladder", false);
			//playerskinnedmesh.GetComponent<Renderer>().material.SetFloat("_cutoff", 1f);
//			rotatescript.enabled = true;

			targetDirection = (hor * right) + (ver * forward);
			targetDirection = targetDirection.normalized;
			targetVelocity = targetDirection;
			if (controller.isGrounded) 
			{
				
				airTime = 0f;
				
				if(Input.GetButtonDown("Crouch")) 
				{ 
					
					
					if (crouching )
					{
						meshanimator.SetBool ("crouch", true);
						canrun = false;
						controller.center = new Vector3(0f,crouchHeight / 2f,0f);
						controller.height = crouchHeight;
						canrun2 = false;
						cameranextposition = cameralowposition;
						canclimb = false;
						speed = crouchspeed;
					}
					
					
					else
					{
						meshanimator.SetBool ("crouch", false);
						canrun = true;
						controller.center = new Vector3(0f,normalHeight / 2f,0f);
						controller.height = normalHeight;
						canrun2 = true;
						cameranextposition = camerahighposition;
						canclimb = true;
						speed = normalspeed;
						
					}
					crouching = !crouching;
					
					
				}
				// Jump
				if (Input.GetButton ("Jump") && Time.time > nextjump)
				{
					nextjump = Time.time + jumpinterval;
					moveDirection.y = jumpHeight;
					

					meshanimator.SetBool ("jump", true);
					
				} 
				else 
				{

					meshanimator.SetBool ("jump", false);
					
				}  
				
				
			}
			
			else 
			{
				
				airTime += Time.deltaTime;
				moveDirection.y -= (gravity) * Time.deltaTime;
				nextjump = Time.time + jumpinterval;
				
				
			}
			if (Input.GetButton ("Fire2") && canrun && canrun2 && ismovingforward) 
			{
				targetVelocity *= runspeed;
				running = true;
				
			}
			else
			{
				targetVelocity *= speed;
				running = false;
				
				
			}
			moveDirection.z = targetVelocity.z;
			moveDirection.x = targetVelocity.x;
		}




		if (hitpoints <= 0)
		{
			//die
			Instantiate(Deadplayer, transform.position, transform.rotation);
			Destroy(gameObject);
		}





			
		cameranewpositionY = Mathf.Lerp(Camera.main.transform.localPosition.y,cameranextposition, Time.deltaTime * 4f);

			

		meshanimator.SetBool ("grounded", controller.isGrounded);
						

			
		meshanimator.SetFloat("hor",(localvelocity.x/speed) + (Input.GetAxis("Mouse X") /3f), dampTime , 0.2f);
		meshanimator.SetFloat("ver",(localvelocity.z/ speed), dampTime , 0.8f);


		cameranewposition = new Vector3(Camera.main.transform.localPosition.x,cameranewpositionY,Camera.main.transform.localPosition.z);
		Camera.main.transform.localPosition = cameranewposition;


		controller.Move (moveDirection * Time.deltaTime);
		if (!prevGrounded && controller.isGrounded)
		{
			
			//doland
			if (airTime > falltreshold)
			{
				Damage(falldamage * airTime * 2f);
			}
		}
		prevGrounded = controller.isGrounded;	
		if (hitpoints < maxhitpoints)
		hitpoints += regen * Time.deltaTime;
		
		string healthstring = (Mathf.Round(hitpoints/10f)).ToString();
		healthtext.text= (healthstring);
		float alpha = (hitpoints/1000f);
			
		painflashtexture.CrossFadeAlpha(1f - alpha, .5f, false);


	
	}
	#endif

	void Damage (float damage) 
	{
		camerarotate cameracontroller = recoilCamera.GetComponent<camerarotate>();
		
		cameracontroller.SendMessage("dorecoil", damage/3f,SendMessageOptions.DontRequireReceiver);
		if (!myAudioSource.isPlaying && hitpoints >= 0)
		{

			int n = Random.Range(1,hurtsounds.Length);
			myAudioSource.clip = hurtsounds[n];
			myAudioSource.pitch = 0.9f + 0.1f *Random.value;
			myAudioSource.Play();
			hurtsounds[n] = hurtsounds[0];
			hurtsounds[0] = myAudioSource.clip;
		}
		//damaged = true;
		//myAudioSource.PlayOneShot(hurtsound);
		hitpoints = hitpoints - damage;
	}


}

