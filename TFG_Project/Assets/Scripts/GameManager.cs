using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {
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

    public void LoadScene()
    {
        SceneManager.LoadScene("AgressiveScene");
    }
}
