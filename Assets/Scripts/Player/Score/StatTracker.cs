using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Entry {
    public string Name;
    public string Age;
    public string Date;
    public string Time;
    public string Playtime;
    public string Score;
    public string CompletedLevels;
    public string Feedback1;
    public string Feedback2;
}

public class StatTracker : MonoBehaviour {
    /*
    FORMAT:
     
    Yearly Leaderboard (max. 50, display 10+ records) - Filename "TurbineTurmoil-Yearly-yyyy.csv":
    Name, Age, Date, Time, Playtime (in seconds), Score, Completed Levels, Feedback 1, Feedback 2
    Nico, 12, 17.06.2018, 20:05:12, 600, 1234, 1, 3, 3
    ...

    Daily Leaderboard (max. 500, display 5+ records) - Filename "TurbineTurmoil-Daily-dd-mm-yyyy.csv":
    Name, Age, Date, Time, Playtime (in seconds), Score, Completed Levels, Feedback 1, Feedback 2
    Nico, 12, 17.06.2018, 20:05:12, 600, 1234, 1, 3, 3
    ...
     */

    private static string DAILY = "Stats/TurbineTurmoil-Daily-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + ".csv";
    private static string YEARLY = "Stats/TurbineTurmoil-Yearly-" + DateTime.Now.Year + ".csv";
    private static string AVERAGE = "Stats/TurbineTurmoil-Average-" + DateTime.Now.Year + ".csv";

    public Text leaderboardText;

    private StreamWriter _writer;
    private StreamReader _reader;

    private List<Entry> _yearlyEntries;
    private List<Entry> _dailyEntries;

    private Entry _playerStats;

    private int _writtenYearlyLines;
    private int _writtenDailyLines;

    public void Awake() {
        _yearlyEntries = new List<Entry>();
        _dailyEntries = new List<Entry>();

        //read already saved highscores
        ReadYearlyHighScore();
        ReadDailyHighScore();

        //track the players data and store them
        _playerStats = new Entry();

        //add our tracked player
        _yearlyEntries.Add(_playerStats);
        _dailyEntries.Add(_playerStats);
    }

