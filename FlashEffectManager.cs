using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlashEffectManager : MonoBehaviour
{
    public static FlashEffectManager Instance;

    [Header("UI References")]
    public GameObject flashPanelParent; // 白色UI图片的父物体
    public Image flashImage; // 白色UI图片的Image组件
    public float flashDuration = 2f; // 闪光总持续时间
    public float fadeInDuration = 0.2f; // 渐入时间
    public float fadeOutDuration = 0.5f; // 渐出时间

    void Awake()
    {
        // 单例模式初始化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        // 初始化隐藏UI
        if (flashPanelParent != null)
            flashPanelParent.SetActive(false);
        else
            Debug.LogError("FlashPanelParent reference is missing!");

        // 初始化透明度
        if (flashImage != null)
            flashImage.color = new Color(1, 1, 1, 0);
        else
            Debug.LogError("FlashImage reference is missing!");
    }

    public void TriggerFlash()
    {
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        // 激活父物体
        if (flashPanelParent != null)
            flashPanelParent.SetActive(true);

        // 渐入效果（从透明到白色）
        float timer = 0;
        while (timer < fadeInDuration)
        {
            if (flashImage != null)
            {
                float alpha = Mathf.Lerp(0, 1, timer / fadeInDuration);
                flashImage.color = new Color(1, 1, 1, alpha);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        // 保持全白
        if (flashImage != null)
            flashImage.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(flashDuration - fadeInDuration - fadeOutDuration);

        // 渐出效果（从白色到透明）
        timer = 0;
        while (timer < fadeOutDuration)
        {
            if (flashImage != null)
            {
                float alpha = Mathf.Lerp(1, 0, timer / fadeOutDuration);
                flashImage.color = new Color(1, 1, 1, alpha);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        // 完全透明
        if (flashImage != null)
            flashImage.color = new Color(1, 1, 1, 0);

        // 隐藏父物体
        if (flashPanelParent != null)
            flashPanelParent.SetActive(false);
    }
}