using UnityEngine;
using System.Collections;

public class throwgrenade : MonoBehaviour {
	public float throwforce = 200.0f;
	public float ejectdelay = 1.0f;
	public GameObject projectile;
	public AudioClip throwSound;
	public AudioSource myAudioSource;

	void throwstuff () 
	{
		if (!GetComponent<Animation>().isPlaying)
		{
			StartCoroutine(throwprojectile(ejectdelay));
			myAudioSource.clip = throwSound;
			myAudioSource.pitch = 0.9f + 0.1f *Random.value;
			myAudioSource.Play();
			
			GetComponent<Animation>().Play("throwAnim");

		}

	
	}


	IEnumerator throwprojectile(float waitTime)
	{
		
		yield return new WaitForSeconds(waitTime);
		
		GameObject grenadeInstance = Instantiate(projectile, transform.position,transform.rotation) as GameObject;
		
		grenadeInstance.GetComponent<Rigidbody>().AddRelativeForce(0f,throwforce/ 4f,throwforce);

	}
}
