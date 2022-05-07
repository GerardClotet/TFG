using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to gather all the data related to the player to then generate a report
/// </summary>
public class ReportGatherer : MonoBehaviour
{
    // Start is called before the first frame update
    UserInputManager inputManager;
    public static ReportGatherer Instance { get; private set; }

    private int nJumps = 0; //number of jumps

    private string name = string.Empty;
    void Awake()
    {
        Instance = this;
        inputManager = UserInputManager.Instance;
        inputManager.jumpEvent += JumpCounter;
    }
    
    private void JumpCounter()
    {
        nJumps += 1;
    }

    public void SetReportGatherer(string n)
    {
        name = n;
    }
}
