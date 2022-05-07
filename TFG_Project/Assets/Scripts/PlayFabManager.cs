using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using System;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
       Login();   
    }

    void Login()
    {
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnSucces, OnError);
    }

    private void OnSucces(LoginResult result)
    {
        Debug.Log("Succesful login/account created!");
        Debug.Log("Your ID is: " + result.PlayFabId);
    }

    public void UploadDataCSV(Dictionary<string,string> dict)
    {
        //  UpdateUserDataRequest
        var request = new UpdateUserDataRequest
        {
            Data = dict
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend,OnError);
    }


    public void GetDataCSV()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest { }, OnDataRecievedCSV, OnError);
    }

    private void OnDataRecievedCSV(GetUserDataResult result)
    {
        Debug.Log("Recieved Data CSV");
        if (result != null && result.Data.ContainsKey("Report"))
        {
            CSVManager.AppendToReportSingleString(result.Data["Report"].Value);
        }
    }

    private void OnDataSend(UpdateUserDataResult dataSend)
    {
        Debug.Log("Data has uploaded properly!");
        Application.Quit();
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }
}
