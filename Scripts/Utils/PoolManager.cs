using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PoolTag {
	BULLET_HOLE = 0,
	ZOMBIE
}

public class PoolManager : Singleton<PoolManager> {

	Dictionary<PoolTag, List<GameObject>> caches = new Dictionary<PoolTag, List<GameObject>>();

	Vector3 hidePos = new Vector3(1000,0,0);
	public string[] resources;

	public void InitPool (PoolTag tag, int amount) {
		List<GameObject> objs = new List<GameObject> ();
		GameObject prefab = RescourcesFactory.Load<GameObject> (resources[(int)tag]);
		for (int i=0; i<amount; i++) {
			objs.Add(Instantiate(prefab, hidePos, Quaternion.identity) as GameObject);
		}

		caches.Add (tag, objs);
	}

	public void Release (GameObject obj, PoolTag tag) {
		
	}

	public GameObject Get (PoolTag tag) {
		return null;
	}
}
