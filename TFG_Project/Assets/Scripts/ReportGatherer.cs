using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script to gather all the data related to the player to then generate a report
/// </summary>
public class ReportGatherer : MonoBehaviour
{
    UserInputManager inputManager;
    Player player;
    public static ReportGatherer Instance { get; private set; }

    //PLAYERDATA
    private int nJumps = 0; //number of jumps
    private int nDeaths = 0;
    private int nDash = 0;
    private string username = string.Empty;
    private int nCollectibles = 0;

    private Dictionary<string,string> questionaryAnswers = new Dictionary<string,string>();
    private List<string> data = new List<string>();
    void Awake()
    {
        Instance = this;
        inputManager = UserInputManager.Instance;
        player = Player.Instance;

        inputManager.jumpEvent += JumpCounter;
        player.dieAction += DieCounter;
        inputManager.dashEvent += DashCounter;
        player.getCollectableMultiplierAction += CollectibleMultiplierCounter;
        SceneManager.activeSceneChanged += ResetScene;

    }

    private void JumpCounter() => nJumps += 1;

    private void DieCounter() => nDeaths += 1;

    private void DashCounter() => nDash += 1;

    private void CollectibleMultiplierCounter() => nCollectibles += 1;

    public void SetReportGathererUserName(string n) => username = n;

    public void AddAnswerValue(int q, int a)
    {
        questionaryAnswers.Add(q.ToString(), a.ToString());
        data.Add(a.ToString());
    }

    public void GatherAndSendInfo()
    {
        data.Reverse();
        data.Insert(0,nJumps.ToString());
        data.Insert(0,nDeaths.ToString());
        data.Insert(0,nDash.ToString());

        Dictionary<string, string> dict = new Dictionary<string, string>
        {
            { username + " Report", CSVManager.GetCommaSeparatedString(data.ToArray())}
        };
        PlayFabManager.Instance.UploadDataCSV(dict);
    }

    public void ResetScene(Scene current, Scene next)
    {
        questionaryAnswers.Clear();
    }
}
