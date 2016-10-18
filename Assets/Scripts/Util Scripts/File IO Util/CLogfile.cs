using UnityEngine;
using System.IO;
using System.Collections;

public class CLogfile : MonoBehaviour
{
    #region Singleton Instance
    private static CLogfile _instance;
    public static CLogfile instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<CLogfile>();
            }
            return _instance;
        }
    }
    #endregion

    private string filepath;
    private string filename;
    private string wholeFilepath;

    private StreamWriter logWriter;

	void Awake ()
    {
        filepath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        filename = "KS_Logfile.txt";

        switch(Application.platform)
        {
            case RuntimePlatform.OSXEditor: wholeFilepath = filepath + "/" + filename;
            break;
            case RuntimePlatform.OSXPlayer: wholeFilepath = filepath + "/" + filename;
            break;
            case RuntimePlatform.WindowsEditor: wholeFilepath = filepath + "\\" + filename;
            break;
            case RuntimePlatform.WindowsPlayer: wholeFilepath = filepath + "\\" + filename;
            break;
        }

        StartCoroutine(CreateLogFile());
	}

    private IEnumerator CreateLogFile()
    {
        yield return new WaitForEndOfFrame();

		#if UNITY_IOS

		logWriter = null;

		#else

		if (!File.Exists(wholeFilepath))
		{
			logWriter = File.CreateText(wholeFilepath);
			yield return new WaitForSeconds(1f);

			logWriter.WriteLine("");
			logWriter.WriteLine("");
			logWriter.WriteLine("================================================================================");
			logWriter.WriteLine("Start New Log - " + System.DateTime.Today);
			logWriter.WriteLine("================================================================================");
			}
			else
			{
			logWriter = new StreamWriter(wholeFilepath);
			logWriter.WriteLine("");
			logWriter.WriteLine("");
			logWriter.WriteLine("================================================================================");
			logWriter.WriteLine("Start New Log - " + System.DateTime.Today);
			logWriter.WriteLine("================================================================================");
		}

		#endif
        
    }

    public void Append(string newline)
    {
		if(logWriter != null)
        	logWriter.WriteLine(newline);
	}

    void OnApplicationQuit()
    {
		if(logWriter != null)
	        logWriter.Close();
    }


}
