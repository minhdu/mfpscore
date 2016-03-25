using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CRUD<T> where T : ModelSerializer, new () {

	public T Find (int id) {
		string tableName = EKRegex.ModelNameNormalizer(typeof(T).ToString());
		Dictionary<string, object> record = Storages.Select(tableName, id);
		T result = new T();
		result.Deserialize(record);
		return result;
	}

	public List<T> GetAll () {
		string tableName = EKRegex.ModelNameNormalizer(typeof(T).ToString());

		List<Dictionary<string, object>> records = Storages.Select(tableName);

		List<T> results = new List<T>();
		foreach(Dictionary<string, object> record in records) {
			T result = new T();
			result.Deserialize(record);
			results.Add(result);
		}
	
		return results;
	}

	public bool Insert (T obj) {
		string tableName = EKRegex.ModelNameNormalizer(typeof(T).ToString());
		return Storages.Insert(tableName, obj.Serialize());
	}

	public bool Update(T obj) {
		string tableName = EKRegex.ModelNameNormalizer(typeof(T).ToString());
		return Storages.Update(tableName, obj.GetID(), obj.Serialize());
	}

	public bool Remove (T obj) {
		string tableName = EKRegex.ModelNameNormalizer(typeof(T).ToString());
		return Storages.Remove(tableName, obj.GetID());
	}

	public bool Remove (int id) {
		string tableName = EKRegex.ModelNameNormalizer(typeof(T).ToString());
		return Storages.Remove(tableName, id);
	}
}
