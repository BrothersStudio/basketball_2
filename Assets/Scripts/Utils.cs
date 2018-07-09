using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{ 
    public static int GetDistance(Vector2 location_1, Vector2 location_2)
    {
        return (int)(Mathf.Abs(location_1.x - location_2.x) + Mathf.Abs(location_1.y - location_2.y));
    }
}
