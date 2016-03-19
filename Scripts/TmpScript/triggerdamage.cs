using UnityEngine;
using System.Collections;

public class triggerdamage : MonoBehaviour {
	public float damage = 5f;
	public Transform particles;
	public Transform bloodparticles;
	public AudioClip impactsound;
	public AudioSource myAudioSource;
	private AnimationCurve curve;
	public bool setOn ;
	void Start()
	{
		curve = new AnimationCurve ();
		curve.AddKey(0.0f,0.1f);
		curve.AddKey(0.75f,1.0f);
		myAudioSource.Stop();
	}
	void Update()
	{
		if(! setOn)
		{
			
			myAudioSource.Stop();
			ParticleSystem[] particleSystems;
			particleSystems = particles.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particle in particleSystems)
			{
				var em = particle.emission;
				em.rate= new ParticleSystem.MinMaxCurve(0,curve);

			}
			ParticleSystem[] bloodparticleSystems;
			bloodparticleSystems = bloodparticles.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particle in bloodparticleSystems)
			{
				var em = particle.emission;
				em.rate= new ParticleSystem.MinMaxCurve(0,curve);
			}
		}
	}
	void OnTriggerStay(Collider other) 
	{
		
		
		if (setOn)
		{
			if (other.gameObject.tag == "flesh")
			{
				ParticleSystem[] bloodparticleSystems;
				bloodparticleSystems = bloodparticles.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particle in bloodparticleSystems)
				{
					var em = particle.emission;
					em.rate= new ParticleSystem.MinMaxCurve(8f,curve);
				}
				if (!myAudioSource.isPlaying)
				{
					myAudioSource.clip = impactsound;
					myAudioSource.loop = true;
					myAudioSource.volume = 1;
					myAudioSource.Play ();
				}
				other.transform.SendMessageUpwards ("Damage",damage * Time.deltaTime, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				ParticleSystem[] particleSystems;
				particleSystems = particles.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particle in particleSystems)
				{
					var em = particle.emission;
					em.rate= new ParticleSystem.MinMaxCurve(20f,curve);
				}
				if (!myAudioSource.isPlaying)
				{
					myAudioSource.clip = impactsound;
					myAudioSource.loop = true;
					myAudioSource.volume = 1;
					myAudioSource.Play ();
				}
				other.transform.SendMessageUpwards ("Damage",damage * Time.deltaTime, SendMessageOptions.DontRequireReceiver);
			}

		}
		else
		{
			ParticleSystem[] particleSystems;
			particleSystems = particles.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particle in particleSystems)
			{
				var em = particle.emission;
				em.rate= new ParticleSystem.MinMaxCurve(0,curve);
			}
			ParticleSystem[] bloodparticleSystems;
			bloodparticleSystems = bloodparticles.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particle in bloodparticleSystems)
			{
				var em = particle.emission;
				em.rate= new ParticleSystem.MinMaxCurve(0,curve);
			}
			myAudioSource.Stop ();
		}
	}
	
	void OnTriggerExit(Collider other)
		
	{
		ParticleSystem[] particleSystems;
		particleSystems = particles.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particle in particleSystems)
		{
			var em = particle.emission;
			em.rate= new ParticleSystem.MinMaxCurve(0,curve);
		}
		ParticleSystem[] bloodparticleSystems;
		bloodparticleSystems = bloodparticles.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particle in bloodparticleSystems)
		{
			var em = particle.emission;
			em.rate= new ParticleSystem.MinMaxCurve(0,curve);
		}
		myAudioSource.Stop ();
	}
	
}

