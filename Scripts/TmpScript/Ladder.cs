using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour {

	public bool test = false;
	Quaternion myrotation;
	Vector3 direction;
	//float waittoclimbagain = 2f;
	public Transform ladderTop;
	public Transform ladderBottom;

	Vector3 wantedLadderposition;
	float ControllerY;

	float lengthDiagonal;
	Vector3 delta;
	float lengthB;
	float wantedZ;
	float wantedX;

	void Start () 
	{
		myrotation = transform.rotation;
		direction = ladderTop.transform.position -  ladderBottom.transform.position;
		direction = direction.normalized;



	}
	
	// Update is called once per frame
	void Update ()
	{


		



	}
	void OnTriggerStay (Collider other) 
	{

		if  (other.tag == "Player")
		{
			playercontroller controller = other.GetComponent<playercontroller>();
			ControllerY = other.transform.position.y;
			test =true;
			delta = ladderTop.position - ladderBottom.position;
			lengthDiagonal = Mathf.Pow((delta.x * delta.x) + (delta.z * delta.z), 0.5f);


			if (lengthDiagonal == 0f)
			{
				wantedZ = ladderBottom.position.z;
				wantedX = ladderBottom.position.x;
			}
			else
			{
				lengthB = lengthDiagonal * ((ControllerY - ladderBottom.position.y)/ (ladderTop.position.y - ladderBottom.position.y));
				wantedZ = ladderBottom.position.z + ((ladderTop.position.z - ladderBottom.position.z) * (lengthB / lengthDiagonal));
				wantedX = ladderBottom.position.x + ((ladderTop.position.x - ladderBottom.position.x) * (lengthB / lengthDiagonal));

			}



			wantedLadderposition = new Vector3(wantedX,ControllerY,wantedZ);

			controller.climbdirection = direction;
			controller.climbladder = true;
			controller.ladderposition = wantedLadderposition;
			controller.ladderforward = (-transform.forward);
			controller.ladderRotation = myrotation;

		}
	
		else
		{

			test = false;
		}


		
	}
	void OnTriggerExit (Collider other)
	{
		if (other.tag == "Player")
		{
			playercontroller controller = other.GetComponent<playercontroller>();

			controller.climbladder = false;
			test = false;

		}
		test = false;


		
	}
}
