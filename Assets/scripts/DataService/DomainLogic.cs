using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

[ExecuteInEditMode]
public class DomainLogic : MonoBehaviour
{
    [SerializeField]
    public TextAsset txtAsset;


    public void RecreateDataBase()
    {
        var dataService = new DataService("Database.db");
        dataService.CreateDB();
    }

    public void CreateSpecialityTable()
    {
        //var dataService = new DataService("Database.db");
        //dataService.CreateSpecialityTable();
    }

    public void CleanUpUsers()
    {
        var dataService = new DataService("Database.db");
        dataService.CleanUpUsers();
    }

    public void LoadLevelsFromText()
    {
        List<Level> levels = new List<Level>();

        string fs = txtAsset.text;
        string[] fLines = Regex.Split(fs, "\n|\r|\r\n");

        for (int i = 0; i < fLines.Length; i++)
        {

            string valueLine = fLines[i];
            string[] values = Regex.Split(valueLine, ";"); // your splitter here

            foreach (var value in values)
            {
                if (string.IsNullOrEmpty(value) == false)
                    levels.Add(new Level { Name = value });
            }
        }

        var dataService = new DataService("Database.db");
        dataService.CreateLevels(levels);
    }
}