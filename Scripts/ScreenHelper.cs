using UnityEngine;

public static class ScreenHelper {

	static Vector2 screenSize = Vector2.zero;
	public static Vector2 ScreenSize {
		get {
			if(screenSize == Vector2.zero)
				screenSize = new Vector2 (Screen.width, Screen.height);
			return screenSize;
		}
	}

	static Vector2 halfScreenSize = Vector2.zero;
	public static Vector2 HalfScreenSize {
		get {
			if (halfScreenSize == Vector2.zero)
				halfScreenSize = new Vector2 (ScreenSize.x / 2, ScreenSize.y / 2);
			return halfScreenSize;
		}
	}

	static float dpi = -1.0f;
	public static float DPI {
		get {
			if (dpi < 0.0f)
				dpi = Screen.dpi / 100;
			return dpi;
		}
	}
}
