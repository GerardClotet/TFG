using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System;

/// <summary>
/// Script to gather all the data related to the player to then generate a report
/// </summary>
public class ReportGatherer : MonoBehaviour
{
    public class DataGathering
    {
        public Level[] levels { get; set; }
        public string username = string.Empty;
    }
    public class Level
    {
        //TODO save test answers here
        public MODE mode { get; set; }
        public int jumps = 0;
        public int deaths = 0;
        public int dashes = 0;
        public int wallJump = 0;
        public int totalCollectibles = 0;
        public float totalTime = 0f;
        public int optionalRooms = 0;
        public Room[] rooms;
    }
    public class Room
    {
        public bool optional = false;
        public int collectiblesGot = 0;
        public int roomDashes = 0;
        public int roomDeaths = 0;
        public int roomJumps = 0;
        public int roomWallJump = 0;
        public float totalRoomTime = 0f;

        //Last run
        public int lastTryJumps = 0;
        public int lastTryDashes = 0;
        public int lastTryWallJump = 0;
        public int lastTryCollectibles = 0;
        public float lastTryTime = 0f;

        //returnedToRoom
        public bool returned = false;
        public int returnedCollectible = 0;
    }

    public static ReportGatherer Instance { get; private set; }

    private string username = string.Empty;

    private float startLevelTime;
    private float startRoomTime;
    private float startRoomLastTryTime;

    private Dictionary<string, string> questionaryAnswers = new Dictionary<string, string>();
    private List<string> data = new List<string>();

    [HideInInspector] private DataGathering dataGathering = new DataGathering();
    private int lvlCounter = -1;
    private int roomCounter = -1;
    private List<RoomChange> levelRooms = new List<RoomChange>();
    private RoomChange previousRoom = null;

    void Awake()
    {
        Instance = this;

        Player.Instance.dieAction += DieCounter;
        Player.Instance.dashAction += DashCounter;
        Player.Instance.jumpAction += JumpCounter;
        Player.Instance.endGame += AddLevelTime;
        SceneManager.activeSceneChanged += ResetScene;
        Player.Instance.bounceAction += WallJumpCounter;
        Player.Instance.collectibleGotAction += CollectibleGotTry;
        startLevelTime = Time.time;
        dataGathering.levels = new Level[5];
    }

    public void GetNewLevel(MODE lvlMode)
    {
        lvlCounter++;
        dataGathering.levels[lvlCounter] = new Level();
        dataGathering.levels[lvlCounter].mode = lvlMode;

        RoomChange[] rmch = FindObjectsOfType<RoomChange>();
        dataGathering.levels[lvlCounter].rooms = new Room[rmch.Length];
        for(int i = 0; i < rmch.Length; i++)
        {
            if(rmch[i].GetRoomStatus())
            {
                dataGathering.levels[lvlCounter].optionalRooms += 1;
            }
        }

        dataGathering.levels[lvlCounter].totalCollectibles = FindObjectsOfType<MultiplierCollectible>().Length;
        startLevelTime = Time.time;
    }

    public void EnterRoom(RoomChange enteringRoom) //Need to see if we go two rooms back
    {
        startRoomTime = Time.time;
        startRoomLastTryTime = Time.time;
        //Check if we have passed in this room
        for (int i = 0; i < levelRooms.Count(); i++)
        {
            if (levelRooms[i] == enteringRoom)
            {
                dataGathering.levels[lvlCounter].rooms[roomCounter].totalRoomTime += Time.time - startRoomTime;
                dataGathering.levels[lvlCounter].rooms[roomCounter].lastTryTime = Time.time - startRoomLastTryTime;
                roomCounter = i;
                dataGathering.levels[lvlCounter].rooms[roomCounter].returned = true;
                return;
            }
        }

        if (roomCounter > -1)
        {
            dataGathering.levels[lvlCounter].rooms[roomCounter].totalRoomTime += Time.time - startRoomTime;
            dataGathering.levels[lvlCounter].rooms[roomCounter].lastTryTime = Time.time - startRoomLastTryTime;
        }

        //First time we pass this room
        levelRooms.Add(enteringRoom);
        roomCounter = levelRooms.Count()-1;
        dataGathering.levels[lvlCounter].rooms[roomCounter] = new Room();
        dataGathering.levels[lvlCounter].rooms[roomCounter].optional = enteringRoom.GetRoomStatus();


    }

