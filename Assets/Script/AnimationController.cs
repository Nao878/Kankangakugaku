using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationController : MonoBehaviour
{
    public string nextSceneName = "gakkou3";

    public void OnAnimationEnd()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}