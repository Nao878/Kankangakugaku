using UnityEngine;
using System.Collections;

public class HutaKaihei : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; // êÿÇËë÷Ç¶ëŒè€
    [SerializeField] private GameObject targetObject2; // åå›ï\é¶ëŒè€

    void Start()
    {
        StartCoroutine(ToggleActiveCoroutine());
    }

    private IEnumerator ToggleActiveCoroutine()
    {
        while (true)
        {
            if (targetObject != null && targetObject2 != null)
            {
                bool nextActive = !targetObject.activeSelf;
                targetObject.SetActive(nextActive);
                targetObject2.SetActive(!nextActive);
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
