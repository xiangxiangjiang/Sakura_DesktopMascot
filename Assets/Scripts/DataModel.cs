using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public Vector3 rolePos;
    public Quaternion roleRot;
    public Vector3 cameraPos;
    public Quaternion cameraRot;
    public bool isTopMost;
    public bool isRunOnStartup;
}

public class DataModel
{
    static DataModel _instance = new DataModel();
    public static DataModel Instance
    {
        get { return _instance; }
    }
    DataModel() { }

    public UserData Data { get; set; }

    public void Init(Vector3 _rolePos, Quaternion _roleRot, Vector3 _cameraPos, Quaternion _cameraRot)
    {
        if (PlayerPrefs.HasKey(Application.productName + "_UserData"))
        {
            var strData = PlayerPrefs.GetString(Application.productName + "_UserData");
            Data = JsonUtility.FromJson<UserData>(strData);
        }
        else
        {
            Data = new UserData() { rolePos = _rolePos, roleRot = _roleRot, cameraPos = _cameraPos, cameraRot = _cameraRot, isTopMost = false, isRunOnStartup = false };
            var strData = JsonUtility.ToJson(Data);
            PlayerPrefs.SetString(Application.productName + "_UserData", strData);
        }
    }

    public void SaveData()
    {
        var strData = JsonUtility.ToJson(Data);
        PlayerPrefs.SetString(Application.productName + "_UserData", strData);
    }

    public void ReloadData()
    {
        if (PlayerPrefs.HasKey(Application.productName + "_UserData"))
        {
            var strData = PlayerPrefs.GetString(Application.productName + "_UserData");
            Data = JsonUtility.FromJson<UserData>(strData);
        }
    }

    public void UpdateTransformData(Transform role, Transform camera)
    {
        Data.rolePos = role.position;
        Data.roleRot = role.rotation;
        Data.cameraPos = camera.position;
        Data.cameraRot = camera.rotation;
    }
}

