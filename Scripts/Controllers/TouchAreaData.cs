using UnityEngine;

public class TouchAreaData{
	public bool FingerDown { get; set; }
	public Rect FingerBound { get; set; }
	public int FingerId { get; set; }

	public TouchAreaData () {
		FingerDown = false;
		FingerId = -1;
	}
}