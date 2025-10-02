using System.Collections;
using UnityEngine;

public class ScaleSwitcher : MonoBehaviour
{
    [Header("対象オブジェクト（3つ）")]
    [SerializeField] private GameObject[] targets = new GameObject[3]; // 3つのオブジェクト
    [Header("切り替え間隔（秒）")]
    [SerializeField] private float interval = 1.0f; // サイズ切り替え間隔
    [Header("変化にかける時間（秒）")]
    [SerializeField] private float duration = 0.5f; // 徐々に変化する時間

    private Vector3[] originalScales = new Vector3[3]; // 元のサイズ
    private bool isZero = false;

    private void Start()
    {
        // 各オブジェクトの元のサイズを記録
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null)
                originalScales[i] = targets[i].transform.localScale;
        }
        StartCoroutine(SwitchScaleLoop());
    }

    private IEnumerator SwitchScaleLoop()
    {
        while (true)
        {
            // だんだん大きく/小さくするコルーチン
            yield return StartCoroutine(ScaleTransition(isZero ? Vector3.zero : originalScales[0], isZero ? originalScales[0] : Vector3.zero));
            isZero = !isZero;
            yield return new WaitForSeconds(interval);
        }
    }

    // すべてのオブジェクトのスケールを徐々に変化させる
    private IEnumerator ScaleTransition(Vector3 from, Vector3 to)
    {
        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null)
                {
                    targets[i].transform.localScale = Vector3.Lerp(from, to, t);
                }
            }
            time += Time.deltaTime;
            yield return null;
        }
        // 最終値をセット
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null)
            {
                targets[i].transform.localScale = to;
            }
        }
    }
}
