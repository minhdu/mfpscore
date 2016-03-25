using UnityEngine;
using System.Collections;

[System.Serializable]
public class ZombieWave : MonoBehaviour, IEventListener {
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
	int curNum = 0;

	public void ListenEvents () {
		EventDispatcher.AddEventListener (GameEvents.GameplayEvents.ZOMBIE_DEAD, OnAnyZombieDead);
	}

	public void OnAnyZombieDead () {
		if(active)
			curNum--;
	}

	public void Active () {
		timer = waveDuration + delayTime;
		if (constrainData && zombies.Length != spawnPoints.Length) {
			isDone = true;
			return;
		}
		curNum = spawnNum;
		StartCoroutine (Spawn ());
		active = true;
		ListenEvents ();
	}

	void Update () {
		if (isDone || !active)
			return;
		if (timer > 0) {
			timer -= Time.deltaTime;
		} else {
			if ((curNum <= 0 && WaveGame.Instance.killAllForNextWave) || !WaveGame.Instance.killAllForNextWave) {
				EventDispatcher.RemoveEventListener (GameEvents.GameplayEvents.ZOMBIE_DEAD, OnAnyZombieDead);
				isDone = true;
			}
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
