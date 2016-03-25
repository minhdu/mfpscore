using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainGameUI : Singleton<MainGameUI> {

	public Text text; 

	public void SetHeadShot () {
		LeanTween.cancel (text.gameObject);
		text.color = Color.black;
		LeanTween.alphaText (text.rectTransform, 0, 1f);
	}
}
