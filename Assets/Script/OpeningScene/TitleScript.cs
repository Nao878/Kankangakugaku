using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    public string scene;
    public string scene2;
    public string scene3;

    public void PressStart()
    {
        Debug.Log("Press Start!");
        SceneManager.LoadScene(scene);    
    }

    public void PressStart2()
    {
        SceneManager.LoadScene(scene2);
    }

    public void PressStart3()
    {
        SceneManager.LoadScene(scene3);
    }
}
