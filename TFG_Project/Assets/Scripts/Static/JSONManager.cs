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
    private static string reportFileName = "PostGame.json";

    public static TestClass ReadJson() => JsonConvert.DeserializeObject<TestClass>(File.ReadAllText(GetFilePath(), Encoding.GetEncoding("Windows-1252")));

    private static string GetFilePath() => Application.dataPath + "/" + reportDirectoryName + "/" + reportFileName;
}