    private void JumpCounter()
    {
        dataGathering.levels[lvlCounter].jumps += 1;
        dataGathering.levels[lvlCounter].rooms[roomCounter].roomJumps += 1;
        dataGathering.levels[lvlCounter].rooms[roomCounter].lastTryJumps += 1;
    }

    private void DieCounter()
    {
        dataGathering.levels[lvlCounter].deaths += 1;
        dataGathering.levels[lvlCounter].rooms[roomCounter].roomDeaths += 1;
        dataGathering.levels[lvlCounter].rooms[roomCounter].lastTryCollectibles = 0;
        dataGathering.levels[lvlCounter].rooms[roomCounter].lastTryDashes = 0;
        dataGathering.levels[lvlCounter].rooms[roomCounter].lastTryJumps = 0;
        dataGathering.levels[lvlCounter].rooms[roomCounter].lastTryWallJump = 0;
        startRoomLastTryTime = Time.time;
    }

    private void DashCounter()
    {
        dataGathering.levels[lvlCounter].dashes += 1;
        dataGathering.levels[lvlCounter].rooms[roomCounter].roomDashes += 1;
        dataGathering.levels[lvlCounter].rooms[roomCounter].lastTryDashes += 1;
    }

    private void WallJumpCounter()
    {
        dataGathering.levels[lvlCounter].wallJump += 1;
        dataGathering.levels[lvlCounter].rooms[roomCounter].roomWallJump += 1;
        dataGathering.levels[lvlCounter].rooms[roomCounter].lastTryWallJump += 1;
    }
    private void CollectibleGotTry()
    {
        dataGathering.levels[lvlCounter].rooms[roomCounter].collectiblesGot += 1;
        Debug.Log("Collectible got but not ended level");
    }
    public void CollectibleMultiplierCounter()
    {
        dataGathering.levels[lvlCounter].rooms[roomCounter].lastTryCollectibles += 1;

        if (dataGathering.levels[lvlCounter].rooms[roomCounter].returned)
        {
            dataGathering.levels[lvlCounter].rooms[roomCounter].returnedCollectible += 1;
        }
    }

    public void SetReportGathererUserName(string n)
    {
        username = n;
        dataGathering.username = n;
    }

    public void AddLevelTime()
    {
        dataGathering.levels[lvlCounter].totalTime = Time.time - startLevelTime;
        dataGathering.levels[lvlCounter].rooms[roomCounter].totalRoomTime = Time.time - startRoomTime;
        dataGathering.levels[lvlCounter].rooms[roomCounter].lastTryTime = Time.time - startRoomLastTryTime;
    }

    public void AddAnswerValue(int q, int a)
    {
        questionaryAnswers.Add(q.ToString(), a.ToString());//TODO treure això
        data.Add(a.ToString());
    }

    public void SendInfoJSON()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>
        {
            {username + " Report", JsonConvert.SerializeObject(dataGathering) }
        };
        PlayFabManager.Instance.UploadData(dict);
    }

    public void SendInfo()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>
        {
            { username + " Report", CSVManager.GetCommaSeparatedString(data.ToArray())}
        };
        PlayFabManager.Instance.UploadData(dict);
    }

    public void ResetScene(Scene current, Scene next)
    {
        questionaryAnswers.Clear();
        levelRooms.Clear();
        roomCounter = -1;
    }


    public MODE ComputeLevelData(Level LevelData)
    {
        int collectibles = 0;
        int optionalRoomsEntered = 0;
        for (int i = 0; i < LevelData.rooms.Length; i++)
        {
            collectibles += LevelData.rooms[i].lastTryCollectibles;
            if(LevelData.rooms[i].optional)
            {
                optionalRoomsEntered += 1;
            }
        }
        ///Percentage of collectables
        float collectibleDiference = LevelData.totalCollectibles - collectibles;
        float collectiblePercentage = (100 * collectibles) / LevelData.totalCollectibles;
        if (collectiblePercentage > 50f)
        {
            //Tends to be more achiever.
        }
        /// 
        ///Optional Zones
        float optionalPercentage = (100 * optionalRoomsEntered) / LevelData.optionalRooms;
        if(optionalPercentage > 50)
        {

        }

        int nJumpsLevel = 0;

        switch(LevelData.mode)
        {
            case MODE.INITIAL:
                nJumpsLevel = 30;
                
                break;
        }

        if(LevelData.jumps > nJumpsLevel) //Todo Mirar si es més granq ue la meitat
        {

        }
        switch(LevelData.jumps -nJumpsLevel)
        {
            case 0:
                break;
        }

        return MODE.INITIAL;
    }
}
