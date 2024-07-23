using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassOut : MonoBehaviour
{
    // subcribe to the OnEnergyDepleted event
    private void Awake()
    {
        Needs.Instance.OnEnergyDepleted += PlayerPassOut;
    }
    
    void PlayerPassOut()
    {
        // Player passes out
        // Set player's position to the nearest bed
    }
}
