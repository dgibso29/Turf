using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager
{

    MainDatabase mainDB;


    /// <summary>
    /// List of all games in the season being created.
    /// </summary>
    List<MainDatabase.Game> currentSeasonGames;
    public EventManager(MainDatabase mainDB)
    {
        this.mainDB = mainDB;
        
    }
    
    public void MLSEventTest()
    {
        List<string> mainDays = new List<string>(3);
        mainDays.Add("Friday");
        mainDays.Add("Saturday");
        mainDays.Add("Sunday");
        List<string> offDays = new List<string>(1);
        offDays.Add("Wednesday");
        offDays.Add("Tuesday");
        List<int> weekendStartTimes = new List<int>(5);
        weekendStartTimes.Add(13);
        weekendStartTimes.Add(16);
        weekendStartTimes.Add(18);
        weekendStartTimes.Add(19);
        weekendStartTimes.Add(20);
        List<int> weekdayStartTimes = new List<int>(2);
        weekdayStartTimes.Add(19);
        weekdayStartTimes.Add(20);
        int stadiumPrepTime = 120;
        int regulationDuration = 120;
        int overtimeDuration = 0;
        int stadiumBreakdownTime = 60;
        mainDB.LeagueDB[0].leagueHistory = new List<MainDatabase.Season>();
        mainDB.LeagueDB[0].SetLeagueRules(2, true, false, true, mainDays, offDays, weekendStartTimes, weekdayStartTimes, stadiumPrepTime, regulationDuration, overtimeDuration, stadiumBreakdownTime, 3, 3);

    }

    #region Notes

    // Create the new season
    // Find all teams in the league and add them to a list
    // Begin iterating through those teams
    // Iteration loop:
    // Create a temporary list of all teams and remove the current team from it
    // Iterate through the temp list
    // Having chosen the first team, choose a date after the start date that is on an allowed day, and set a time after the allowedStartTime (this variable needs to be added)
    // Create an event with this time, adding each team. Choose between the two teams stadiums at random. If second time around, ensure game is opposite of first (one home, one away)
    // Make sure event has all needed info (Teams, Date & Time, Stadium) and add it to the event DB

    // Addtl: Loop through as many times as needed to ensure every team plays every other team the mandated number (2 for MLS)
    // Addtl: Check against existing events to ensure teams are not scheduled when they already have games
    // Addtl: Add variable to league rules for minimum time between games, and max time (and mandated break after tightly scheduled matches?). Check against eventDB to ensure new games respect these rules
    // Addtl: Games should only be scheduled tightly if necessary (check against EventDB for scheduling conflicts).

    // DO THIS FIRST: Create separate function for scheduling the actual games, so that matches can be rescheduled if needed. AKA iterate here, but call the separate function to actually schedule the match.
    // DO THIS NEXT: Separate function to sort through teams in DB by League (PUT THIS IN MAINDATABASE AND PROVIDE OVERLOADS FOR OTHER SEARCH VARIABLES -- LEAGUE, STADIUM, COUNTRY, SPORT, ETC
    

    // KEEP LIST OF USED DATES TO IMPROVE PROCESSING TIME?
    // Make it a class variable. Reset it when League Generation is called. Add to it when games are created.
    // Need to track per team...? Maybe check teamavailability first? Pre-emptively block out dates when the team can't play? AKA Create a list of blocked dates at the start.

    // CHECK IF TEAMS ARE AVAILABLE FIRST!

    #endregion
        /// <summary>
        /// Generate the schedule of games for the provided league's next season. Year of next season will be determined by most recent season
        /// OR current year if no previous seasons (for new leagues, etc).
        /// </summary>
        /// <param name="league">League to generate schedule for.</param>
    public void GenerateLeagueSchedule(MainDatabase.League league)
    {
        // Declare variables
        List<MainDatabase.Team> teamsInLeague = new List<MainDatabase.Team>();

        // Determine viable game days and start times for the league, and set those references now.
        // These variables will be passed through to the game generation function -- saving on performance by doing here.

        // CHOOSE POSSIBLE DAYS FOR EACH WEEK
        // Choose weekends first, then try for off day games if needed. Otherwise skip. If two games in one week (Off and main), skip the next week entirely (from Week 1 to Week 3).

        // Create the lists off on and off days to check against
        List<DayOfWeek> mainDays = new List<DayOfWeek>();
        List<DayOfWeek> offDays = new List<DayOfWeek>();

        // Main days
        foreach (string d in league.rules.mainDays)
        {
            mainDays.Add(mainDB.StringToDayOfWeek(d));
        }
        // Off days
        foreach (string d in league.rules.offDays)
        {

            offDays.Add(mainDB.StringToDayOfWeek(d));
        }

        // Create the lists of weekend and weekday start times to check against
        List<int> weekendStartTimes = league.rules.weekendStartTimes;
        List<int> weekdayStartTimes = league.rules.weekdayStartTimes;

        // Create the new season
        MainDatabase.Season newSeason = new MainDatabase.Season();
        // Set it to the proper year
        // Check if there is a preceding season...
        if(league.leagueHistory[league.leagueHistory.Count] != null)
        {
            // And set the new season's year to the next year
            newSeason.year = DateTime.Parse(league.leagueHistory[league.leagueHistory.Count].year).AddYears(1).Year.ToString();
        }
        else
        {
            // Otherwise, set the season's year to the current year
            newSeason.year = TimeManager.gameDate.Year.ToString();
        }
        // Set other season info
        newSeason.league = league.name;
        newSeason.seasonStats = new List<MainDatabase.TeamLeagueStats>();
        // Add the season to the league
        mainDB.LeagueDB[mainDB.LeagueDB.FindIndex(l => l.name == league.name)].leagueHistory.Add(newSeason);

        // Generate the games!
        Debug.Log("Attempting " + league.name + " " + newSeason.year + " season regular schedule generation at " + DateTime.Now.ToLongTimeString() + "!");
        // Add all the games in the league to a list
        foreach(MainDatabase.Team t in mainDB.TeamDB)
        {
            if(t.league == league.name){
                teamsInLeague.Add(t);
            }
        }
        // Shuffle the list of teams to ensure the schedule is.. well, shuffled.
        ListHelper.Shuffle(teamsInLeague);

        // For each team in the list, generate a temporary list of all teams, remove the current team, and create those games!
        foreach(MainDatabase.Team t in teamsInLeague)
        {
            // Copy the teamsInLeague list...
             List<MainDatabase.Team> otherTeams = new List<MainDatabase.Team>(teamsInLeague);
            // Remove the current team
            otherTeams.Remove(t);
            // Generate a game between the current team and each remaining team!
            foreach(MainDatabase.Team other in otherTeams)
            {
                Debug.Log("Attempting to create " + t.name + " v " + other.name);
                GenerateLeagueGame(league, t, other, newSeason, mainDays, offDays, weekendStartTimes, weekdayStartTimes);
            }
        }
        Debug.Log("Completed game generation at " + DateTime.Now.ToLongTimeString());

    }

    public void GenerateLeagueGame(MainDatabase.League league, MainDatabase.Team teamOne, MainDatabase.Team teamTwo, MainDatabase.Season currentSeason, 
        List<DayOfWeek> mainDays, List<DayOfWeek> offDays, List<int> weekendStartTimes, List<int> weekdayStartTimes)
    {
        // Tl;dr notes for this function:
        // Find valid days -- SOME OF THIS SHOULD GO IN THE SCHEDULE GENERATION FUNCTION TO SAVE ON PROCESSING TIME DUH
        // DUHHHHHHHHHH


        // Check both team's availability for the current week, and keep skipping forward until both are available
        // Determine which team is home/away and assign the venue

        // Once we find a viable week, ensure the stadium has an available time slot. If so, create the game!
        // Otherwise, skip forward one week and go back to checking team availability.
        // Repeat the loop until we find a valid date.

        MainDatabase.Game newGame = null;
        // Declare variables
        string homeTeam;
        string awayTeam;
        string venue;
        // Time game starts (separate of eventDate).
        string gameStart = null;
        int eventDuration = (league.rules.stadiumPrepTime + league.rules.regulationDuration + league.rules.stadiumBreakdownTime);
        // Time and date event prep beings
        string eventDate = null;

        DateTime potentialDate = new DateTime();

        while (newGame == null)
        {
            // Choose a potential date

            // Check if both teams are available
            if(CheckTeamAvailability(teamOne, potentialDate) && CheckTeamAvailability(teamTwo, potentialDate))
            {

            }

            // Find all existing matchups of these teams in this season
            List<MainDatabase.Game> previousMeetings = new List<MainDatabase.Game>();
            foreach (MainDatabase.Game g in mainDB.EventDB)
            {
                if (g != null)
                    if (g.season == currentSeason.year && g.league == league.name && ((g.awayTeam == teamOne.name && g.homeTeam == teamTwo.name)
                        || (g.awayTeam == teamTwo.name && g.homeTeam == teamOne.name)))
                    {
                        previousMeetings.Add(g);
                    }
            }
            // Check how many are away and home for teamOne
            int teamOneHome = 0;
            int teamOneAway = 0;

            if (previousMeetings.Count > 0)
                foreach (MainDatabase.Game g in previousMeetings)
                {
                    if (g.homeTeam == teamOne.name)
                    {
                        teamOneHome++;
                    }
                    else if (g.awayTeam == teamOne.name)
                    {
                        teamOneAway++;
                    }
                }
            // Assign home & away teams
            if (teamOneHome > teamOneAway)
            {
                homeTeam = teamTwo.name;
                awayTeam = teamOne.name;
            }
            else if (teamOneHome < teamOneAway)
            {
                homeTeam = teamOne.name;
                awayTeam = teamTwo.name;
            }
            else
            {
                //homeTeam = teamOne.name;
                //awayTeam = teamTwo.name;
                int random = UnityEngine.Random.Range(0, 2);
                if (random == 0)
                {
                    homeTeam = teamOne.name;
                    awayTeam = teamTwo.name;
                }
                else
                {
                    homeTeam = teamTwo.name;
                    awayTeam = teamOne.name;
                }
            }
            // Assign stadium to home team field
            venue = mainDB.StringToStadium(mainDB.StringToTeam(homeTeam).stadium).name;

            // Temporary break
            break;
        }
    }

    /// <summary>
    /// Return true if team is available to play a game on the provided date
    /// </summary>
    /// <param name="team"></param>
    /// <param name="dateToCheck"></param>
    /// <returns></returns>
    public bool CheckTeamAvailability(MainDatabase.Team team, DateTime dateToCheck)
    {

        // Check if the team is available on the current date
        foreach (MainDatabase.Game g in mainDB.EventDB)
        {
            if (g.GetDateTime().ToShortDateString() == dateToCheck.ToShortDateString() && (mainDB.StringToTeam(g.awayTeam) == team || mainDB.StringToTeam(g.homeTeam) == team))
            {
                //Debug.Log(team.name + " has a game on this day");
                //Debug.Log(potentialDate.ToString());
                return false;
            }
        }

        // Create a list to ensure there are not too many games in a short span of time
        List<DateTime> datesToCheck = new List<DateTime>();
        List<MainDatabase.Game> potentialConflicts = new List<MainDatabase.Game>();
        //int numCheck = 0;
        for (int i = -7; i < 7; i++)
        {
            //numCheck++;
            DateTime dayToCheck = dateToCheck.AddDays(i);
            datesToCheck.Add(dayToCheck);
        }
        //Debug.Log(numCheck);
        // Check each date for other games including this team
        foreach (DateTime date in datesToCheck)
        {
            //Debug.Log(date.ToString());
            foreach (MainDatabase.Game g in mainDB.EventDB)
            {
                if (g.GetDateTime().ToShortDateString() == date.ToShortDateString() && (mainDB.StringToTeam(g.awayTeam) == team || mainDB.StringToTeam(g.homeTeam) == team))
                {
                    //Debug.Log(g.name);
                    //Debug.Log(g.date);
                    potentialConflicts.Add(g);
                }
            }
        }
        // If there cannot be more games in this two week period, return false.
        if (potentialConflicts.Count > mainDB.StringToLeague(team.league).rules.maxGamesInTwoWeeks)
        {
            //Debug.Log("Too many games in two week period for " + team.name + "; " + potentialConflicts.Count);
            //Debug.Log(potentialDate.ToString());
            return false;
        }
        // Check if this game would violate the minimum interval between games
        foreach (MainDatabase.Game g in potentialConflicts)
        {
            DateTime laterGame = g.GetDateTime().AddDays(mainDB.StringToLeague(team.league).rules.minIntervalBetweenGames);
            DateTime earlierGame = g.GetDateTime().AddDays(-(mainDB.StringToLeague(team.league).rules.minIntervalBetweenGames));
            if (laterGame >= dateToCheck && dateToCheck >= earlierGame)
            {
                //Debug.Log("Too many games within minimum interval for " + team.name);
                //Debug.Log(potentialDate.ToString());
                return false;
            }
        }

        //Debug.Log(team.name + " is available!");
        // If we made it here, this date is good to go!
        return true;
    }
}


    #region OLD CODE 
    ///// <summary>
    ///// Generate the schedule of games for the next season.
    ///// </summary>
    ///// <param name="league"></param>
    //public void GenerateLeagueSchedule(MainDatabase.League league)
    //{
    //    MLSEventTest();
    //    // Find all teams in the league and add them to a list
    //    // Begin iterating through those teams
    //    // Iteration loop:
    //    // Create a temporary list of all teams and remove the current team from it
    //    // Iterate through the temp list
    //    // Having chosen the first team, choose a date after the start date that is on an allowed day, and set a time after the allowedStartTime (this variable needs to be added)
    //    // Create an event with this time, adding each team. Choose between the two teams stadiums at random. If second time around, ensure game is opposite of first (one home, one away)
    //    // Make sure event has all needed info (Teams, Date & Time, Stadium) and add it to the event DB

    //    // Addtl: Loop through as many times as needed to ensure every team plays every other team the mandated number (2 for MLS)
    //    // Addtl: Check against existing events to ensure teams are not scheduled when they already have games
    //    // Addtl: Add variable to league rules for minimum time between games, and max time (and mandated break after tightly scheduled matches?). Check against eventDB to ensure new games respect these rules
    //    // Addtl: Games should only be scheduled tightly if necessary (check against EventDB for scheduling conflicts).

    //    // DO THIS FIRST: Create separate function for scheduling the actual games, so that matches can be rescheduled if needed. AKA iterate here, but call the separate function to actually schedule the match.
    //    // DO THIS NEXT: Separate function to sort through teams in DB by League (PUT THIS IN MAINDATABASE AND PROVIDE OVERLOADS FOR OTHER SEARCH VARIABLES -- LEAGUE, STADIUM, COUNTRY, SPORT, ETC

    //    // Create new season
    //    MainDatabase.Season newSeason = new MainDatabase.Season();
    //    newSeason.year = TimeManager.gameDate.Year.ToString();
    //    newSeason.league = league.name;
    //    newSeason.seasonStats = new List<MainDatabase.TeamLeagueStats>();
    //    mainDB.LeagueDB[0].leagueHistory.Add(newSeason);


    //    // Generate the games
    //    Debug.Log("Attempting league generation");
    //    List<MainDatabase.Team> teamsInLeague = new List<MainDatabase.Team>();
    //    foreach (MainDatabase.Team t in mainDB.TeamDB)
    //    {
    //        if (t.league == league.name)
    //        {
    //            teamsInLeague.Add(t);
    //        }
    //    }
    //    ListHelper.Shuffle(teamsInLeague);
    //    Debug.Log("Generating games at " + DateTime.Now.ToLongTimeString());
    //    int iterationTracker = 0;
    //    foreach (MainDatabase.Team t in teamsInLeague)
    //    {
    //        List<MainDatabase.Team> otherTeams = new List<MainDatabase.Team>(teamsInLeague);
    //        otherTeams.Remove(t);
    //        foreach (MainDatabase.Team other in otherTeams)
    //        {
    //            Debug.Log("Attempting to create " + t.name + " v " + other.name);
    //            mainDB.EventDB.Add(GenerateLeagueGame(mainDB.StringToLeague(t.league), t, other, newSeason));
    //        }
    //        iterationTracker++;
    //        //if (iterationTracker == league.teams/2)
    //        //{
    //        //    Debug.Log("STOPPPP");
    //        //    break;
    //        //}
    //    }
    //    Debug.Log("Completed game generation at " + DateTime.Now.ToLongTimeString());
    //}


    //// KEEP LIST OF USED DATES TO IMPROVE PROCESSING TIME?
    //// Make it a class variable. Reset it when League Generation is called. Add to it when games are created.
    //// Need to track per team...? Maybe check teamavailability first? Pre-emptively block out dates when the team can't play? AKA Create a list of blocked dates at the start.

    //// CHECK IF TEAMS ARE AVAILABLE FIRST!


    //public MainDatabase.Game GenerateLeagueGame(MainDatabase.League league, MainDatabase.Team teamOne, MainDatabase.Team teamTwo, MainDatabase.Season currentSeason)
    //{        
    //    MainDatabase.Game newGame = null;
    //    // Declare variables
    //    string homeTeam;
    //    string awayTeam;
    //    string venue;
    //    // Time game starts (separate of eventDate).
    //    string gameStart = null;
    //    int eventDuration = (league.rules.stadiumPrepTime + league.rules.regulationDuration + league.rules.stadiumBreakdownTime);
    //    // Time and date event prep beings
    //    string eventDate = null;

    //    // Find all existing matchups of these teams in this season
    //    List<MainDatabase.Game> previousMeetings = new List<MainDatabase.Game>();
    //    foreach (MainDatabase.Game g in mainDB.EventDB)
    //    {
    //        if(g != null)
    //        if (g.season == currentSeason.year && g.league == league.name && ((g.awayTeam == teamOne.name && g.homeTeam == teamTwo.name) || (g.awayTeam == teamTwo.name && g.homeTeam == teamOne.name)))
    //        {
    //            previousMeetings.Add(g);
    //        }
    //    }
    //    // Check how many are away and home for teamOne
    //    int teamOneHome = 0;
    //    int teamOneAway = 0;

    //    if (previousMeetings.Count > 0)
    //    foreach (MainDatabase.Game g in previousMeetings)
    //    {
    //        if (g.homeTeam == teamOne.name)
    //        {
    //            teamOneHome++;
    //        }
    //        else if (g.awayTeam == teamOne.name)
    //        {
    //            teamOneAway++;
    //        }
    //    }
    //    // Assign home & away teams
    //    if (teamOneHome > teamOneAway)
    //    {
    //        homeTeam = teamTwo.name;
    //        awayTeam = teamOne.name;
    //    }
    //    else if(teamOneHome < teamOneAway)
    //    {
    //        homeTeam = teamOne.name;
    //        awayTeam = teamTwo.name;
    //    }
    //    else
    //    {
    //        //homeTeam = teamOne.name;
    //        //awayTeam = teamTwo.name;
    //        int random = UnityEngine.Random.Range(0, 2);
    //        if (random == 0)
    //        {
    //            homeTeam = teamOne.name;
    //            awayTeam = teamTwo.name;
    //        }
    //        else
    //        {
    //            homeTeam = teamTwo.name;
    //            awayTeam = teamOne.name;
    //        }
    //    }
    //    // Assign stadium to home team field
    //    venue = mainDB.StringToStadium(mainDB.StringToTeam(homeTeam).stadium).name;


    //    // CHOOSE POSSIBLE DAYS FOR EACH WEEK
    //    // Choose weekends first, then try for off day games if needed. Otherwise skip. If two games in one week (Off and main), skip the next week entirely (from Week 1 to Week 3).

    //    // Find date that:
    //    // 1. Is within the season timeframe
    //    // 2. Is within the allowable league dates
    //    // 3. The stadium is available.
    //    // 4. Each Team is available (and has not played a game too recently)

    //    // Create the lists off on and off days to check against
    //    List<DayOfWeek> mainDays = new List<DayOfWeek>();
    //    List<DayOfWeek> offDays = new List<DayOfWeek>();

    //    // Main days
    //    foreach(string d in league.rules.mainDays)
    //    {
    //        mainDays.Add(mainDB.StringToDayOfWeek(d));
    //    }
    //    // Off days
    //    foreach (string d in league.rules.offDays)
    //    {

    //        offDays.Add(mainDB.StringToDayOfWeek(d));
    //    }

    //    // Create the lists of weekend and weekday start times to check against
    //    List<int> weekendStartTimes = league.rules.weekendStartTimes;
    //    List<int> weekdayStartTimes = league.rules.weekdayStartTimes;

    //    DateTime seasonStart = new DateTime(TimeManager.gameDate.Year, DateTime.Parse(league.startDate).Month, DateTime.Parse(league.startDate).Day);
    //    //Make sure we start on a Friday
    //    if (seasonStart.DayOfWeek != DayOfWeek.Friday)
    //    {
    //        bool notFriday = true;
    //        while (notFriday)
    //        {
    //            seasonStart = seasonStart.AddDays(1);
    //            if (seasonStart.DayOfWeek == DayOfWeek.Friday)
    //            {
    //                notFriday = false;
    //                break;
    //            }
    //        }
    //    }
    //    // The Friday of the week we are currently looking at. We will always start on Fridays, and increment forward 1 week at a time as needed.
    //    DateTime currentFriday = seasonStart;
    //    // Container for potential event start time.
    //    DateTime potentialEventDate = seasonStart;
    //    int loopBreaker = 0;
    //    // Start looking for a viable day, always starting on a friday.
    //    while (gameStart == null)
    //    {
    //        if(currentFriday > DateTime.Parse(league.endDate))
    //        {
    //            currentFriday = seasonStart;
    //        }
    //        //Debug.Log("Current Friday at start of loop is " + currentFriday.ToString());
    //        //Debug.Log(currentFriday.ToString());
    //        //Debug.Log("Starting loop");
    //        loopBreaker++;
    //        //Debug.Log("Loop #" + loopBreaker);
    //        if (loopBreaker > 100)
    //        {
    //            Debug.Log("Breaking loop at " + DateTime.Now.ToShortTimeString());
    //            gameStart = "lol";
    //            break;
    //        }
    //        //Debug.Log("Attempting to create a game between " + homeTeam + " and " + awayTeam + " at " + venue);
    //        // Get a list of the next 7 days
    //        List<DateTime> currentWeek = new List<DateTime>();
    //        for (int i = 0; i < 7; i++)
    //        {
    //            DateTime tempDay = currentFriday.AddDays(i);
    //            currentWeek.Add(tempDay);
    //        }
    //        // Look for a possible time based on stadium availability
    //        ListHelper.Shuffle(currentWeek);
    //        foreach (DateTime day in currentWeek)
    //        {
    //            // Makes sure each team is available
    //            if (CheckTeamAvailability(teamOne, day) && CheckTeamAvailability(teamTwo, day))
    //            {
    //                #region Look for Main Days
    //                // Look for Main Days first
    //                if (mainDays.Contains(day.DayOfWeek))
    //                {
    //                    //Debug.Log("Looking for main days");
    //                    // If it is a weekend, use the weekend start times list
    //                    if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
    //                    {
    //                        List<int> possibleStartTimes = weekendStartTimes;
    //                        List<DateTime> timesToTry = new List<DateTime>();
    //                        // Add the possible times to a list
    //                        foreach (int time in possibleStartTimes)
    //                        {
    //                            timesToTry.Add(new DateTime(day.Year, day.Month, day.Day, time, 0, 0));
    //                        }
    //                        // Check each time until there are none to check or we find one with no conflicts.
    //                        ListHelper.Shuffle(timesToTry);
    //                        foreach (DateTime d in timesToTry)
    //                        {
    //                            bool conflict = CheckForStadiumScheduleConflict(mainDB.StringToStadium(venue), d, eventDuration);
    //                            if (!conflict)
    //                            {
    //                                potentialEventDate = d;
    //                                break;
    //                            }
    //                        }
    //                        if (potentialEventDate != seasonStart)
    //                        {
    //                            break;
    //                        }
    //                    }
    //                    // If it is a weekday, use the weekday start times list
    //                    else if (day.DayOfWeek == DayOfWeek.Monday || day.DayOfWeek == DayOfWeek.Tuesday || day.DayOfWeek == DayOfWeek.Wednesday || day.DayOfWeek == DayOfWeek.Thursday || day.DayOfWeek == DayOfWeek.Friday)
    //                    {
    //                        if (day.DayOfWeek == DayOfWeek.Monday)
    //                        {
    //                            //Debug.Log("Fuck, it's Monday");
    //                        }
    //                        List<int> possibleStartTimes = weekdayStartTimes;
    //                        List<DateTime> timesToTry = new List<DateTime>();
    //                        // Add the possible times to a list
    //                        foreach (int time in possibleStartTimes)
    //                        {
    //                            timesToTry.Add(new DateTime(day.Year, day.Month, day.Day, time, 0, 0));
    //                        }
    //                        // Check each time until there are none to check or we find one with no conflicts.
    //                        ListHelper.Shuffle(timesToTry);
    //                        foreach (DateTime d in timesToTry)
    //                        {
    //                            bool conflict = CheckForStadiumScheduleConflict(mainDB.StringToStadium(venue), d, eventDuration);
    //                            if (!conflict)
    //                            {
    //                                potentialEventDate = d;
    //                                //Debug.Log(potentialEventDate);
    //                                //Debug.Log(seasonStart);
    //                                break;
    //                            }
    //                        }
    //                        if (potentialEventDate != seasonStart)
    //                        {
    //                            break;
    //                        }
    //                    }
    //                }
    //                #endregion
    //                #region Look for Off Days
    //                //Then look for off days
    //                if (offDays.Contains(day.DayOfWeek))
    //                {
    //                    //Debug.Log("Looking for off days");

    //                    // If it is a weekend, use the weekend start times list
    //                    if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
    //                    {
    //                        List<int> possibleStartTimes = weekendStartTimes;
    //                        List<DateTime> timesToTry = new List<DateTime>();
    //                        // Add the possible times to a list
    //                        foreach (int time in possibleStartTimes)
    //                        {
    //                            timesToTry.Add(new DateTime(day.Year, day.Month, day.Day, time, 0, 0));
    //                        }
    //                        // Check each time until there are none to check or we find one with no conflicts.
    //                        ListHelper.Shuffle(timesToTry);
    //                        foreach (DateTime d in timesToTry)
    //                        {
    //                            bool conflict = CheckForStadiumScheduleConflict(mainDB.StringToStadium(venue), d.AddHours(-(league.rules.stadiumPrepTime)), eventDuration);
    //                            if (!conflict)
    //                            {
    //                                potentialEventDate = d;
    //                                break;
    //                            }
    //                        }
    //                        if (potentialEventDate != seasonStart)
    //                        {
    //                            break;
    //                        }
    //                    }
    //                    // If it is a weekday, use the weekday start times list
    //                    else if (day.DayOfWeek == DayOfWeek.Monday || day.DayOfWeek == DayOfWeek.Tuesday || day.DayOfWeek == DayOfWeek.Wednesday || day.DayOfWeek == DayOfWeek.Thursday || day.DayOfWeek == DayOfWeek.Friday)
    //                    {
    //                        List<int> possibleStartTimes = weekdayStartTimes;
    //                        List<DateTime> timesToTry = new List<DateTime>();
    //                        // Add the possible times to a list
    //                        foreach (int time in possibleStartTimes)
    //                        {
    //                            timesToTry.Add(new DateTime(day.Year, day.Month, day.Day, time, 0, 0));
    //                        }
    //                        // Check each time until there are none to check or we find one with no conflicts.
    //                        ListHelper.Shuffle(timesToTry);
    //                        foreach (DateTime d in timesToTry)
    //                        {
    //                            bool conflict = CheckForStadiumScheduleConflict(mainDB.StringToStadium(venue), d.AddHours(-(league.rules.stadiumPrepTime)), eventDuration);
    //                            if (!conflict)
    //                            {
    //                                potentialEventDate = d;
    //                                break;
    //                            }
    //                        }
    //                        if (potentialEventDate != seasonStart)
    //                        {
    //                            break;
    //                        }
    //                    }
    //                }
    //                #endregion
    //            }
    //            else
    //            {
    //                //Debug.Log("TEAMS NOT AVAILABLE");
    //                //potentialEventDate = seasonStart;
    //                //currentFriday = currentFriday.AddDays(7);
    //                break;
    //            }

    //        }
    //            // If we do not have a date, advance to the next week and start over.
    //        if (potentialEventDate == seasonStart)
    //        {
    //            //Debug.Log("We fucked, no date");
    //            currentFriday = currentFriday.AddDays(7);
    //        }
    //        else
    //        {
    //            // Otherwise, take the date we have and check for team availability.
    //            bool teamOneAvailable = CheckTeamAvailability(teamOne, potentialEventDate);
    //            bool teamTwoAvailable = CheckTeamAvailability(teamTwo, potentialEventDate);

    //            // If both teams are available, we're good!
    //            if (teamOneAvailable && teamTwoAvailable)
    //            {
    //                eventDate = potentialEventDate.ToString();
    //                newGame = new MainDatabase.Game();
    //                newGame.CreateGame(homeTeam + " v " + awayTeam, potentialEventDate.AddMinutes(-(league.rules.stadiumPrepTime)).ToString(), eventDuration, potentialEventDate.ToString(), league.sport, league.name, currentSeason.year, homeTeam, awayTeam, venue);
    //                Debug.Log(newGame.name + " created succesfully on " + potentialEventDate.ToShortDateString() + " at " + potentialEventDate.ToShortTimeString() + " at " + DateTime.Now.ToLongTimeString());
    //                //lastGameFriday = currentFriday;
    //                return newGame;
    //            }
    //            else
    //            {
    //                //Debug.Log("We fucked, team not available");
    //                //Debug.Log("Current week is " + currentFriday.ToString());
    //                currentFriday = currentFriday.AddDays(7);
    //                //Debug.Log(currentFriday.ToString());
    //            }
    //        }

    //    }          


    //    //Debug.Log("Hopefully a game was made");
    //    return newGame;
    //}

    ///// <summary>
    ///// If returned true, there is a conflict.
    ///// </summary>
    ///// <param name="venue"></param>
    ///// <param name="potentialDate"></param>
    ///// <param name="eventDuration"></param>
    ///// <returns></returns>
    //public Boolean CheckForStadiumScheduleConflict(MainDatabase.Stadium venue, DateTime potentialDate, int eventDuration)
    //{
    //    //Debug.Log("Checking " + venue.name + " for conflict on " + potentialDate.ToString());
    //    List<MainDatabase.Event> potentialConflicts = mainDB.EventDB.FindAll(e => DateTime.Parse(e.date).Day == potentialDate.Day);
    //    List<MainDatabase.Event> remainingConflicts = new List<MainDatabase.Event>();
    //    //Debug.Log(potentialConflicts.Count);
    //    if (potentialConflicts.Count > 0)
    //    {
    //        foreach (MainDatabase.Event e in potentialConflicts)
    //        {
    //            if (e.venue == venue.name)
    //            {
    //                remainingConflicts.Add(e);
    //            }
    //        }
    //        foreach (MainDatabase.Event e in remainingConflicts)
    //        {
    //            DateTime potConflictDate = DateTime.Parse(e.date);
    //            // Check if the potential event will conflict with the existing event, both before or after.
    //            DateTime previousEvent = potConflictDate.AddMinutes(e.eventDuration);
    //            DateTime followingEvent = potentialDate.AddMinutes(eventDuration);

    //            if (previousEvent > potentialDate || followingEvent > potConflictDate)
    //            {
    //                //Debug.Log("Conflict is " + e.name + " at " + e.GetDateTime().ToString());
    //                return true;
    //            }
    //        }
    //    }
    //    //Debug.Log("No Conflict");
    //    // If we get here, there are no remaining conflicts.
    //    return false;


    //}
    ///// <summary>
    ///// If returned false, there is a conflict.
    ///// </summary>
    ///// <param name="team"></param>
    ///// <param name="potentialDate"></param>
    ///// <returns></returns>
    //public Boolean CheckTeamAvailability(MainDatabase.Team team, DateTime potentialDate)
    //{
    //    //Debug.Log("Checking availability for " + team.name + " on " + potentialDate.ToShortDateString() + " at " + potentialDate.ToShortTimeString());
    //    //Debug.Log(potentialDate.ToString());
    //    // Check if the team is available on the current date
    //    foreach (MainDatabase.Game g in mainDB.EventDB)
    //    {
    //        if (g.GetDateTime().ToShortDateString() == potentialDate.ToShortDateString() && (mainDB.StringToTeam(g.awayTeam) == team || mainDB.StringToTeam(g.homeTeam) == team))
    //        {
    //            //Debug.Log(team.name + " has a game on this day");
    //            //Debug.Log(potentialDate.ToString());
    //            return false;
    //        }
    //    }
    //    // Create a list to ensure there are not too many games in a short span of time
    //    List<DateTime> datesToCheck = new List<DateTime>();
    //    List<MainDatabase.Game> potentialConflicts = new List<MainDatabase.Game>();
    //    //int numCheck = 0;
    //    for(int i = -7; i < 7; i++)
    //    {
    //        //numCheck++;
    //        DateTime dayToCheck = potentialDate.AddDays(i);
    //        datesToCheck.Add(dayToCheck);
    //    }
    //    //Debug.Log(numCheck);
    //    // Check each date for other games including this team
    //    foreach(DateTime date in datesToCheck)
    //    {
    //        //Debug.Log(date.ToString());
    //        foreach(MainDatabase.Game g in mainDB.EventDB)
    //        {
    //            if(g.GetDateTime().ToShortDateString() == date.ToShortDateString() && (mainDB.StringToTeam(g.awayTeam) == team || mainDB.StringToTeam(g.homeTeam) == team))
    //            {
    //                //Debug.Log(g.name);
    //                //Debug.Log(g.date);
    //                potentialConflicts.Add(g);
    //            }
    //        }
    //    }
    //    // If there cannot be more games in this two week period, return false.
    //    if(potentialConflicts.Count > mainDB.StringToLeague(team.league).rules.maxGamesInTwoWeeks)
    //    {
    //        //Debug.Log("Too many games in two week period for " + team.name + "; " + potentialConflicts.Count);
    //        //Debug.Log(potentialDate.ToString());
    //        return false;
    //    }
    //    // Check if this game would violate the minimum interval between games
    //    foreach(MainDatabase.Game g in potentialConflicts)
    //    {
    //        DateTime laterGame = g.GetDateTime().AddDays(mainDB.StringToLeague(team.league).rules.minIntervalBetweenGames);
    //        DateTime earlierGame = g.GetDateTime().AddDays(-(mainDB.StringToLeague(team.league).rules.minIntervalBetweenGames));
    //        if (laterGame >= potentialDate && potentialDate >= earlierGame)
    //        {
    //            //Debug.Log("Too many games within minimum interval for " + team.name);
    //            //Debug.Log(potentialDate.ToString());
    //            return false;
    //        }
    //    }

    //    //Debug.Log(team.name + " is available!");
    //    // If we made it here, this date is good to go!
    //    return true ;
    //}
    #endregion
