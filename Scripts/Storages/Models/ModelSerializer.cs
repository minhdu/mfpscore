using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ModelSerializer {
	public abstract int GetID ();
	public abstract string Serialize ();
	public abstract void Deserialize (Dictionary<string, object> dict);
}
