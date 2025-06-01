using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flashexpolsion : MonoBehaviour
{
    // Start is called before the first frame update
  [Header("Settings")]
    public float activeDuration = 1.5f; // 爆炸范围激活持续时间

  [Header("Audio")]
    public AudioClip explosionSound; // 爆炸音效
    private AudioSource audioSource;
 [Tooltip("如果物体可能被重复使用，建议使用PlayClipAtPoint")]
    public bool useFallbackAudio = true;
void Awake()
    {
        //audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
    var audioSource = GetComponent<AudioSource>();
         try
        {
            if (explosionSound != null)
            {
                // 优先使用组件上的AudioSource
                if (audioSource != null)
                {
                    audioSource.PlayOneShot(explosionSound,0.4f);
                }
                // 备用方案：在世界空间播放音效
                else if (useFallbackAudio)
                {
                    AudioSource.PlayClipAtPoint(explosionSound, transform.position);
                }
            }
            else
            {
                Debug.LogWarning("爆炸音效未配置: " + gameObject.name);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("音效播放失败: " + e.Message);
        }
        // 激活后开始计时，1.5秒后禁用
        Invoke("DisableExplosionRange", activeDuration);
    }

    void DisableExplosionRange()
    {
        // 禁用自身
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        // 检测玩家触发
        if (other.CompareTag("Player"))
        {
            FlashEffectManager.Instance.TriggerFlash();
            Debug.Log("我吃闪了！！！！！");
             audioSource.PlayOneShot(explosionSound,1.2f);
        }
    }
}
