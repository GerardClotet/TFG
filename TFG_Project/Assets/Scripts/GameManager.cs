using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public enum MODE //Agresive_Explorer, PAssive_Explorer, Agressive_Achiever, Passive_Achiever
{
    INITIAL,
    AGRESSIVE,
    PASSIVE
}

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public MODE currentSceneMode { get; private set; }

    private static string agressiveScene = "AgressiveScene";
    private static string passiveScene = "PassiveScene";
    private List<MODE> modeList = new List<MODE>();
    private static List<MODE> compareList = new List<MODE> {MODE.INITIAL,MODE.AGRESSIVE, MODE.PASSIVE };

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

    private void Start()
    {
        PlayFabManager.Instance.dataSended += Application.Quit;
        ReportGatherer.Instance.GetNewLevel(currentSceneMode);
    }
    /// <summary>
    /// End scene only uploads data and quits game when the user has played all modes //Need to change that but for now its ok
    /// </summary>
    public void EndScene()
    {
        bool containedAll = !compareList.Except(modeList).Any();
        if (!containedAll)
        {
            currentSceneMode = ReportGatherer.Instance.ComputeLevelData();
            currentSceneMode = MODE.PASSIVE;
            if (!modeList.Contains(currentSceneMode))
            {
                modeList.Add(currentSceneMode);
                switch (currentSceneMode)
                {
                    case MODE.AGRESSIVE:
                        DontDestroyOnLoad(FindObjectOfType<Player>());
                        StartCoroutine(LoadAsyncScene(agressiveScene));
                        break;
                    case MODE.PASSIVE:
                        DontDestroyOnLoad(FindObjectOfType<Player>());
                        StartCoroutine(LoadAsyncScene(passiveScene));
                        break;
                }
            }
            else
            {
                if(currentSceneMode == MODE.AGRESSIVE)
                {
                    currentSceneMode = MODE.PASSIVE;
                    modeList.Add(currentSceneMode);
                    DontDestroyOnLoad(FindObjectOfType<Player>());
                    StartCoroutine(LoadAsyncScene(passiveScene));
                }
                else
                {
                    currentSceneMode = MODE.AGRESSIVE;
                    modeList.Add(currentSceneMode);
                    DontDestroyOnLoad(FindObjectOfType<Player>());
                    StartCoroutine(LoadAsyncScene(agressiveScene));
                }
            }
        }
        else
        {
            ReportGatherer.Instance.SendInfoJSON();
        }
    }

    IEnumerator LoadAsyncScene(string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while (!asyncLoad.isDone)
        {
            yield return true;
        }
        if(ReportGatherer.Instance.Explorer)
        {
            GameObject[] gos = GameObject.FindGameObjectsWithTag("Explorer");
            for(int i =0; i < gos.Length; i++)
            {
                gos[i].SetActive(false);
            }
        }

        if(!ReportGatherer.Instance.Achiever)
        {
            var b = FindObjectsOfType<MultiplierCollectible>(true);
            for(int i =0; i < b.Length; i++)
            {
                b[i].gameObject.SetActive(false);
            }
        }
        else
        {
            var b = FindObjectsOfType<MultiplierCollectible>(true);
            for (int i = 0; i < b.Length; i++)
            {
                if (b[i].transform.parent.GetComponent<RoomChange>().GetRoomStatus() && !ReportGatherer.Instance.Explorer)
                {
                    b[i].gameObject.SetActive(false);
                }
            }
        }
        ReportGatherer.Instance.GetNewLevel(currentSceneMode);
    }
}
