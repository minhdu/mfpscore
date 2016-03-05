using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
	static T _instance;
	static public T Instance {
		get {
			if (_instance == null)
			{
				_instance = (T)FindObjectOfType(typeof(T));
			}
			return _instance;
		}
		set {
			_instance = value;
		}
	}

	void Awake () {
		if(_instance != null && this != _instance) {
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
	}

	void OnDestroy () {
		Destroy(gameObject);
	}
}