using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchManager : MonoBehaviour

{

    public string Scene1;
    public string Scene2;

    void Update()

    {

        if (Input.GetKeyDown(KeyCode.Alpha1))

        {

            SceneManager.LoadScene(sceneName: Scene1);

        }

        if (Input.GetKeyDown(KeyCode.Alpha2))

        {

            SceneManager.LoadScene(sceneName: Scene2);

        }

    }

}