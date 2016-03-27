using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class Userinfo : ModelSerializer {

	public int Id {get; set;}
	public int Stage {get; set;}
	public int HighScore {get; set;}

	public override int GetID () {
		return Id;
	}
	
	public override string Serialize ()
	{
		Dictionary<string, object> dict = new Dictionary<string, object>();

		dict.Add ("id", Id);
		dict.Add ("stage", Stage);
		dict.Add ("highscore", HighScore);

		return Json.Serialize(dict);
	}
	
	public override void Deserialize (Dictionary<string, object> dict)
	{
		Id = Parser.GetInt (dict, "id");
		Stage = Parser.GetInt (dict, "stage");
		HighScore = Parser.GetInt (dict, "highscore");
	}
}
