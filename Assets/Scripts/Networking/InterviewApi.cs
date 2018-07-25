using System;
using System.IO;
using System.Net;
using UnityEngine;

[Serializable]
public class Interview
{
    public string tag;
    public string message;
    public string level;
    public string user_id;

}

public static class InterviewController
{
    private const string BASE_URL = "http://ec2-18-211-193-5.compute-1.amazonaws.com/";

    public static Interview GetInterview(string tag, string level, string user_id)
    {
        HttpWebRequest request =
            (HttpWebRequest)WebRequest.Create(String.Format("{0}interview?tag={1}&level={2}&user_id={3}", BASE_URL, tag, level, user_id));
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        Interview interview = JsonUtility.FromJson<Interview>(jsonResponse);
        return interview;
    }

    public static string PostInterview(Interview interview)
    {
        HttpWebRequest request =
            (HttpWebRequest)WebRequest.Create(String.Format("{0}interview", BASE_URL));
        
        request.Method = "POST";
        request.ContentType = "application/json";

        string json = JsonUtility.ToJson(interview);

        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();
        }

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        string status = ((HttpWebResponse)response).StatusDescription;

        Debug.Log(String.Format("Interview posted with status {0}", status));

        return status;
    }
}