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

    private int nJumps = 0; //number of jumps
 
    void Start()
    {
        inputManager = UserInputManager.Instance;
        inputManager.jumpEvent += JumpCounter;
    }

    private void JumpCounter()
    {
        nJumps += 1;
    }


}
