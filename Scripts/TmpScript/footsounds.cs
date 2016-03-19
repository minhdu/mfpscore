using UnityEngine;
using System.Collections;

public class footsounds : MonoBehaviour {
	private AudioSource myaudio;
	public AudioClip[] Sounds; 

	void Start() {
		myaudio = GetComponent<AudioSource>();

	}

	void OnCollisionEnter() 
	{
		if (!myaudio.isPlaying)
		{
			int n = Random.Range(1,Sounds.Length);
			myaudio.clip = Sounds[n];
			myaudio.pitch = 0.9f + 0.1f *Random.value;
			myaudio.PlayOneShot(myaudio.clip);

			Sounds[n] = Sounds[0];
			Sounds[0] = myaudio.clip;
		}
	}

		
}
