using UnityEngine;
using System.Collections;

public class CMenuManager : MonoBehaviour
{
    #region Singleton Instance
    private static CMenuManager _instance;
    public static CMenuManager instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<CMenuManager>();
            }
            return _instance;
        }
    }
    #endregion

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
