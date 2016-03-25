using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserinfoService{

	static CRUD<Userinfo> user = new CRUD<Userinfo> ();

	static public Userinfo GetUserInfo(int id)
	{
		//Storages.OpenDB();
		return user.Find (id);
	}
	
	static public List<Userinfo> GetAll()
	{
		//Storages.OpenDB();
		return user.GetAll();
	}

	static public void Update(Userinfo u){
		user.Update (u);
	}
}
