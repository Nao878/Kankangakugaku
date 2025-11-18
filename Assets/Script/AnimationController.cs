using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationController : MonoBehaviour
{
    public void OnAnimationEnd()
    {
        SceneManager.LoadScene("gakkou3");
    }
}