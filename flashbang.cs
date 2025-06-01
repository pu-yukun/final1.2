using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Flashbang.cs
public class flashbang : MonoBehaviour
{
  [Header("Settings")]
    public float detonateTime = 3f; // 引爆时间
    public float destroyDelay = 2f; // 引爆后销毁延迟

    [Header("References")]
    public GameObject explosionRange; // 子物体（爆炸范围）

    void Start()
    {
        // 初始禁用子物体
        if (explosionRange != null)
            explosionRange.SetActive(false);

        // 预定引爆时间
        Invoke("Detonate", detonateTime);
    }

    void Detonate()
    {
        // 激活子物体（爆炸范围）
        if (explosionRange != null)
            explosionRange.SetActive(true);

        // 预定销毁时间
        Invoke("DestroyFlashbang", destroyDelay);
    }

    void DestroyFlashbang()
    {
        // 销毁父物体（包含子物体）
        Destroy(gameObject);
    }
}