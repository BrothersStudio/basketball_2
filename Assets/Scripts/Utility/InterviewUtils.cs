using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class AllInterviews
{
    public Interview[] all_interviews;
}

public static class InterviewUtils {

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

        Debug.Log(loaded_json.all_interviews);
        var ret_list = new List<Interview>();
        ret_list.AddRange(loaded_json.all_interviews);
        return ret_list;
    }
}
