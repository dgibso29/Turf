using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;


/// <summary>
/// Primary database for game. Contains references to all other databases. 
/// </summary>
[XmlRoot("MainDatabase")]
public class MainDatabase {

    // Declare all database classes
    public class GameInfo
    {
        /// <summary>
        /// Name of saved game.
        /// </summary>
        public string name;
        /// <summary>
        /// In-game date and time.
        /// </summary>
        public string date;
        /// <summary>
        /// Time this saved game was last modified.
        /// </summary>
        public string savetime;


        public System.DateTime GetDateTime()
        {
            return System.DateTime.Parse(date);
        }


        public System.DateTime GetSaveDateTime()
        {
            return System.DateTime.Parse(savetime);
        }

    }

    public class Stadium
    {
        public string name;
        public string city;
        public string state;
        public string country;
        public string owner;
        public int primaryCapacity;
        public int secondaryCapacity;
        public int tertiaryCapacity;
        public string primary;
        public string secondary;
        public string tertiary;
        public Staff staff;
        public Finances finances;
       
    }

    public class City
    {
        public string name;
        public string state;
        public string country;
        public int population;
        public string climate;
        /// <summary>
        /// Measure of average wage in city vs national. 0 is equivalent to national average.
        /// </summary>
        public float wages;

    }

    public class Team
    {
        public string name;
        public string sport;
        public string league;
        public string stadium;
        public string city;
        public string state;
        public string country;
        public int fanbase;
        /// <summary>
        /// Statistics of team in current league season.
        /// </summary>
        public TeamLeagueStats currentSeasonStats = new TeamLeagueStats();


    }

    public class Company
    {
        public string name;
        public string city;
        public string owner;
        public Staff staff;
        public Finances finances;
    }

    public class EventTemplate
    {
        public string name;
        public string type;

    }

    public class Event
    {
        public string name;
        public string type;
        /// <summary>
        /// Time & date when event prep starts (separate from gameStart, concertStart, etc. (convert to DateTime).
        /// </summary>
        public string date;
        /// <summary>
        /// Length of event in minutes.
        /// </summary>
        public int eventDuration;
        public string venue;

        public System.DateTime GetDateTime()
        {
            return System.DateTime.Parse(date);
        }
    }

    public class Game : Event
    {
        /// <summary>
        /// Time the game actually begins -- Event.date tracks when the event itself begins, including prep time.
        /// </summary>
        public string gameStart;
        public string sport;
        public string league;
        /// <summary>
        /// Year of season game is played in. Season is accessible using helper function. 
        /// </summary>
        public string season;
        public string homeTeam;
        public string awayTeam;
        public int scoreHome;
        public int scoreAway;
        public bool overtime;
        public bool penalties;
        public int penaltiesHome;
        public int penaltiesAway;
        public string winner = "none";

        public void CreateGame(string name, string date, int duration, string gameStart, string sport, string league, string season, string homeTeam, string awayTeam, string venue)
        {
            this.name = name;
            this.date = date;
            this.eventDuration = duration;
            this.gameStart = gameStart;
            this.sport = sport;
            this.league = league;
            this.season = season;
            this.homeTeam = homeTeam;
            this.awayTeam = awayTeam;
            this.venue = venue;
        }

    }

    public class Concert : Event
    {

    }


    public class Sport
    {
        public string name;
        public float popularity;
    }

    public class Vendor
    {
        public string name;
    }

    public class League
    {
        public string name;
        public string sport;
        public string abbreviation;
        /// <summary>
        /// Minimum capacity of stadiums for this league.
        /// </summary>
        public int minCap;
        /// <summary>
        /// Maximum capacity of stadiums for this league. Used to keep lower leagues in smaller stadiums. Value of '0' means no cap.
        /// </summary>
        public int maxCap;
        public float popularity;
        public int teams;
        public string startDate;
        public string endDate;
        public string primGameTime;
        public string secGameTime;
        public string terGameTime;
        public Rules rules;
        /// <summary>
        /// Track league history by season.
        /// </summary>
        public List<Season> leagueHistory;

