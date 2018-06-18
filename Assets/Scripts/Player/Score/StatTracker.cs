using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct YearlyEntry {
    public string Name;
    public string Date;
    public float Playtime;
    public int Score;
    public int CompletedLevels;
    public int Feedback1;
    public int Feedback2;
}

public struct DailyEntry {
    public string Name;
    public string Date;
    public float Playtime;
    public int Score;
    public int CompletedLevels;
    public int Feedback1;
    public int Feedback2;
}

public class StatTracker : MonoBehaviour {
    /*
    FORMAT:
     
    Yearly Leaderboard (max. 50, display 10+ records) - Filename "TurbineTurmoil-Yearly-yyyy.csv":
    Name, Date, Playtime, Score, Completed Levels, Feedback 1, Feedback 2
    Nico, 17.06.2018, 03h:00m:00s, 1234, 1, 3, 3
    ...

    Daily Leaderboard (max. 500, display 5+ records) - Filename "TurbineTurmoil-Daily-dd-mm-yyyy.csv":
    Name, Date, Playtime, Score, Completed Levels, Feedback 1, Feedback 2
    Nico, 17.06.2018, 03h:00m:00s, 1234, 1, 3, 3
    ...
     */

    private static string DAILY = "Stats/TurbineTurmoil-Daily-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + ".csv";
    private static string YEARLY = "Stats/TurbineTurmoil-Yearly-" + DateTime.Now.Year + ".csv";
    private static string AVERAGE = "Stats/TurbineTurmoil-Average-" + DateTime.Now.Year + ".csv";

    public Text leaderboardText;

    private StreamWriter _writer;
    private StreamReader _reader;

    private List<YearlyEntry> _yearlyEntries;
    private List<DailyEntry> _dailyEntries;

    private YearlyEntry _playerStatsYearly;
    private DailyEntry _playerStatsDaily;

    private int _writtenYearlyLines;
    private int _writtenDailyLines;

    public void Awake() {
        _yearlyEntries = new List<YearlyEntry>();
        _dailyEntries = new List<DailyEntry>();

        ReadYearlyHighScore();
        ReadDailyHighScore();

        //track the players data and store them
        _playerStatsYearly = new YearlyEntry();
        _playerStatsDaily = new DailyEntry();

        //add our tracked player
        _yearlyEntries.Add(_playerStatsYearly);
        _dailyEntries.Add(_playerStatsDaily);
    }

    public void Update() {
        ScoreTracker.Playtime += Time.deltaTime; //track playtime

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
        _playerStatsYearly.Name = ScoreTracker.PlayerName;
        _playerStatsYearly.Date = DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
        _playerStatsYearly.Playtime = ScoreTracker.Playtime;
        _playerStatsYearly.Score = ScoreTracker.ScoreLevel1 + ScoreTracker.ScoreLevel2;
        _playerStatsYearly.Feedback1 = ScoreTracker.Feedback1;
        _playerStatsYearly.Feedback2 = ScoreTracker.Feedback2;

        if(ScoreTracker.CompletedLevel1 && ScoreTracker.CompletedLevel2) {
            _playerStatsYearly.CompletedLevels = 2;
        } else if (ScoreTracker.CompletedLevel1) {
            _playerStatsYearly.CompletedLevels = 1;
        } else {
            _playerStatsYearly.CompletedLevels = 0;
        }

        //fill with data
        _playerStatsDaily.Name = ScoreTracker.PlayerName;
        _playerStatsDaily.Date = DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
        _playerStatsDaily.Playtime = ScoreTracker.Playtime;
        _playerStatsDaily.Score = ScoreTracker.ScoreLevel1 + ScoreTracker.ScoreLevel2;
        _playerStatsDaily.Feedback1 = ScoreTracker.Feedback1;
        _playerStatsDaily.Feedback2 = ScoreTracker.Feedback2;

        if (ScoreTracker.CompletedLevel1 && ScoreTracker.CompletedLevel2) {
            _playerStatsDaily.CompletedLevels = 2;
        } else if (ScoreTracker.CompletedLevel1) {
            _playerStatsDaily.CompletedLevels = 1;
        } else {
            _playerStatsDaily.CompletedLevels = 0;
        }

        //apply
        _yearlyEntries[_yearlyEntries.Count - 1] = _playerStatsYearly;
        _dailyEntries[_dailyEntries.Count - 1] = _playerStatsDaily;
    }

    private void WriteYearlyScore() {
        _writer = new StreamWriter(YEARLY); //creates new one automatically

        string line = "Name,Date,Playtime,Score,Completed Levels,Feedback 1,Feedback 2";
        _writer.WriteLine(line);

        _yearlyEntries = _yearlyEntries.OrderBy(o => o.Score).ToList(); //sort by score

        //write max 50 lines
        for(int i = 0; i < 50; i++) {
            if (i >= _yearlyEntries.Count) break; //reached end of the entry list

            line = _yearlyEntries[i].Name + "," + _yearlyEntries[i].Date + "," + Math.Round(_yearlyEntries[i].Playtime / 60.0f, 2) + "," + _yearlyEntries[i].Score + "," +
                _yearlyEntries[i].CompletedLevels + "," + _yearlyEntries[i].Feedback1 + "," + _yearlyEntries[i].Feedback2;
            _writer.WriteLine(line); //write the content
        }

        _writer.Close();
    }

    private void WriteDailyScore() {
        _writer = new StreamWriter(DAILY); //creates new one automatically

        string line = "Name,Date,Playtime,Score,Completed Levels,Feedback 1,Feedback 2";
        _writer.WriteLine(line);

        _dailyEntries = _dailyEntries.OrderBy(o => o.Score).ToList(); //sort by score

        //write max 500 lines
        for (int i = 0; i < 500; i++) {
            if (i >= _dailyEntries.Count) break; //reached end of the entry list

            line = _dailyEntries[i].Name + "," + _dailyEntries[i].Date + "," + Math.Round(_dailyEntries[i].Playtime / 60.0f, 2) + "," + _dailyEntries[i].Score + "," +
                _dailyEntries[i].CompletedLevels + "," + _dailyEntries[i].Feedback1 + "," + _dailyEntries[i].Feedback2;
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
            YearlyEntry newEntry = new YearlyEntry();

            //story everything in the list as an entry
            newEntry.Name = content[0];
            newEntry.Date = content[1];
            float.TryParse(content[2], out newEntry.Playtime);
            int.TryParse(content[3], out newEntry.Score);
            int.TryParse(content[4], out newEntry.CompletedLevels);
            int.TryParse(content[5], out newEntry.Feedback1);
            int.TryParse(content[6], out newEntry.Feedback2);

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
            DailyEntry newEntry = new DailyEntry();

            //story everything in the list as an entry
            newEntry.Name = content[0];
            newEntry.Date = content[1];
            float.TryParse(content[2], out newEntry.Playtime);
            int.TryParse(content[3], out newEntry.Score);
            int.TryParse(content[4], out newEntry.CompletedLevels);
            int.TryParse(content[5], out newEntry.Feedback1);
            int.TryParse(content[6], out newEntry.Feedback2);

            _dailyEntries.Add(newEntry);
        }

        _reader.Close();

        return true;
    }
}
