using System;
using SQLite4Unity3d;
using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

public class DataService
{
    private SQLiteConnection _connection;

    public DataService(string DatabaseName)
    {

        #region DataServiceInit


#if UNITY_EDITOR
        var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif

        #endregion

        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Final PATH: " + dbPath);
    }



    public void CleanUpUsers()
    {
        _connection.DropTable<User>();
        _connection.CreateTable<User>();
    }

    public void CleanUpLevels()
    {
        _connection.DropTable<Level>();
        _connection.CreateTable<Level>();
    }

    public void CreateDB()
    {
        if (_connection.Table<User>().Where(x => x.Id == 1).FirstOrDefault() == null)
        {
            _connection.CreateTable<User>();
        }

        _connection.DropTable<Level>();
        _connection.CreateTable<Level>();

        //try
        //{
        //    if (_connection.Table<Tile>().Where(x => x.Id > 0).FirstOrDefault() == null)
        //    {
        //        _connection.DropTable<Tile>();
        //        _connection.CreateTable<Tile>();
        //    }
        //}
        //catch (Exception e)
        //{
        //    Debug.LogWarning(e.Message);

        //    _connection.DropTable<Tile>();
        //    _connection.CreateTable<Tile>();
        //}

        _connection.DropTable<Tile>();
        _connection.CreateTable<Tile>();

    }

    /*
     * User
     * * --------------------------------------------------------------------------------------------------------------------------------------
     */

    public void CreateUser(User user)
    {
        _connection.Insert(user);
    }

    public void UpdateUser(User user)
    {
        int rowsAffected = _connection.Update(user);
        Debug.Log("(UPDATE User) rowsAffected : " + rowsAffected);
    }

    public User GetUser()
    {
        try
        {
            return _connection.Table<User>().Where(x => x.Id == 1).FirstOrDefault();
        }
        catch (Exception)
        {
            _connection.CreateTable<User>();
            return _connection.Table<User>().Where(x => x.Id == 1).FirstOrDefault();
        }
    }

    public User GetLastUser()
    {
        return _connection.Table<User>().Last();
    }

    //public User GetUserByFacebookId(int facebookId)
    //{
    //    return _connection.Table<User>().Where(x => x.FacebookApp.FacebookId == facebookId).FirstOrDefault();
    //}

    /*
    * User - END
    * * --------------------------------------------------------------------------------------------------------------------------------------
    */

    /*
     * Map
     * * --------------------------------------------------------------------------------------------------------------------------------------
     */

    // X : 0 - 11
    // Y : 0 - 8
    //public int CreateMap(Map map)
    //{
    //    _connection.Insert(map);
    //    return map.Id;
    //}

    //public int UpdateMap(Map map)
    //{
    //    _connection.Update(map);
    //    return map.Id;
    //}

    //public Map GetMap(int mapId)
    //{
    //    return _connection.Table<Map>().Where(x => x.Id == mapId).FirstOrDefault();
    //}

    //public int GetNextMapId(int number)
    //{
    //    return _connection.Table<Map>().Where(x => x.Number == number).FirstOrDefault().Id;
    //}

    //public void CreateTiles(List<MapTile> mapTiles)
    //{
    //    _connection.InsertAll(mapTiles);
    //}

    //public IEnumerable<Map> GetMaps()
    //{
    //    return _connection.Table<Map>();
    //}

    //public IEnumerable<MapTile> GetTiles(int mapId)
    //{
    //    return _connection.Table<MapTile>().Where(x => x.MapId == mapId);
    //}

    //public void DeleteMapTiles(int mapId)
    //{
    //    var sql = string.Format("delete from MapTile where MapId = {0}", mapId);
    //    _connection.ExecuteSql(sql);
    //}

    /*
     * Map - END
     * * --------------------------------------------------------------------------------------------------------------------------------------
     */

    public int CreateLevel(Level level)
    {
        level.Id = _connection.Insert(level);
        return level.Id;
    }

    internal void CreateLevels(List<Level> levels)
    {
        _connection.InsertAll(levels);
    }

    internal Level GetLevel(string mapName)
    {
        return _connection.Table<Level>().Where(x => x.Name == mapName).FirstOrDefault();
    }

    public IEnumerable<Level> GetLevels()
    {
        return _connection.Table<Level>().Where(x => x.Id > 0);
    }

    public Tile[,] GetMainMenuTiles(int mapId)
    {
        int xLength = 0;
        int yLength = 0;
        List<Tile> allTiles = null;
        if (mapId != 0)
        {
            allTiles = _connection.Table<Tile>().Where(x => x.MapId == mapId).ToList();
            if (allTiles != null)
            {
                for (var i = 0; i < allTiles.Count; i++)
                {
                    if (xLength < allTiles[i].X)
                        xLength = allTiles[i].X;
                    if (yLength < allTiles[i].Y)
                        yLength = allTiles[i].Y;
                }
            }
        }

        if (allTiles != null && allTiles.Count > 0)
        {
            Tile[,] tiles = new Tile[xLength + 1, yLength + 1];
            for (var i = 0; i < allTiles.Count; i++)
            {
                tiles[allTiles[i].X, allTiles[i].Y] = allTiles[i];
            }
            return tiles;
        }
        else
        {
            return null;
        }
    }

    public void SaveTiles(List<Tile> tiles)
    {
        _connection.InsertAll(tiles);
    }

    public void UpdateTiles(List<Tile> tiles)
    {
        _connection.UpdateAll(tiles);
    }
}
