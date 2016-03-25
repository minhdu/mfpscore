using UnityEngine;

public class WaveGame : Singleton<WaveGame> {
	
	public ZombieWave[] wavesData;
	public bool killAllForNextWave;
	int curWaveIndex = -1;
	ZombieWave curWave;

	void FixedUpdate () {
		if ((curWave == null || (curWave != null && curWave.IsDone)) && curWaveIndex < wavesData.Length - 1) {
			curWaveIndex++;
			curWave = wavesData [curWaveIndex];
			curWave.Active ();
			MainGameUI.Instance.WaveNumInfo = "Wave: " + (curWaveIndex+1).ToString() + "/" + wavesData.Length;
		} else if(curWaveIndex >= wavesData.Length){
			EventDispatcher.TriggerEvent(GameEvents.GameStateEvents.END_GAME);
		}
	}
}
