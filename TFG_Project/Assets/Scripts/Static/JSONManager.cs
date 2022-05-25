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

public class JSONManager 
{
    private static string reportDirectoryName = "Report";
    private static string[] reportsFileName = { "InitialGameTest.json", "AgressiveGameTest.json", "PassiveGameTest.json", "AchieverGameTest.json", "ExplorerGameTest.json" };

    public static TestClass ReadJson() => JsonConvert.DeserializeObject<TestClass>(File.ReadAllText(GetFilePath(reportsFileName[(int)GameManager.Instance.currentSceneMode]), Encoding.GetEncoding("Windows-1252")));

    private static string GetFilePath(string filename) => Application.dataPath + "/" + reportDirectoryName + "/" + filename;
}
