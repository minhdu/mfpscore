using UnityEngine;
using System.Collections;

public enum PoolTag {
	NONE,
	BULLET_HOLE
}

public class PoolManager : Singleton<PoolManager> {

	public void Release (GameObject obj, PoolTag tag) {
		
	}

	public GameObject Get (PoolTag tag) {
		return null;
	}
}
