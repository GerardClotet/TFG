using UnityEngine;
using System.IO;
using Newtonsoft.Json;

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

    public static TestClass ReadJson()
    {
        return JsonConvert.DeserializeObject<TestClass>(File.ReadAllText(GetFilePath()));
    }

    static string GetFilePath()
    {
        return Application.dataPath + "/" + reportDirectoryName + "/" + reportFileName;
    }
}
