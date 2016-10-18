
using Assets.Scripts.UnityHTTP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using AssemblyInfo;


public class InteractCLVR : MonoBehaviour
{


    public List<string> thing2, thingKeys;
    public string thing3, thingStr, thingKeysS, thingValues, rememberKey;
    public int a = 0;

    void update()
    {
        //StartCoroutine(SomeRoutine());
    }

    public IEnumerator SomeRoutine()
    {
        //UnityHTTP.Request someRequest = new UnityHTTP.Request("get", "https://api.clever.com/v1.1/students/530e5960049e75a9262cff1d/district");
        UnityHTTP.Request someRequest = new UnityHTTP.Request("get", "https://api.clever.com/v1.1/districts");
        someRequest.AddHeader("Authorization", "Bearer DEMO_TOKEN");
        someRequest.Send();

        while (!someRequest.isDone)
        {
            yield return null;
        }

        //parse some JSON, for example:
        JSONObject thing = new JSONObject(someRequest.response.Text);
        thingStr = thing.ToString();
        print(thingStr);


        CLVRDistrict dis1 = new CLVRDistrict();
        //accessData(thing, 0);
        ParseDistrict(thing, 0, dis1);
        print(dis1.Name);
        print(dis1.Id);

    }


    public void test()
    {
        //print("lbalba");
        StartCoroutine(SomeRoutine());
        //print(thing2);
        //string thing3 = string.Join(",", thing2.ToArray());
        //Console.WriteLine(thing3);
       // print(thing3);
        //print(thing4);
    }

    void accessData(JSONObject obj, int lvl)
    {
       
        string str = "     ";
        for (int i = 0; i < lvl; i++)
            str = str + " **** ";

        switch (obj.type)
        {
            case JSONObject.Type.OBJECT:
                {
                    lvl = lvl + 1;
                    for (int i = 0; i < obj.list.Count; i++)
                    {
                        string key = (string)obj.keys[i];
                        JSONObject j = (JSONObject)obj.list[i];
                        Debug.Log(str + " **** " +"KEY: " + key + " - " + obj.type.ToString());
                        //Debug.Log("OBJECT FOUND");
                        accessData(j, lvl);
                    }
                }
                break;
            case JSONObject.Type.ARRAY:
                foreach (JSONObject j in obj.list)
                {
                    Debug.Log("Array");
                    accessData(j, lvl);
                }
                break;
            case JSONObject.Type.STRING:
                Debug.Log(str + obj.str + " - " + obj.type.ToString());
                break;
            case JSONObject.Type.NUMBER:
                Debug.Log(str + obj.n + " - " + obj.type.ToString());
                break;
            case JSONObject.Type.BOOL:
                Debug.Log(str + obj.b + " - " + obj.type.ToString());
                break;
            case JSONObject.Type.NULL:
                Debug.Log(str + "NULL");
                break;

        }
    }

    public string ParseDistrict(JSONObject obj, int lvl, CLVRDistrict district)
    {

        string str = "     ";
        for (int i = 0; i < lvl; i++)
            str = str + " **** ";

        switch (obj.type)
        {
            case JSONObject.Type.OBJECT:
                {
                    lvl = lvl + 1;
                    for (int i = 0; i < obj.list.Count; i++)
                    {
                        string key = (string)obj.keys[i];
                        JSONObject j = (JSONObject)obj.list[i];
                        Debug.Log(str + " **** " + "KEY: " + key);
                        rememberKey = key;
                        //Debug.Log("OBJECT FOUND");
                        ParseDistrict(j, lvl, district);
                    }
                }
                break;
            case JSONObject.Type.ARRAY:
                foreach (JSONObject j in obj.list)
                {
                    ParseDistrict(j, lvl, district);
                }
                break;
            case JSONObject.Type.STRING:
                {
                    if (rememberKey == "name") district.Name = obj.str;
                    if (rememberKey == "id") district.Id = obj.str;
                    Debug.Log(str + obj.str);
                }
                break;
            case JSONObject.Type.NUMBER:
                Debug.Log(str + obj.n);
                break;
            case JSONObject.Type.BOOL:
                Debug.Log(str + obj.b);
                break;
            case JSONObject.Type.NULL:
                Debug.Log(str + "NULL");
                break;

        }
        return "0";
    }
}

