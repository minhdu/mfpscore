using UnityEngine;
using System.Collections;

public class GrenadeHandler : Singleton<GrenadeHandler> {

	public Renderer animatedGrenadeRender;
	public Transform explosionPrefab;
	public AudioClip hitAudio;
	public float cookTime = 3.0f;

	public GameObject grenadePrefab;
	GameObject currentGrenade;

	Rigidbody grenadeRigidbody;
	AudioSource audioSource;

	Transform grenadeTransform;
	Transform mParentTransform;

	Vector3 originalPosition;
	Quaternion originalRotation;

	float dirSpeed = 15;

	void Start () {
		InitNewGrenade ();
	}

	public void Throw () {
		StartCoroutine (InternalThrow ());
	}

	IEnumerator InternalThrow () {
		currentGrenade.SetActive (true);
		animatedGrenadeRender.enabled = true;
		yield return new WaitForSeconds (1.65f);
		animatedGrenadeRender.enabled = false;
		grenadeTransform.parent = null;
		grenadeRigidbody.useGravity = true;
		grenadeRigidbody.isKinematic = false;
		grenadeRigidbody.velocity = FPSCamera.Instance.Transform.TransformDirection(new Vector3 (0, 0, dirSpeed));
		yield return new WaitForSeconds (cookTime);
		Instantiate(explosionPrefab, grenadeTransform.position, Quaternion.identity);
		Destroy (currentGrenade);
		CameraShake.Instance.GrenadeShake ();
		Revert ();
		InitNewGrenade ();
	}

	void Revert () {
		grenadeRigidbody.useGravity = false;
		grenadeRigidbody.isKinematic = true;
		grenadeTransform.parent = mParentTransform;
		grenadeTransform.localPosition = originalPosition;
		grenadeTransform.rotation = originalRotation;
	}

	public void InitNewGrenade () {
		currentGrenade = Instantiate (grenadePrefab) as GameObject; // Pool
		currentGrenade.transform.parent = grenadePrefab.transform.parent;
		currentGrenade.transform.localPosition = grenadePrefab.transform.localPosition;
		currentGrenade.transform.localRotation = grenadePrefab.transform.localRotation;
		currentGrenade.transform.localScale = grenadePrefab.transform.localScale;
		audioSource = currentGrenade.GetComponent<AudioSource> ();
		grenadeRigidbody = currentGrenade.GetComponent<Rigidbody> ();
		grenadeTransform = currentGrenade.GetComponent<Transform>();
		mParentTransform = grenadeTransform.parent;
		originalPosition = grenadeTransform.localPosition;
		originalRotation = grenadeTransform.rotation;
	}
}
