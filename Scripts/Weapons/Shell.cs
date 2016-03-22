using UnityEngine;
using System.Collections;

public class Shell : MonoBehaviour {
	public float  waitTime = 2f;
	public AudioSource myAudioSource;
	public AudioClip[] shellsounds ;
	// Use this for initialization
	void Start () {
		Destroy (gameObject, waitTime);	
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionEnter(Collision collision) 
	{
		if (!myAudioSource.isPlaying)
		{
			myAudioSource.clip = shellsounds[Random.Range(0,shellsounds.Length)];
			myAudioSource.Play();
		}
	}

}
