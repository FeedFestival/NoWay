using System.Collections.Generic;
using SQLite4Unity3d;
using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Assets.scripts.utils;

public class User
{

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public string Name { get; set; }
    public int Maps { get; set; }

    // Game Settings
    public bool IsUsingSound { get; set; }
    public string Language { get; set; }

    //[Ignore]
    //public FacebookApp FacebookApp { get; set; }

    public override string ToString()
    {
        return string.Format(@"
            [User: 
            Id={0}, 
            Name={1}, 
            Maps={2}, 
            IsUsingSound={3}",
            Id,
            Name,
            Maps,
            IsUsingSound);
    }

    public static User FillData(string properties)
    {
        return new User
        {
            Id = utils.GetIntDataValue(properties, "ID:"),
            Name = utils.GetDataValue(properties, "Name:"),
            Maps = utils.GetIntDataValue(properties, "Maps:"),
            IsUsingSound = utils.GetBoolDataValue(properties, "IsUsingSound:")
            //FacebookApp = new FacebookApp
            //{
            //    FacebookId = utils.GetLongDataValue(properties, "FacebookId:")
            //}
        };
    }
}

public class Level
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }

    public bool isNew;
    public bool hasNoSavedTiles;

    public override string ToString()
    {
        return string.Format(@"
            Id={0}, 
            Name={1}",
            Id,
            Name
            );
    }
}

public enum TileState
{
    Clear = 0,
    Blocked = 1,
    DeathZone = 2,
    BoxIn = 3
}

public enum Speciality
{
    None = 0,
    EntryPoint = 1,
    ExitPoint = 2
}

[Serializable]
public class Tile
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int MapId { get; set; }

    public int X { get; set; }
    public int Y { get; set; }

    public TileState State
    {
        get;
        set;
    }

    public Speciality Speciality
    {
        get;
        set;
    }

    public string Data { get; set; }

    [Ignore]
    public float x { get; set; }
    [Ignore]
    public float y { get; set; }
    [Ignore]
    public float z { get; set; }
    [Ignore]
    public TileDebug TileDebug { get; internal set; }

    public Vector3 Position;

    public Tile MakeTile(
        TileState tileState = global::TileState.Clear,
        int x = 0,
        int y = 0,
        Vector3 position = default(Vector3))
    {
        State = tileState;
        X = x;
        Y = y;
        Position = position;

        return this;
    }

    internal void SetState(TileState state)
    {
        State = state;

        if (TileDebug == null) return;
        TileDebug.TileState = state;
    }
}