    public void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            DisplayLeaderboard();
        }
    }

    private void OnDestroy() {
        //when exiting playmode (OnApplicationQuit did not work)
        TrackData();

        //write to files
        WriteYearlyScore();
        WriteDailyScore();

        Debug.Log("Writing Data...");
    }

    public void DisplayLeaderboard() {
        if (leaderboardText == null) return;

        TrackData();

        string leaderboard = "";
        leaderboard += "Daily Highscores:" + Environment.NewLine;

        _dailyEntries = _dailyEntries.OrderBy(o => o.Score).ToList(); //sort by score

        for (int i = 0; i < 5; i++) { //max. 5 entries
            if (i >= _dailyEntries.Count) break;

            leaderboard += _dailyEntries[i].Name + "\t\t" + _dailyEntries[i].Score + Environment.NewLine;
        }

        leaderboard += "Yearly Highscores:" + Environment.NewLine;

        _yearlyEntries = _yearlyEntries.OrderBy(o => o.Score).ToList(); //sort by score

        for (int i = 0; i < 10; i++) { //max 10. entries
            if (i >= _yearlyEntries.Count) break;

            leaderboard += _yearlyEntries[i].Name + "\t\t" + _yearlyEntries[i].Score + Environment.NewLine;
        }

        leaderboardText.text = leaderboard;
    }

    public void TrackData() {
        //fill with data
        _playerStats.Name = ScoreTracker.PlayerName;
        _playerStats.Age = ScoreTracker.PlayerAge;
        _playerStats.Date = DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
        _playerStats.Time = GetFormattedTime();
        _playerStats.Playtime = (int)Time.time + "";
        _playerStats.Score = (ScoreTracker.ScoreLevel1 + ScoreTracker.ScoreLevel2).ToString();
        _playerStats.Feedback1 = ScoreTracker.Feedback1.ToString();
        _playerStats.Feedback2 = ScoreTracker.Feedback2.ToString();

        if (ScoreTracker.CompletedLevel1 && ScoreTracker.CompletedLevel2) {
            _playerStats.CompletedLevels = "2";
        } else if (ScoreTracker.CompletedLevel1) {
            _playerStats.CompletedLevels = "1";
        } else {
            _playerStats.CompletedLevels = "0";
        }

        //apply
        _yearlyEntries[_yearlyEntries.Count - 1] = _playerStats;
        _dailyEntries[_dailyEntries.Count - 1] = _playerStats;
    }

    private string GetFormattedTime() {
        string formattedTime = DateTime.Now.TimeOfDay.Hours + ":";

        if(DateTime.Now.TimeOfDay.Minutes < 10) {
            formattedTime += "0" + DateTime.Now.TimeOfDay.Minutes + ":";
        } else {
            formattedTime += DateTime.Now.TimeOfDay.Minutes + ":";
        }

        if (DateTime.Now.TimeOfDay.Seconds < 10) {
            formattedTime += "0" + DateTime.Now.TimeOfDay.Seconds;
        } else {
            formattedTime += DateTime.Now.TimeOfDay.Seconds;
        }

        return formattedTime;
    }

    private void WriteYearlyScore() {
        _writer = new StreamWriter(YEARLY); //creates new one automatically

        string line = "Name,Age,Date,Time,Playtime (in Seconds),Score,Completed Levels,Feedback 1,Feedback 2";
        _writer.WriteLine(line);

        _yearlyEntries = _yearlyEntries.OrderBy(o => o.Score).ToList(); //sort by score

        //write max 50 lines
        for(int i = 0; i < 50; i++) {
            if (i >= _yearlyEntries.Count) break; //reached end of the entry list

            line = _yearlyEntries[i].Name + "," + _yearlyEntries[i].Age + "," + _yearlyEntries[i].Date + "," + _yearlyEntries[i].Time + ","
                + _yearlyEntries[i].Playtime + "," + _yearlyEntries[i].Score + "," + _yearlyEntries[i].CompletedLevels + "," + _yearlyEntries[i].Feedback1 + ","
                + _yearlyEntries[i].Feedback2;
            _writer.WriteLine(line); //write the content
        }

        _writer.Close();
    }

    private void WriteDailyScore() {
        _writer = new StreamWriter(DAILY); //creates new one automatically

        string line = "Name,Age,Date,Time,Playtime (in Seconds),Score,Completed Levels,Feedback 1,Feedback 2";
        _writer.WriteLine(line);

        _dailyEntries = _dailyEntries.OrderBy(o => o.Score).ToList(); //sort by score

        //write max 500 lines
        for (int i = 0; i < 500; i++) {
            if (i >= _dailyEntries.Count) break; //reached end of the entry list

            line = _dailyEntries[i].Name + "," + _dailyEntries[i].Age + "," + _dailyEntries[i].Date + "," + _dailyEntries[i].Time + ","
                + _dailyEntries[i].Playtime + "," + _dailyEntries[i].Score + "," + _dailyEntries[i].CompletedLevels + "," + _dailyEntries[i].Feedback1 + "," 
                + _dailyEntries[i].Feedback2;
            _writer.WriteLine(line); //write the content
        }

        _writer.Close();
    }

    private bool ReadYearlyHighScore() {
        if (!File.Exists(YEARLY)) return false;

        _reader = new StreamReader(YEARLY);

        string[] lines = new string[51]; //max 50 records + 1 heading line
        _writtenYearlyLines = 0;

        while(_reader.Peek() >= 0 && _writtenYearlyLines < lines.Length) {
            //peek and read line by line
            lines[_writtenYearlyLines] = _reader.ReadLine();
            _writtenYearlyLines++;
        }

        if (_writtenYearlyLines <= 1) {
            _reader.Close();
            return false; //nothing in the textfile except maybe heading
        }

        _yearlyEntries.Clear(); //clear the list

        //skipping first one since it is the heading
        for(int i = 1; i < _writtenYearlyLines; i++) {
            if (lines[i] == "") break; //nothing in array

            //process each line
            string[] content = lines[i].Split(','); //split by comma
            Entry newEntry = new Entry();

            //story everything in the list as an entry
            newEntry.Name = content[0];
            newEntry.Age = content[1];
            newEntry.Date = content[2];
            newEntry.Time = content[3];
            newEntry.Playtime = content[4];
            newEntry.Score = content[5];
            newEntry.CompletedLevels = content[6];
            newEntry.Feedback1 = content[7];
            newEntry.Feedback2 = content[8];

            _yearlyEntries.Add(newEntry);
        }

        _reader.Close();

        return true;
    }

    private bool ReadDailyHighScore() {
        if (!File.Exists(DAILY)) return false;

        _reader = new StreamReader(DAILY);

        string[] lines = new string[501]; //max 500 records + 1 heading line
        _writtenDailyLines = 0;

        while (_reader.Peek() >= 0 && _writtenDailyLines < lines.Length) {
            //peek and read line by line
            lines[_writtenDailyLines] = _reader.ReadLine();
            _writtenDailyLines++;
        }

        if (_writtenDailyLines <= 1) {
            _reader.Close();
            return false; //nothing in the textfile except maybe heading
        }

        _dailyEntries.Clear(); //clear the list

        //skipping first one since it is the heading
        for (int i = 1; i < _writtenDailyLines; i++) {
            if (lines[i] == "") break; //nothing in array

            //process each line
            string[] content = lines[i].Split(','); //split by comma
            Entry newEntry = new Entry();

            //story everything in the list as an entry
            newEntry.Name = content[0];
            newEntry.Age = content[1];
            newEntry.Date = content[2];
            newEntry.Time = content[3];
            newEntry.Playtime = content[4];
            newEntry.Score = content[5];
            newEntry.CompletedLevels = content[6];
            newEntry.Feedback1 = content[7];
            newEntry.Feedback2 = content[8];

            _dailyEntries.Add(newEntry);
        }

        _reader.Close();

        return true;
    }
}
