using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

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

    public void UploadData(Dictionary<string,string> dict)//Todo need some way of expanding the value of the key in playfab without overriding it
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
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }
}
