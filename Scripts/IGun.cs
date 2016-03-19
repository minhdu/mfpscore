using UnityEngine;
using System.Collections;

public interface IGun {

	bool IsAiming ();
	void DoShoot ();
	void StopShoot ();
	void DoReload ();
	void DoAim ();
}
