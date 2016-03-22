using UnityEngine;
using System.Collections;

[System.Serializable]
public class ZombieWave : MonoBehaviour {
	public GameObject[] zombies;
	public SpawnPoint[] spawnPoints;
	public bool constrainData = true;
	public float delayTime;
	public float spawnRate;
	public int spawnNum;
	public float waveDuration;

	float timer;
	bool isDone = false;
	public bool IsDone {
		get {
			return isDone;
		}
		set {
			isDone = value;
		}
	}

	bool active = false;

	public void Active () {
		timer = waveDuration + delayTime;
		if (constrainData && zombies.Length != spawnPoints.Length) {
			isDone = true;
			return;
		}
		StartCoroutine(Spawn());
		active = true;
	}

	void Update () {
		if (isDone || !active)
			return;
		if (timer > 0) {
			timer -= Time.deltaTime;
		} else {
			isDone = true;
		}
	}

	IEnumerator Spawn () {
		int currentSpawnPoint = 0;
		for(int i=0; i<spawnNum; i++) {
			if(currentSpawnPoint >= spawnPoints.Length)
				currentSpawnPoint = 0;
			if(constrainData) {
				Instantiate(zombies[currentSpawnPoint], spawnPoints[currentSpawnPoint].Position, spawnPoints[currentSpawnPoint].Rotation);
			}
			else {
				Instantiate(zombies[Random.Range(0, zombies.Length)], spawnPoints[currentSpawnPoint].Position, spawnPoints[currentSpawnPoint].Rotation);
			}
			currentSpawnPoint++;
			yield return new WaitForSeconds(spawnRate);
		}
	}
}
