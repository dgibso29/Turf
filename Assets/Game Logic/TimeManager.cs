using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles all in-game time functionality.
/// </summary>
public class TimeManager
{

    /// <summary>
    /// Current date in-game (Time & Date)
    /// </summary>
    public static DateTime gameDate;

    float timeScale = 1;

    Boolean paused = false;

    /// <summary>
    /// Set up Time Manager on load.
    /// </summary>
    /// <param name="mainDB"></param>
    public void InitializeTimeManager(MainDatabase mainDB)
    {
        // Set game time to time from save.
        gameDate = mainDB.gameInfoDB[0].GetDateTime();


        // Time debug code
        //Debug.Log(gameTime);
        //string saveDate = gameTime.ToString();
        //mainDB.gameInfoDB[0].date = saveDate;
        //Debug.Log(saveDate);

    }

    public IEnumerator UpdateTime()
    {
        while (!paused) {
            yield return new WaitForSecondsRealtime(1f * timeScale);
            IncrementTimeByOneSecond();
        }

    }

    public void ChangeGameSpeed(float newSpeed)
    {
        timeScale = newSpeed;
        Time.timeScale = newSpeed;
        if(newSpeed == 0)
        {
            paused = true;
        }
        else
        {
            paused = false;
        }
    }

    void IncrementTimeByOneSecond()
    {
        gameDate = gameDate.AddMinutes(1);
        Debug.Log(gameDate);
    }

}
