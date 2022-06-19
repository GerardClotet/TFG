using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Text;

public class Question
{
    public string question { get; set; }
    public string[] answers { get; set; }
}
public class TestClass
{
    public Question[] questions { get; set; }
}

public class RoomStats
{
    public string roomName { get; set; }
    public int requiredJumpsRoom { get; set; }
    public int requiredBouncesRoom { get; set; }
    public int requiredDashesRoom { get; set; }
    public float roomTime { get; set; }
}

public class LevelStats
{
    public RoomStats[] room { get; set; }
}

public static class JSONManager 
{
    private static string reportDirectoryName = "Report";
    private static string[] reportsFileName = { "InitialGameTest.json", "AgressiveGameTest.json", "PassiveGameTest.json", "AchieverGameTest.json", "ExplorerGameTest.json" };
    private static string[] statsFileName = { "InitialLevelStats.json", "AgressiveLevelStats.json"};

    public static TestClass ReadTests() => JsonConvert.DeserializeObject<TestClass>(File.ReadAllText(GetFilePath(reportsFileName[(int)GameManager.Instance.currentSceneMode]), Encoding.GetEncoding("Windows-1252")));

    public static LevelStats ReadLevelStats() => JsonConvert.DeserializeObject<LevelStats>(File.ReadAllText(GetFilePath(statsFileName[(int)GameManager.Instance.currentSceneMode])));

    private static string GetFilePath(string filename) => Application.dataPath + "/" + reportDirectoryName + "/" + filename;
}
