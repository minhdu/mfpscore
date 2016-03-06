using UnityEngine;
using System.Collections;

public class CrossHair : Singleton<CrossHair> {

	public float maxZoom;
	public RectTransform[] lines;
	Vector3[] originalPosition;
	float zoomFactor = 0;
	float increaseFactor = 0.02f;

	void Awake () {
		originalPosition = new Vector3[lines.Length];
		for (int i = 0; i < lines.Length; i++) {
			originalPosition [i] = lines [i].localPosition;
		}
	}

	public void Zoom () {
		increaseFactor += 0.005f;
		zoomFactor += increaseFactor;
		if (lines[1].localPosition.x + zoomFactor > maxZoom)
			zoomFactor = maxZoom - lines[1].localPosition.x;
		LeanTween.moveLocal(lines[0].gameObject, new Vector3(lines[0].localPosition.x-zoomFactor, lines[0].localPosition.y, 0), 0.01f);
		LeanTween.moveLocal(lines[1].gameObject, new Vector3(lines[1].localPosition.x+zoomFactor, lines[1].localPosition.y, 0), 0.01f);
		LeanTween.moveLocal(lines[2].gameObject, new Vector3(lines[2].localPosition.x, lines[2].localPosition.y+zoomFactor, 0), 0.01f);
		LeanTween.moveLocal(lines[3].gameObject, new Vector3(lines[3].localPosition.x, lines[3].localPosition.y-zoomFactor, 0), 0.01f);
	}

	public void Reverts () {
		Debug.Log (zoomFactor);
		for (int i = 0; i < lines.Length; i++) {
			LeanTween.moveLocal(lines[i].gameObject, originalPosition[i], 0.25f-zoomFactor/40f).setOnComplete(ResetFactor);
		}
	}

	void ResetFactor () {
		zoomFactor = 0;
		increaseFactor = 0.02f;
	}
}
