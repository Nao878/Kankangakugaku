using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KamiImage2 : MonoBehaviour
{
    public string scene,scene2,scene3;
    public GameObject KamiObj,HaikeiObj;
    private AudioSource audioSource,BGM;
    public AudioClip buttonSE,VoiceSE;

    void Start()
    {
        KamiObj.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    public void PressStart()
    {
        Debug.Log("Press Start!");
        SceneManager.LoadScene(scene);
    }

    public void PressStart2()
    {
        KamiObj.SetActive(true);
        HaikeiObj.SetActive(true);
        BGM.Stop();
        audioSource.PlayOneShot(buttonSE);
        Invoke("Voice", 1f);
        Invoke("LoadScene1", 2f);
    }

    public void PressStart3()
    {
        SceneManager.LoadScene(scene3);
    }

    void Voice()
    {
        audioSource.PlayOneShot(VoiceSE);
    }

    void LoadScene1()
    {
        Debug.Log("LoadScene1");
        SceneManager.LoadScene(scene2);
    }
}
