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
        public string roomName;
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
    private RoomChange[] levelRooms;
    private RoomChange previousRoom = null;

    [HideInInspector] public bool Explorer { get; private set; }
    [HideInInspector] public bool Achiever { get; private set; }
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

        Explorer = false;
        Achiever = false;
    }

    public void GetNewLevel(MODE lvlMode)
    {
        Explorer = false;
        Achiever = false;

        lvlCounter++;
        dataGathering.levels[lvlCounter] = new Level();
        dataGathering.levels[lvlCounter].mode = lvlMode;

        RoomChange[] rmch = FindObjectsOfType<RoomChange>();
        dataGathering.levels[lvlCounter].rooms = new Room[rmch.Length];
        levelRooms = new RoomChange[rmch.Length];

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
        //Check if we have passed in this room
        for (int i = 0; i < levelRooms.Length; i++)
        {
            if (levelRooms[i] == enteringRoom)
            {
                dataGathering.levels[lvlCounter].rooms[roomCounter].totalRoomTime += Time.time - startRoomTime;
                dataGathering.levels[lvlCounter].rooms[roomCounter].lastTryTime = Time.time - startRoomLastTryTime;
                roomCounter = i;
                dataGathering.levels[lvlCounter].rooms[roomCounter].returned = true;
                startRoomTime = Time.time;
                startRoomLastTryTime = Time.time;
                return;
            }
        }

        if (roomCounter > -1)
        {
            dataGathering.levels[lvlCounter].rooms[roomCounter].totalRoomTime += Time.time - startRoomTime;
            dataGathering.levels[lvlCounter].rooms[roomCounter].lastTryTime = Time.time - startRoomLastTryTime;
        }
        startRoomTime = Time.time;
        startRoomLastTryTime = Time.time;
        //First time we pass this room
        char pos = enteringRoom.name[enteringRoom.name.Length - 1];
        int value = (int)char.GetNumericValue(pos);
        levelRooms[value -1] = enteringRoom;
        roomCounter = value - 1;
        dataGathering.levels[lvlCounter].rooms[roomCounter] = new Room();
        dataGathering.levels[lvlCounter].rooms[roomCounter].optional = enteringRoom.GetRoomStatus();
        dataGathering.levels[lvlCounter].rooms[roomCounter].roomName = enteringRoom.name;




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
        if (levelRooms != null)
        {
            Array.Clear(levelRooms, 0, levelRooms.Length);
        }
        roomCounter = -1;
    }


    public MODE ComputeLevelData()
    {
        Level LevelData = dataGathering.levels[lvlCounter];
        int collectibles = 0;
        int optionalRoomsEntered = 0;
        float Agressive_Passive = 0; //negative is agressive, positive is passive

        int nJumpsLevel = 0;
        int nDashesLevel = 0;
        int nWallJumpLevel = 0;
        float requiredTime = 0;
        LevelStats stats = JSONManager.ReadLevelStats();

        for(int j = 0; j < stats.room.Length; j++)
        {
            if (LevelData.rooms[j] != null)
            {
                nJumpsLevel += stats.room[j].requiredJumpsRoom;
                nDashesLevel += stats.room[j].requiredDashesRoom;
                nWallJumpLevel += stats.room[j].requiredBouncesRoom;
                requiredTime += stats.room[j].roomTime;
            }
        }

        for (int i = 0; i < LevelData.rooms.Length; i++)
        {
            if (LevelData.rooms[i] != null)
            {
                collectibles += LevelData.rooms[i].lastTryCollectibles;
                if (LevelData.rooms[i].optional)
                {
                    optionalRoomsEntered += 1;
                }
            }
        }
        ///Percentage of collectables
        float collectibleDiference = LevelData.totalCollectibles - collectibles;
        float collectiblePercentage = (100 * collectibles) / LevelData.totalCollectibles;
        if (collectiblePercentage > 50f)
        {
            //Tends to be more achiever.
            Achiever = true;
        }
        /// 
        ///Optional Zones
        float optionalPercentage = (100 * optionalRoomsEntered) / LevelData.optionalRooms;
        if(optionalPercentage > 50)
        {
            Explorer = true;
        }

        //Jumps
        float difJumps = nJumpsLevel - LevelData.jumps;
        if( difJumps <= nJumpsLevel)
        {
            Debug.Log("Less than a double"); //Probably Assegurador 
            Agressive_Passive = 1;
        }
        else if(difJumps > nJumpsLevel)
        {
            Debug.Log("More than double"); //Probably an Agressive
            Agressive_Passive += -1;
        }

        ///Dashes
        ///
        float difDash = nDashesLevel - LevelData.dashes;
        if (difDash <= nDashesLevel)
        {
            Debug.Log("Less than a double"); //Probably Assegurador 
            Agressive_Passive += 1;

        }
        else if (difDash > nDashesLevel)
        {
            Debug.Log("More than double"); //Probably an Agressive
            Agressive_Passive += -1;
        }


        for (int k = 0; k < LevelData.rooms.Length; k++)
        {
            if (LevelData.rooms[k] != null)
            {
                float dBounce = LevelData.rooms[k].lastTryWallJump - stats.room[k].requiredBouncesRoom;
                float dJumps = LevelData.rooms[k].lastTryJumps - stats.room[k].requiredJumpsRoom;
                float dDash = LevelData.rooms[k].lastTryDashes - stats.room[k].requiredDashesRoom;
                
                float avgJumps = LevelData.rooms[k].roomJumps / LevelData.rooms[k].roomsDeaths;
                float avgDash = LevelData.rooms[k].roomDashes / LevelData.rooms[k].roomsDeaths;
                float avgWallJump = LevelData.rooms[k].roomWallJump / LevelData.rooms[k].roomsDeaths;
                float avgTime = LevelData.rooms[k].totalRoomTime / LevelData.rooms[k].roomsDeath;
                
                if (avgWallJump <= stats.room[k].requiredBouncesRoom)
                {
                    Agressive_Passive -= 1;
                }
                else
                {
                    Agressive_Passive += 1;
                }

                if (avgJumps <= stats.room[k].requiredJumpsRoom)
                {
                    Agressive_Passive -= 1;
                }
                else
                {
                    Agressive_Passive += 1;
                }

                if (avgDash <= stats.room[k].requiredDashesRoom)
                {
                    Agressive_Passive -= 1;
                }
                else
                {
                    Agressive_Passive += 1;
                }
                
                if(avgTime <= stats.room[k].roomTime)
                {
                    Agressive_Passive -= 1;
                }
                else
                {
                    Agressive_Passive += 1;
                }                
            }
        }

        if(Agressive_Passive < 0) //AgresiveProfile
        {
            return MODE.AGRESSIVE_EXPLORER;
        }
        else
        {
            return MODE.PASSIVE_EXPLORER;         
        }
        //LevelData.deaths


        return MODE.INITIAL;
    }
}
