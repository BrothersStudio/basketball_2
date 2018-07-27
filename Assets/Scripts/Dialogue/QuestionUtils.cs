using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class AllQuestions
{
    public List<Question> questions;
}

[System.Serializable]
public class Question
{
    public string tag;
    public string prompt;
}

public static class QuesionUtils
{
    public static List<Question> all_questions = GetAllQuesitions();

    public static Question GetQuestionByTag(string tag)
    {
        Question return_q = null;
        foreach (Question question in all_questions)
        {
            if (question.tag == tag)
            {
                return_q = question;
                break;
            }
        }
        return return_q;
    }

    public static Question GetRandomQuestion()
    {
        return all_questions.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
    }

    public static List<Question> GetAllQuesitions()
    {
        TextAsset question_json = Resources.Load("interview_questions") as TextAsset;
        AllQuestions loaded_json = JsonUtility.FromJson<AllQuestions>(question_json.text);
        return loaded_json.questions;
    }
}
