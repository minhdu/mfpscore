using UnityEngine;
using System.Collections;

public class HeadBob : MonoBehaviour {

	public Transform player;
	public float moveSpeed = 8f;
	public float headbobSpeed = 0.5f;
	float bobX = 0.1f;
	float bobY = 0.1f;
	public float aimratio = 1f;

	float headbobStepCounter;
	float jumplandMove = 3f;
	Vector3 parentLastPosition;
	Vector3 startPosition;

	Vector3 wantedposition;
	bool prevGrounded;
	public AudioClip[] FootstepSounds;    
	public AudioClip JumpSound;           
	public AudioClip LandSound;
	public AudioSource leftfootAudioSource;
	public AudioSource rightfootAudioSource;
	public AudioSource myAudioSource;
	private Vector3  prevPosition ;							
	private Vector3 prevVelocity = Vector3.zero;
	private float	springPos  = 0f;
	private float	springVelocity  = 0f;
	private float	springElastic = 1.1f;		
	private float	springDampen = 0.8f;
	private float	springVelocityThreshold = 0.05f;
	private float springPositionThreshold= 0.05f;


	void Start () 
	{
		
		parentLastPosition = transform.parent.position;
		startPosition = transform.localPosition;
		prevPosition = player.transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{

		Vector3 velocity= (player.transform.position - prevPosition) / Time.deltaTime;
		Vector3 velocityChange = velocity - prevVelocity;
		prevPosition = player.transform.position;
		prevVelocity = velocity;

		springVelocity -= velocityChange.y;		
			


								
		springVelocity -= springPos*springElastic;					
		springVelocity *= springDampen;								
		springPos += springVelocity * Time.deltaTime;	
		springPos = Mathf.Clamp( springPos, -.3f, .3f );			

		if (Mathf.Abs(springVelocity) < springVelocityThreshold && Mathf.Abs (springPos) < springPositionThreshold)
		{
			springVelocity = 0;
			springPos = 0;
		}


		float yPos = springPos * jumplandMove;

		playercontroller playercontrol = player.GetComponent<playercontroller>();



		float moveX = Mathf.Sin(headbobStepCounter)* bobX * aimratio;
		float moveY = Mathf.Sin(headbobStepCounter * 2) * bobY * -1f * aimratio;

			
		if (!prevGrounded && player.GetComponent<CharacterController>().isGrounded && !myAudioSource.isPlaying)
		{
			
			//doland
			
			myAudioSource.clip = LandSound;
			myAudioSource.Play();
			
			
		}
		else if (prevGrounded && !player.GetComponent<CharacterController>().isGrounded && !myAudioSource.isPlaying)
		{
			//dojump
			
			
			myAudioSource.clip = JumpSound;
			myAudioSource.Play();
			
		}



		if (player.GetComponent<CharacterController>().isGrounded) 
		{
			
			//dostepbob
			if (player.GetComponent<CharacterController>().velocity.magnitude > 0.2f)
			{
				if (moveX >= 0.099f )
				{
					if (!rightfootAudioSource.isPlaying)
					{
						int n = Random.Range(1,FootstepSounds.Length);
						rightfootAudioSource.clip = FootstepSounds[n];
						rightfootAudioSource.pitch = 0.9f + 0.1f *Random.value;
						rightfootAudioSource.PlayOneShot(rightfootAudioSource.clip);
						
						FootstepSounds[n] = FootstepSounds[0];
						FootstepSounds[0] = rightfootAudioSource.clip;
					}
					
					
				}
				else if (moveX <= -0.099f)
				{
					if (!leftfootAudioSource.isPlaying)
					{
						int n = Random.Range(1,FootstepSounds.Length);
						leftfootAudioSource.clip = FootstepSounds[n];
						leftfootAudioSource.pitch = 0.9f + 0.1f *Random.value;
						leftfootAudioSource.PlayOneShot(leftfootAudioSource.clip);
						
						FootstepSounds[n] = FootstepSounds[0];
						FootstepSounds[0] = leftfootAudioSource.clip;
					}
				}

				headbobStepCounter += Vector3.Distance (parentLastPosition, transform.parent.position) * headbobSpeed;
			}
			else //returnNormal
			{
				headbobStepCounter = 0f;
			}


		} 
		else if (playercontrol.climbladder)
		{
			if (player.GetComponent<CharacterController>().velocity.magnitude > 0.2f)
			{
				if (moveX >= 0.099f )
				{
					if (!rightfootAudioSource.isPlaying)
					{
						int n = Random.Range(1,FootstepSounds.Length);
						rightfootAudioSource.clip = FootstepSounds[n];
						rightfootAudioSource.pitch = 0.9f + 0.1f *Random.value;
						rightfootAudioSource.PlayOneShot(rightfootAudioSource.clip);
						
						FootstepSounds[n] = FootstepSounds[0];
						FootstepSounds[0] = rightfootAudioSource.clip;
					}
					
					
				}
				else if (moveX <= -0.099f)
				{
					if (!leftfootAudioSource.isPlaying)
					{
						int n = Random.Range(1,FootstepSounds.Length);
						leftfootAudioSource.clip = FootstepSounds[n];
						leftfootAudioSource.pitch = 0.9f + 0.1f *Random.value;
						leftfootAudioSource.PlayOneShot(leftfootAudioSource.clip);
						
						FootstepSounds[n] = FootstepSounds[0];
						FootstepSounds[0] = leftfootAudioSource.clip;
					}
				}
			}
			headbobStepCounter += Vector3.Distance (parentLastPosition, transform.parent.position) * headbobSpeed *2.4f;

		}
	

		wantedposition = new Vector3(moveX,moveY + yPos,0f);

		transform.localPosition = Vector3.Lerp(transform.localPosition,startPosition + wantedposition,20f * Time.deltaTime);
		prevGrounded = player.GetComponent<CharacterController>().isGrounded;
		parentLastPosition = transform.parent.position;
	}
}
