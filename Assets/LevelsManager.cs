using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsManager : MonoBehaviour
{

    void Start()
    {

    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }
}