        public void SetLeagueRules(int meetings, bool draws, bool overtime, bool penalties, List<string> mainDays, List<string> offDays, List<int> weekendStartTimes, List<int> weekdayStartTimes,
            int stadiumPrepTime, int regulationDuration, int overtimeDuration, int stadiumBreakdownTime, int maxGamesInTwoWeeks, int minIntervalBetweenGames)
        {
            rules = new Rules();
            rules.numberofmeetings = meetings;
            rules.drawsAllowed = draws;
            rules.overtimeAllowed = overtime;
            rules.penaltyShootoutsAllowed = penalties;
            rules.mainDays = mainDays;
            rules.offDays = offDays;
            rules.weekendStartTimes = weekendStartTimes;
            rules.weekdayStartTimes = weekdayStartTimes;
            rules.stadiumPrepTime = stadiumPrepTime;
            rules.regulationDuration = regulationDuration;
            rules.overtimeDuration = overtimeDuration;
            rules.stadiumBreakdownTime = stadiumBreakdownTime;
            rules.maxGamesInTwoWeeks = maxGamesInTwoWeeks;
            rules.minIntervalBetweenGames = minIntervalBetweenGames;
        }


    }



    public class Climate
    {
        public string name;
        public float JanHi;
        public float JanLo;
        public float JanPre;
        public float FebHi;
        public float FebLo;
        public float FebPre;
        public float MarHi;
        public float MarLo;
        public float MarPre;
        public float AprHi;
        public float AprLo;
        public float AprPre;
        public float MayHi;
        public float MayLo;
        public float MayPre;
        public float JunHi;
        public float JunLo;
        public float JunPre;
        public float JulHi;
        public float JulLo;
        public float JulPre;
        public float AugHi;
        public float AugLo;
        public float AugPre;
        public float SepHi;
        public float SepLo;
        public float SepPre;
        public float OctHi;
        public float OctLo;
        public float OctPre;
        public float NovHi;
        public float NovLo;
        public float NovPre;
        public float DecHi;
        public float DecLo;
        public float DecPre;
    }

    // Declare all additional data classes

        /// <summary>
        /// Handles staff for stadiums and companies (Stadium level vs corporate level employees).
        /// </summary>
    public class Staff
    {
        public int ticket;
        public int security;
        public int grounds;

    }
    /// <summary>
    /// Hanldes finances for stadiums and companies (stadium finances contribute to corporate finances).
    /// </summary>
    public class Finances
    {
        public float funds;

    }
    /// <summary>
    /// Holds rules for sport leagues.
    /// </summary>
    public class Rules
    {
        /// <summary>
        /// Number of meetings between 2 teams (20 teams that each meet twice = 38 games per team).
        /// </summary>
        public int numberofmeetings;
        /// <summary>
        /// Can teams in this league tie? If not, overtime or penalty rules apply (depending on sport).
        /// </summary>
        public bool drawsAllowed;
        /// <summary>
        /// Allow overtime if draws not allowed.
        /// </summary>
        public bool overtimeAllowed;
        /// <summary>
        /// Allow penalty shootouts if draws are not allowed.
        /// </summary>
        public bool penaltyShootoutsAllowed;
        /// <summary>
        /// Primary days to play games (IE Weekends for Soccer/Football
        /// </summary>
        public List<string> mainDays;
        /// <summary>
        /// Off days to play games if needed (IE Wednesdays for soccer. Make sure games are not scheduled too tighly if off day is used.
        /// </summary>
        public List<string> offDays;
        /// <summary>
        /// Legal start times on weekends.
        /// </summary>
        public List<int> weekendStartTimes;
        /// <summary>
        /// Legal start times on weekdays;
        /// </summary>
        public List<int> weekdayStartTimes;
        /// <summary>
        /// Time stadium needs to prep for a game
        /// </summary>
        public int stadiumPrepTime;
        /// <summary>
        /// How long a regular game will last
        /// </summary>
        public int regulationDuration;
        /// <summary>
        /// How much time overtime adds to the duration of a game
        /// </summary>
        public int overtimeDuration;
        /// <summary>
        /// Time stadium needs to breakdown a game
        /// </summary>
        public int stadiumBreakdownTime;
        /// <summary>
        /// Max number of games a team can play in a two week period
        /// </summary>
        public int maxGamesInTwoWeeks;
        /// <summary>
        /// Minimum interval between games
        /// </summary>
        public int minIntervalBetweenGames;


    }
    /// <summary>
    /// Tracks all season information for a sport.
    /// </summary>
    public class Season
    {
        public string league;
        public string year;
        /// <summary>
        /// Holds info for each team that played this season.
        /// </summary>
        public List<TeamLeagueStats> seasonStats;
    }

