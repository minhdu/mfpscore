﻿using UnityEngine;
using System.Collections;
using System;

public class CoroutinableMono : MonoBehaviour {
	public void RunCoroutine (IEnumerator coroutine, Action callback) {
		StartCoroutine(RunCoroutineThenCallback (coroutine, callback));
	}

	public void RunCoroutine (string coroutine, Action callback) {
		StartCoroutine(RunCoroutineThenCallback (coroutine, callback));
	}

	IEnumerator RunCoroutineThenCallback (IEnumerator coroutine, Action callback) {
		yield return StartCoroutine (coroutine);
		if (callback != null)
			callback ();
	}

	IEnumerator RunCoroutineThenCallback (string coroutine, Action callback) {
		yield return StartCoroutine (coroutine);
		if (callback != null)
			callback ();
	}
}