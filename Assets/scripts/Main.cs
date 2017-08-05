using System.Collections;
using System.Collections.Generic;
using Assets.scripts.utils;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public bool DebugScript;

    private static Main _main;
    public static Main Instance
    {
        get { return _main; }
    }

    public User LoggedUser;

    public SceneSetup SceneSetup;

    public bool SaveMemory;

    [HideInInspector]
    public DataService DataService;
    [HideInInspector]
    public GameController GameController;
    
    void Awake()
    {
        _main = GetComponent<Main>();
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        utils.Setup();
        
        GameController.Instance.gameObject.SetActive(true);
        
        /*
         * ---------------------------------------------------------------------
         * * ---------------------------------------------------------------------
         * * ---------------------------------------------------------------------
         */

        DataService = new DataService("Database.db");
        CreateSession();

        SceneSetup.Init();
    }

    // callbacks
    public delegate void OnSplashFinish();


    IEnumerator ShowIntro(OnSplashFinish onSplashFinish)
    {
        yield return new WaitForSeconds(2f);

        onSplashFinish();
    }

    public void CreateSession()
    {
        LoggedUser = DataService.GetUser();
        if (LoggedUser == null)
        {
            DataService.CreateDB();
            var user = new User
            {
                Id = 1,
                Name = "Player 1",
                Language = "en_US",
                Maps = 0
            };
            Debug.Log("This is a fresh install. Creating user..." + user);
            DataService.CreateUser(user);
            //
            LoggedUser = DataService.GetUser();
        }

        if (LoggedUser.Maps == 0)
        {
            LoggedUser.Maps = 1;
        }
    }
}