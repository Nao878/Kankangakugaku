using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Stage2 : MonoBehaviour
{
    [SerializeField] GameObject boudati;
    [SerializeField] GameObject hora;
    [SerializeField] GameObject finalObj;
    [SerializeField] GameObject TVObj;
    [SerializeField] GameObject buttonSusumu;
    private int horacount = 0;
    private int tvCount = 0;
    [SerializeField] string nextScene;

    public void Hora()
    {
        boudati.SetActive(true);
        ++horacount;
    }
    public void TVFun()
    {
        TVObj.SetActive(true);
        ++tvCount;
    }

    private void Update()
    {
        if (horacount == 2)
        {
            hora.SetActive(true);
        }
        if (horacount == 3)
        {
            finalObj.SetActive(true);
            buttonSusumu.SetActive(false);
        }
        if (tvCount == 2)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
