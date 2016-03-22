using UnityEngine;

public class WaveGame : MonoBehaviour {
	public ZombieWave[] wavesData;
	int curWaveIndex = -1;
	ZombieWave curWave;

	void FixedUpdate () {
		if ((curWave == null || (curWave != null && curWave.IsDone)) && curWaveIndex < wavesData.Length-1) {
			curWaveIndex++;
			curWave = wavesData[curWaveIndex];
			curWave.Active();
		}
	}
}
