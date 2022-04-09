using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
public class PlayFabManager : MonoBehaviour
{
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

    void OnError(PlayFabError error)
    {
        Debug.Log("Failure on login/account creating!");
        Debug.Log(error.GenerateErrorReport());
    }
}
