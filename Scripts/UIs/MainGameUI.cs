
using UnuGames.MVVM.ViewModel;

// This code is generated automatically by UIMan ViewModelGerenrator, please do not modify!

public partial class MainGameUI : UIManScreen {


	string _waveNumInfo = "";
	[UIManProperty]
	public string WaveNumInfo {
		get { return _waveNumInfo; }
		set { _waveNumInfo = value; OnPropertyChanged(); }
	}

}
