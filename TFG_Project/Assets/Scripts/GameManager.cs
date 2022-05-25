using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public enum MODE
    {
        INITIAL,
        AGRESSIVE,
        PASSIVE,
        EXPLORER,
        ACHIEVER
    }
    public static GameManager Instance;
    public MODE currentSceneMode { get; private set; }

    private static string agressiveScene = "AgressiveScene";
    private List<MODE> modeList = new List<MODE>();
    private static List<MODE> compareList = new List<MODE> {MODE.INITIAL,MODE.AGRESSIVE, MODE.EXPLORER, MODE.PASSIVE, MODE.ACHIEVER };

    private void Awake()
    {
        currentSceneMode = MODE.INITIAL;
        modeList.Add(currentSceneMode);

        Instance = this;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        List<GameObject> objs = GameObject.FindGameObjectsWithTag("Preserve").ToList();
        objs.Add(Camera.main.gameObject);
        for(int i = 0; i < objs.Count; i++)
        {
            DontDestroyOnLoad(objs[i]);
        }
    }

    /// <summary>
    /// End scene only uploads data and quits game when the user has played all modes //Need to change that but for now its ok
    /// </summary>
    public void EndScene()
    {
        ReportGatherer.Instance.GatherInfo();
        bool containedAll = !compareList.Except(modeList).Any();
        if (!containedAll)
        {
            //TODO compute the result print charts etc and decide which scene will be loaded
            currentSceneMode = MODE.AGRESSIVE;
            modeList.Add(currentSceneMode);
            switch (currentSceneMode)
            {
                case MODE.AGRESSIVE:
                    SceneManager.LoadScene(agressiveScene);
                    break;
            }
        }
        else
        {
            ReportGatherer.Instance.SendInfo();
            Application.Quit();
        }
    }
}
