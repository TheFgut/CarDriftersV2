using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{ 
    [SerializeField] private int targetFramerate = 60;
    void Start()
    {

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = targetFramerate;
    }

}
