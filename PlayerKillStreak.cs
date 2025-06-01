using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerKillStreak : MonoBehaviour
{
    public static PlayerKillStreak Instance; // 单例模式方便访问

    [Header("UI Settings")]
    public Image killIconDisplay;
    public Sprite[] killSprites; // 顺序存放1-6杀图标

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip[] killSounds; // 顺序存放1-6杀音效

    [Header("Settings")]
    public float resetTime = 5f;

    private int _currentKills;
    private Coroutine _resetCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            killIconDisplay.enabled = false; // 初始隐藏图标
            _currentKills=0;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterKill()
    {
        _currentKills = Mathf.Min(_currentKills + 1, 6); // 最大6杀
        
        UpdateUI();
        PlaySound();
        ResetTimer();
    }

    private void UpdateUI()
    {
        int index = Mathf.Clamp(_currentKills - 1, 0, 5);
        
        if (killSprites.Length > index)
        {
            killIconDisplay.sprite = killSprites[index];
            killIconDisplay.enabled = true;
        }
    }

    private void PlaySound()
    {
        int index = Mathf.Clamp(_currentKills - 1, 0, 5);
        
        if (killSounds.Length > index && killSounds[index] != null)
        {
            audioSource.PlayOneShot(killSounds[index]);
        }
    }

    private void ResetTimer()
    {
        if (_resetCoroutine != null)
            StopCoroutine(_resetCoroutine);
        
        _resetCoroutine = StartCoroutine(ResetCountdown());
    }

    private IEnumerator ResetCountdown()
    {
        yield return new WaitForSeconds(resetTime);
        _currentKills = 0;
        killIconDisplay.enabled = false; // 隐藏图标
    }
}