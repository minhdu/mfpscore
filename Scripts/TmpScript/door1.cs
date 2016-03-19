using UnityEngine;
using System.Collections;

public class door1 : MonoBehaviour {


	public Transform door;
	public AudioSource myAudioSource;
	public AudioClip openSound;
	public bool canOpen = true;
	public float speed = 2.0f;

	private float z;
	public float maxrotation = 0.8f;


	private Quaternion wantedRotation = Quaternion.identity;
	public bool opendoor = false;
	public bool closedoor = false;
	private bool isopen = false;

	void Update ()
	{
		door.transform.localRotation = Quaternion.Lerp(door.transform.localRotation,wantedRotation,Time.deltaTime * speed);
	}
	void doorAction () 
	{

		if (canOpen)
		{
			if (opendoor)
			{

				if(!myAudioSource.isPlaying)
				{
					myAudioSource.PlayOneShot(openSound);

				}

				wantedRotation.y = -maxrotation;


			}
			else if( closedoor)
			{
				if(!myAudioSource.isPlaying)
				{
					myAudioSource.PlayOneShot(openSound);

				}
				
				wantedRotation.y = 0f;

			}

				
		}



	

	}
	void OnTriggerStay (Collider other) 
	{
		
		if  (other.tag == "Player")
		{
			//enable




			if (Input.GetButtonDown("Action"))
			{
				//dostuff

				if (!isopen)
				{
					opendoor = true;
					closedoor = false;
					isopen = true;
				}
				else
				{
					closedoor = true;
					opendoor = false;
					isopen = false;
				}
				doorAction ();
			}


			
		}
		

		
		
		
	}

}
