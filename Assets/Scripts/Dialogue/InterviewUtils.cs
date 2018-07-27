using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class AllInterviews
{
    public List<Interview> Interviews;
}

[System.Serializable]
public class Interview
{
    public string level;
    public string tag;
    public string user_id;
    public string message;
}

public static class InterviewUtils 
{
    public static List<Interview> default_interviews = GetDefaultInterviews();

    public static string[] GetTagList()
    {
        List<string> tag_list = new List<string>();
        foreach (Interview interview in default_interviews)
        {
            tag_list.Add(interview.tag);
        }
        return tag_list.ToArray();
    }

    public static List<Interview> GetDefaultInterviews()
    {
        TextAsset interview_json = Resources.Load("default_interviews") as TextAsset;
        AllInterviews loaded_json = JsonUtility.FromJson<AllInterviews>(interview_json.text);
        return loaded_json.Interviews;
    }
}
