using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AITurn 
{
    static bool active;
    public static bool Activity
    {
        get
        {
            return active;
        }
        set
        {
            active = value;
        }
    }
}
