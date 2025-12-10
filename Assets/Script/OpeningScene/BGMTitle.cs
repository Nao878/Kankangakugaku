using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMTitle : MonoBehaviour
{
    [SerializeField] private AudioSource bgmTitle;
    [SerializeField] GameObject titlePic2;
    private int titleCo = 0;

    public void BgmTitle()
    {
        bgmTitle.Play();
        titlePic2.SetActive(true);
        ++titleCo;
    }

    public void GoEndings()
    {
        SceneManager.LoadScene("endings");
    }

    public void GoSetting()
    {
        SceneManager.LoadScene("Setting");
    }

    void Update()
    {
        if (titleCo == 2)
        {
            SceneManager.LoadScene("stage1");
        }
    }
}