    /// <summary>
    /// Used to track team stats
    /// </summary>
    public class TeamLeagueStats
    {
        public string team;
        public string league;
        public string season;
        public int points;
        public float pointsPerGame = 0f;
        public int gamesPlayed;
        public int wins;
        public int losses;
        public int penaltyWins;
        public int penaltyLosses;
        public int draws;
        public int goalsFor;
        public int goalsAgainst;
        public int goalDifference;
        /// <summary>
        /// Did the game go into overtime?
        /// </summary>
        public bool overtime;
    }

    // Declare all database lists for Xml to deserialize

    // GameInfo DB
    [XmlArray("GameInfoDB")]
    [XmlArrayItem("Game")]
    public List<GameInfo> gameInfoDB = new List<GameInfo>();

    // Stadium DB
    [XmlArray("StadiumDB")]
    [XmlArrayItem("Stadium")]
    public List<Stadium> StadiumDB = new List<Stadium>();

    // City DB
    [XmlArray("CityDB")]
    [XmlArrayItem("City")]
    public List<City> CityDB = new List<City>();

    // Team DB
    [XmlArray("TeamDB")]
    [XmlArrayItem("Team")]
    public List<Team> TeamDB = new List<Team>();

    // Company DB
    [XmlArray("CompanyDB")]
    [XmlArrayItem("Company")]
    public List<Company> CompanyDB = new List<Company>();

    // EventTemplate DB
    [XmlArray("EventTemplateDB")]
    [XmlArrayItem("Template")]
    public List<EventTemplate> EventTemplateDB = new List<EventTemplate>();

    // Event DB
    [XmlArray("EventDB")]
    [XmlArrayItem("Game", typeof(Game))]
    [XmlArrayItem("Concert", typeof(Concert))]
    public List<Event> EventDB = new List<Event>();

    // Sport DB
    [XmlArray("SportDB")]
    [XmlArrayItem("Sport")]
    public List<Sport> SportDB = new List<Sport>();

    // Vendor DB   
    [XmlArray("VendorDB")]
    [XmlArrayItem("Vendor")]
    public List<Vendor> VendorDB = new List<Vendor>();

    // League DB
    [XmlArray("LeagueDB")]
    [XmlArrayItem("League")]
    public List<League> LeagueDB = new List<League>();

    // Climate DB
    [XmlArray("ClimateDB")]
    [XmlArrayItem("Climate")]
    public List<Climate> ClimateDB = new List<Climate>();

    // Load database

    public static MainDatabase Load(string path)
    {

        var serializer = new XmlSerializer(typeof(MainDatabase));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as MainDatabase;
        }
        
    }

    // Save database -- this will be how we save the game, so the path will be the name of the save, and it will be placed in a different directory than the initial database file. 
    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(MainDatabase));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
            stream.Close();
        }
    }

    // Helper functions
    public Stadium StringToStadium(string stadiumName)
    {
        return StadiumDB[StadiumDB.FindIndex(p => p.name == stadiumName)];
    }

    public Season StringToSeason(string seasonLeague, string seasonYear)
    {
        League league = LeagueDB[LeagueDB.FindIndex(p => p.name == seasonLeague)];
        return league.leagueHistory[league.leagueHistory.FindIndex(p => p.year == seasonYear)];
    }

    public Team StringToTeam(string teamName)
    {
        return TeamDB[TeamDB.FindIndex(t => t.name == teamName)];
    }

    public League StringToLeague(string leagueName)
    {
        return LeagueDB[LeagueDB.FindIndex(l => l.name == leagueName)];
    }

    public DayOfWeek StringToDayOfWeek(string day)
    {

       switch (day)
        {
            case "Monday":
                return DayOfWeek.Monday;
            case "Tuesday":
                return DayOfWeek.Tuesday;
            case "Wednesday":
                return DayOfWeek.Wednesday;
            case "Thursday":
                return DayOfWeek.Thursday;
            case "Friday":
                return DayOfWeek.Friday;
            case "Saturday":
                return DayOfWeek.Saturday;
            case "Sunday":
                return DayOfWeek.Sunday;
            default:
                return DayOfWeek.Saturday;               

        }
    }


}
