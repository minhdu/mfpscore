using UnityEngine;
using System.Collections;

public class WeaponRoot : MonoBehaviour {

	public Transform player;
	public float moveSpeed = 8f;
	public float headbobSpeed = 0.5f;
	public float maxSway = 10f;
	private float currentsway;
	private float currentrotation;
	public float bobX = 0.1f;
	public float bobY = 0.2f;
	public float aimbob = 0.5f;
	public float normalbob = 1.0f;
	private float currentbob;
	public float jumplandMove = 0.1f;

	float headbobStepCounter;
	Vector3 parentLastPosition;
	Vector3 startPosition;

	Vector3 wantedposition;
	private Vector3  prevPosition ;							
	private Vector3 prevVelocity = Vector3.zero;
	private float	springPos  = 0f;
	private float	springVelocity  = 0f;
	private float	springElastic = 1.1f;		
	private float	springDampen = 0.8f;
	private float	springVelocityThreshold = 0.1f;
	private float springPositionThreshold= 0.1f;
	void Start () 
	{

		parentLastPosition = transform.parent.position;
		startPosition = transform.localPosition;
		prevPosition = player.transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		
		Vector3 velocity= (player.transform.position - prevPosition) / Time.deltaTime;
		Vector3 velocityChange = velocity - prevVelocity;
		prevPosition = player.transform.position;
		prevVelocity = velocity;
		playercontroller playercontrol = player.GetComponent<playercontroller>();
		springVelocity -= velocityChange.y;		
		if (((Input.GetButton("Aim")|| 	Input.GetAxis("Aim") > 0.1)) && !playercontrol.running)
		{
			currentsway = maxSway/2.5f;
			currentbob = aimbob;
		}
		else
		{
			currentsway = maxSway;
			currentbob = normalbob;
		}



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
		float moveX = Mathf.Sin(headbobStepCounter)* bobX * currentbob;
		float moveY = Mathf.Sin(headbobStepCounter * 2) * bobY * -1f * currentbob;
		if (player.GetComponent<CharacterController>().isGrounded) 
		{
			
			//dostepbob
			if (player.GetComponent<CharacterController>().velocity.magnitude > 0.2f)
			{
				headbobStepCounter += Vector3.Distance (parentLastPosition, transform.parent.position) * headbobSpeed;
			}
			else //returnNormal
			{
				headbobStepCounter = 0f;
			}


		} 




	

		Vector3 playervelocity = playercontrol.localvelocity;
		if (playervelocity.x > 0.2f)
		{
			//Sway right
			currentrotation = currentsway  * playervelocity.x;
		}

		else if (playervelocity.x < -0.2f)
		{
			//Sway left
			currentrotation = -currentsway * -playervelocity.x;
		}
		else
		{
			currentrotation = 0f;
		}
		Vector3 Wantedrotation = new Vector3(0f,0f, currentrotation);

		transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(Wantedrotation),moveSpeed * Time.deltaTime);
		wantedposition = new Vector3(moveX,moveY + yPos,0f);
		transform.localPosition = Vector3.Lerp(transform.localPosition,startPosition + wantedposition,20f * Time.deltaTime);
		parentLastPosition = transform.parent.position;
	}
}
