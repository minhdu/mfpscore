using UnityEngine;
using UnityEngine.UI;
using UnuGames.MVVM.ViewModel;

public partial class MainGameUI : UIManScreen {
	
	static MainGameUI _instance;
	static public MainGameUI Instance {
		get {
			return _instance;
		}
	}

	public Text text;

	void Awake () {
		_instance = this;
	}

	public void SetHeadShot () {
		LeanTween.cancel (text.gameObject);
		text.color = Color.black;
		LeanTween.alphaText (text.rectTransform, 0, 1f);
	}

#region Fields

	// Your fields here
#endregion

#region Built-in Events
	public override void OnShow (params object[] args)
	{
		base.OnShow (args);
	}

	public override void OnShowComplete ()
	{
		base.OnShowComplete ();
	}

	public override void OnHide ()
	{
		base.OnHide ();
	}

	public override void OnHideComplete ()
	{
		base.OnHideComplete ();
	}
#endregion

#region Custom implementation

	// Your custom code here
#endregion

#region Override animations
	/* Uncommend this for override show/hide animation of Screen/Dialog use tweening code
	public override IEnumerator AnimationShow ()
	{
		return base.AnimationShow ();
	}

	public override IEnumerator AnimationHide ()
	{
		return base.AnimationHide ();
	}
	*/
#endregion
}
