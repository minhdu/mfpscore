using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;
using MiniJSON;

public class Storages {

	public enum DatabaseType {
		QUESTION,
		DATA
	}

	public const float TIME_OUT = 10f;
    public const string DATA_DB_FILE = "ads.google.dll";
	
	static private IDbConnection dbcon;

	static public void OpenDB ()
    {

		CloseDB ();

        string dbFilePath = "";
        string dbName = "";

        dbName = DATA_DB_FILE;
        
        dbFilePath = Application.persistentDataPath + "/" + dbName;
        
		if (!System.IO.File.Exists(dbFilePath)) {

            float startTime = Time.realtimeSinceStartup;
            
            WWW loadDB = null;
            
            #if UNITY_EDITOR
            loadDB = new WWW("file://" + Application.streamingAssetsPath + "/" + dbName);
            #elif UNITY_ANDROID
            loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/" + dbName);
            #elif UNITY_IPHONE
            loadDB = new WWW("file://" + Application.dataPath + "/Raw/" + dbName);
            #endif


            while (!loadDB.isDone) 
            {
                if(Time.realtimeSinceStartup - startTime > TIME_OUT)
                    return;
            }
            
            File.WriteAllBytes(dbFilePath, loadDB.bytes);
        }
        
        //Close current DB
        CloseDB();

		//Create connection
		string connectionString = "URI=file:" + dbFilePath;
		dbcon = (IDbConnection) new SqliteConnection(connectionString);
		dbcon.Open();
	}

	static public void CloseDB () 
    {
		try 
        {
			dbcon.Close();
			dbcon = null;
		}
        catch {}
	}

	static public Dictionary<string, object> Select (string tableName, int id) {

		Dictionary<string, object> result = StoragesManager.Instance.TryGetRecord(tableName, id);
		if(result != null)
			return result;

		result = new Dictionary<string, object>();
		string query = "SELECT * FROM " + tableName + " WHERE id=" + id;
		IDbCommand dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText = query;
		IDataReader reader = dbcmd.ExecuteReader();

		if(reader.Read()) {
			string val = reader.GetString(1);
			result = Json.Deserialize(val) as Dictionary<string, object>;
			reader.Close();
			reader = null;
			dbcmd.Dispose();
			dbcmd = null;
			StoragesManager.Instance.CacheRecord(tableName, id, result);
			return result;				
		}

		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		Debug.LogError("Cannot find any record with id=" + id + " in table " + tableName);
		return null;
	}

	static public List<Dictionary<string, object>> Select (string tableName) {

		List<Dictionary<string, object>> result = StoragesManager.Instance.TryGetTable(tableName);
		if(result != null)
			return result;

		result = new List<Dictionary<string, object>>();
		string query = "SELECT * FROM " + tableName;
		IDbCommand dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText = query;
		IDataReader reader = dbcmd.ExecuteReader();

		while(reader.Read()) {
			string val = reader.GetString(1);
			Dictionary<string, object> record = Json.Deserialize(val) as Dictionary<string, object>;
			result.Add(record);
		}

		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		StoragesManager.Instance.CacheTable(tableName, result);
		return result;
	}

	static public bool Insert (string tableName, string value) {

		string query = "INSERT INTO " + tableName + "(value) VALUES('" + value + "')";
		IDbCommand dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText = query;

		try {
			dbcmd.ExecuteNonQuery();

			try {
				dbcmd.CommandText = "SELECT last_insert_rowid()";
				int insertedId = int.Parse(dbcmd.ExecuteScalar().ToString());
				Dictionary<string, object> data = Json.Deserialize(value) as Dictionary<string, object>;
				StoragesManager.Instance.CacheRecord(tableName, insertedId, data);
			}catch {}

			dbcmd.Dispose();
			dbcmd = null;
			return true;
		}
		catch (Exception e){
			dbcmd.Dispose();
			dbcmd = null;
			Debug.LogError("Cannot insert into " + tableName + " error " + e);
			return false;
		}
	}

	static public bool Update (string tableName, int id, string value) {
		
		string query = "UPDATE " + tableName + " SET value='" + value + "' WHERE id=" + id;
		IDbCommand dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText = query;
		
		try {
			dbcmd.ExecuteNonQuery();
			dbcmd.Dispose();
			dbcmd = null;
			Dictionary<string, object> data = Json.Deserialize(value) as Dictionary<string, object>;
			StoragesManager.Instance.CacheRecord(tableName, id, data);

			return true;
		}
		catch (Exception e){
			dbcmd.Dispose();
			dbcmd = null;
			Debug.LogError("Cannot update " + id.ToString() + " into " + tableName + " error " + e);
			return false;
		}
	}

	static public bool Remove (string tableName, int id) {
		
		string query = "DELETE FROM " + tableName + " WHERE id=" + id;
		IDbCommand dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText = query;
		
		try {
			dbcmd.ExecuteNonQuery();
			dbcmd.Dispose();
			dbcmd = null;
			StoragesManager.Instance.Remove(tableName, id);
			return true;
		}
		catch (Exception e){
			dbcmd.Dispose();
			dbcmd = null;
			Debug.LogError("Cannot delete " + id + " from " + tableName + " error " + e);
			return false;
		}
	}

//
//	public void FecthUserData () {
//
//		IDbCommand dbcmd = dbcon.CreateCommand();
//		IDataReader reader = null;
//
//		User.Current.PassedQuestion = new int[15];
//		for(int qIndex=1; qIndex<=15; qIndex++) {
//			
//			dbcmd.CommandText = "SELECT Pass FROM Manager WHERE _id=" + qIndex.ToString();
//			reader = dbcmd.ExecuteReader();
//			reader = reader.GetData(0);
//			User.Current.PassedQuestion[qIndex-1] = reader.GetInt32(0);
//		}
//
//
//
//		reader.Close();
//		reader = null;
//		dbcmd.Dispose();
//		dbcmd = null;
//	}
//
//	public List<Question> Query () {
//
//		if(dbcon == null) {
//			//TODO: error message here
//			return null;
//		}
//
//		List<Question> questions = new List<Question>();
//
//		//Execute sql command
//		IDbCommand dbcmd = dbcon.CreateCommand();
//		IDataReader reader = null;
//
//		for(int qIndex=1; qIndex<=15; qIndex++) {
//
//			string questionToLoad = (User.Current.PassedQuestion[qIndex-1] + 1).ToString();
//
//			dbcmd.CommandText = "SELECT * FROM Question" + qIndex.ToString() + "WHERE _id=" + questionToLoad;
//			reader = dbcmd.ExecuteReader();
//			reader = reader.GetData(0);
//			Question q = new Question(
//				reader.GetString(1), //Question
//				reader.GetString(2), //Case A
//				reader.GetString(3), //Case B
//				reader.GetString(4), //Case C
//				reader.GetString(5), //Case D
//				reader.GetInt32(6)); //True case
//		}
//		
//		reader.Close();
//		reader = null;
//		dbcmd.Dispose();
//		dbcmd = null;
//
//		return questions;
//	}
}
