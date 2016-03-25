using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class EKRegex : MonoBehaviour {

	static public string ModelNameNormalizer (string org) {
		string result = "";

		foreach(char c in org) {
			if(Regex.IsMatch(c.ToString(), "[A-Z]"))
				result += "_" + c.ToString().ToLower();
			else
				result += c.ToString();
		}

		result = result.Substring(1, result.Length-1);
		return result;
	}

}
