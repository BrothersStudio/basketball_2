using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseController : MonoBehaviour
{
    Phase current_phase = Phase.BlueAct;

    void ChangePhase()
    {
        if (current_phase == Phase.BlueAct)
        {
            current_phase = Phase.RedAct;
        }
        else if (current_phase == Phase.RedAct)
        {
            current_phase = Phase.BlueAct;
        }
    }
}

public enum Phase
{
    BlueAct,
    RedAct
}