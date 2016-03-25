using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoragesManager : Singleton<StoragesManager> {

	private Dictionary<string, Dictionary<int, Dictionary<string, object>>> dbCahes;

	public void Awake(){
		Storages.OpenDB ();
		Instance = this;
	}

	public bool Exists (string tableName, int recordId) {
		if(dbCahes.ContainsKey(tableName)) {
			if(dbCahes[tableName].ContainsKey(recordId))
				return true;
		}

		return false;
	}

	public Dictionary<string, object> TryGetRecord (string tableName, int recordId) {

		if(dbCahes == null)
			dbCahes = new Dictionary<string, Dictionary<int, Dictionary<string, object>>>();

		Dictionary<int, Dictionary<string, object>> table = new Dictionary<int, Dictionary<string, object>>();
		if(dbCahes.TryGetValue(tableName, out table)) {
			Dictionary<string, object> result = new Dictionary<string, object>();
			if(table.TryGetValue(recordId, out result))
				return result;
		}

		return null;
	}

	public List<Dictionary<string, object>> TryGetTable (string tableName) {

		if(dbCahes == null)
			dbCahes = new Dictionary<string, Dictionary<int, Dictionary<string, object>>>();

		Dictionary<int, Dictionary<string, object>> table = new Dictionary<int, Dictionary<string, object>>();
		if(dbCahes.TryGetValue(tableName, out table)) {
			List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();
			foreach(KeyValuePair<int, Dictionary<string, object>> ent in table) {
				results.Add(ent.Value);
			}

			return results;
		}
		
		return null;
	}

	public void CacheRecord (string tableName, int id, Dictionary<string, object> data) {

		if(dbCahes == null)
			dbCahes = new Dictionary<string, Dictionary<int, Dictionary<string, object>>>();

		if(dbCahes.ContainsKey(tableName)) {
			(dbCahes[tableName])[id] = data;
		}
		else {
			dbCahes.Add(tableName, new Dictionary<int, Dictionary<string, object>>());
			(dbCahes[tableName]).Add(id, data);
		}
	}

	public void CacheTable (string tableName, List<Dictionary<string, object>> data) {

		if(dbCahes == null)
			dbCahes = new Dictionary<string, Dictionary<int, Dictionary<string, object>>>();

		foreach(Dictionary<string, object> record in data) {
			int id = Parser.GetInt(record, "id");
			CacheRecord(tableName, id, record);
		}
	}

	public void Remove (string tableName, int id) {
		try {
			(dbCahes[tableName]).Remove(id);
		}
		catch {}
	}

	public void OnApplicationQuit () {
		Storages.CloseDB();
	}
}
