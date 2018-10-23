using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class DatabaseHandler
{

    public MainDatabase mainDB;

    //private void Start()
    //{
    //    LoadMainDB();
    //}

    public void LoadMainDB(string filepath)
    {
        mainDB = MainDatabase.Load(Path.Combine(Application.dataPath, filepath));

        //for (int i = 0; i < mainDB.CityDB.Count; i++)
        //{
        //    Debug.Log("Checking CityDB Entry #" + i + "...");
        //    Debug.Log(mainDB.CityDB[i].name);
        //    Debug.Log(mainDB.CityDB[i].state);
        //    Debug.Log(mainDB.CityDB[i].country);
        //    Debug.Log(mainDB.CityDB[i].population);
        //    Debug.Log(mainDB.CityDB[i].climate);
        //    Debug.Log("Success!");
        //}
        //Debug.Log("CityDB check complete!");

        //for (int i = 0; i < mainDB.TeamDB.Count; i++)
        //{
        //    Debug.Log("Checking TeamDB Entry #" + i + "...");
        //    Debug.Log(mainDB.TeamDB[i].name);
        //    Debug.Log(mainDB.TeamDB[i].sport);
        //    Debug.Log(mainDB.TeamDB[i].stadium);
        //    Debug.Log(mainDB.TeamDB[i].city);
        //    Debug.Log(mainDB.TeamDB[i].fanbase);
        //    Debug.Log("Success!");
        //}
        //Debug.Log("TeamDB check complete!");

        //Debug.Log(mainDB.StadiumDB.Count);
        //for (int i = 0; i < mainDB.StadiumDB.Count; i++)
        //{
        //    Debug.Log("Checking StadiumDB Entry #" + i + "...");
        //    Debug.Log(mainDB.StadiumDB[i].name);
        //    Debug.Log(mainDB.StadiumDB[i].city);
        //    Debug.Log(mainDB.StadiumDB[i].owner);
        //    Debug.Log(mainDB.StadiumDB[i].capacity);
        //    Debug.Log(mainDB.StadiumDB[i].staff.ticket);
        //    Debug.Log(mainDB.StadiumDB[i].staff.security);
        //    Debug.Log(mainDB.StadiumDB[i].staff.grounds);
        //    Debug.Log("Success!");
        //}
        //Debug.Log("StadiumDB check complete!");

        

    }


    public void SaveMainDB(string savepath)
    {

        // Set save time to current system time.
        mainDB.gameInfoDB[0].savetime = System.DateTime.Now.ToString();
        // Serialise database to XML using provided save path. 
        mainDB.Save(Path.Combine(Application.dataPath, savepath));

    }



}
