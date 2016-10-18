using UnityEngine;
using System;
using System.Collections;

public class UserSettings : MonoBehaviour
{
    public static int NumberOfPlayers;

    public const string FIRST_RUN_KEY = "_savedFirstRun";
	public const string SFX_ENABLED_KEY = "_sfxEnabled";
	public const string MUSIC_ENABLED_KEY = "_musicEnabled";
    
    private int _firstRun;
	private static bool _sfxEnabled;
	private static bool _musicEnabled;



    public static bool MusicEnabled {
        get { return _musicEnabled; }
        private set { }
    }

    public static bool SFXEnabled {
        get { return _sfxEnabled; }
        private set { }
    }

    void Awake()
    {
        //PlayerPrefs.SetInt(FIRST_RUN_KEY, 0);

        _firstRun = PlayerPrefs.GetInt(FIRST_RUN_KEY);
        if (_firstRun == 0)
        {
            // Default Settings
            PlayerPrefs.SetInt(FIRST_RUN_KEY, 1);
            SetMusicEnabled(true);
            SetSFXEnabled(true);
        }
        else
        {
            // Load User Settings
            int music = PlayerPrefs.GetInt(MUSIC_ENABLED_KEY);
            int sfx = PlayerPrefs.GetInt(SFX_ENABLED_KEY);

            SetMusicEnabled((music == 1));
            SetSFXEnabled((sfx == 1));
        }
    }

    public static void SetSFXEnabled(bool enable)
    {
        int nEnable = (enable) ? 1 : 0;

        _sfxEnabled = enable;
        PlayerPrefs.SetInt(SFX_ENABLED_KEY, nEnable);
        CAudioManager.instance.SfxSource.enabled = enable;
    }

    public static void SetMusicEnabled(bool enable)
    {
        int nEnable = (enable) ? 1 : 0;

        _musicEnabled = enable;
        PlayerPrefs.SetInt(MUSIC_ENABLED_KEY, nEnable);
        CAudioManager.instance.MusicSource.enabled = enable;

        if (enable)
            CAudioManager.instance.MusicSource.Play();
    }
    
    


	//public static DateTime firstInstallDate
 //       {
	//	get {
	//		string _firstInstallDate;
	//		if(PlayerPrefs.HasKey(FIRST_INSTALL_DATE_KEY)){
	//			_firstInstallDate = PlayerPrefs.GetString(FIRST_INSTALL_DATE_KEY);
				
	//		}else{
	//			DateTime today = System.DateTime.Today;
	//			_firstInstallDate = today.ToString();
	//			PlayerPrefs.SetString(FIRST_INSTALL_DATE_KEY, _firstInstallDate);
	//			//date: 09/28/2014 00:00:00,_firstInstallDate:09/28/2014 00:00:00
	//		}
	//		DateTime dt = Convert.ToDateTime(_firstInstallDate);
	//		return dt;
	//	}
	//}


}
