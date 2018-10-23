using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for the game loop.
/// </summary>
public class GameManager : MonoBehaviour
{

    /// <summary>
    /// Location of database on disk.
    /// </summary>
    public string filepath = "Databases/US_Database.xml";
    /// <summary>
    /// Path to save database on disk.
    /// </summary>
    public string savepath = "Saves/test save.xml";

    public DatabaseHandler DBHandler;
    public TimeManager TimeMgr;
    public EventManager EventMgr;
    

    void Start ()
    {

        InitializeGameLogic();
        //EventMgr.GenerateLeagueSchedule(DBHandler.mainDB.LeagueDB[0]);
        DBHandler.SaveMainDB(savepath);
        
	}
	
    /// <summary>
    /// Game loop.
    /// </summary>
	void Update ()
    {

    }

    void InitializeGameLogic()
    {
        // Create the DatabaseHandler
        DBHandler = new DatabaseHandler();
        // Load the database 
        DBHandler.LoadMainDB(filepath);
        // Create the TimeManager
        TimeMgr = new TimeManager();
        //Initialize the TimeManager
        TimeMgr.InitializeTimeManager(DBHandler.mainDB);
        // Start Time
        StartCoroutine(TimeMgr.UpdateTime());
        // Create the EventManager
        EventMgr = new EventManager(DBHandler.mainDB);

    }




}
