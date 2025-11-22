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
        // 一つ目のエンディングを解禁
        PlayerPrefs.SetInt("Ending_1", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(scene2);
    }

    public void PressStart3()
    {
        SceneManager.LoadScene(scene3);
    }
}
