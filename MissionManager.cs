using UnityEngine;  
using UnityEngine.UI;  
using UnityEngine.Video;  

public class MissionManager : MonoBehaviour  
{  
    [Header("UI References")]  
    public GameObject missionListUI; // 任务列表UI  
    public Text missionText;         // 任务文本  
    public GameObject successUI;     // 任务成功UI  
    public RawImage videoRawImage;   // 视频播放组件  
    public VideoPlayer videoPlayer;  // 视频播放器  
    public Button nextMapButton;     // 下一步按钮  
    public Text timerText;           // 撤离点计时器文本  

    [Header("Mission Settings")]  
    public string missionSequence = "任务一到达撤离点。在撤离点坚守。";  
    public int rescueWaitTime = 10;  // 坚守时间（现在为整数）  
    public Transform evacuationPoint; // 撤离点位置  
    public LayerMask playerLayer;    // 玩家层级  

    private string[] missions;  
    private int currentMissionIndex;  
    private bool isEvacuationComplete;  
    private bool isPlayerInEvacuationZone;  
    private PlayerController playerController;  

    private float timer = 0f; // 计时器，用于追踪经过的时间  

    void Start()  
    {  
        // 初始化任务系统  
        missions = missionSequence.Split('。');  
        playerController = FindObjectOfType<PlayerController>();  
        successUI.SetActive(false);  
        timerText.gameObject.SetActive(false); // 初始时禁用计时器文本  
        UpdateMissionDisplay();  

        // 绑定按钮事件  
        nextMapButton.onClick.AddListener(OnNextMapClicked);  
    }  

    void Update()  
    {  
        // 检测第二个任务的计时器  
        if (currentMissionIndex == 1 && isPlayerInEvacuationZone)  
        {  
            timer += Time.deltaTime; // 记录经过的时间  

            // 每秒更新一次倒计时  
            if (timer >= 1f) // 如果过去了一秒  
            {  
                rescueWaitTime--; // 整数倒计时递减  
                UpdateTimerDisplay();  
                timer = 0f; // 更新倒计时后重置计时器  

                if (rescueWaitTime <= 0)  
                {  
                    CompleteCurrentMission();  
                    ShowSuccessUI();  
                }  
            }  
        }  
    }  

    // 检测玩家进入撤离点  
    private void OnTriggerEnter(Collider other)  
    {  
        if (other.CompareTag("Player"))  
        {  
            if (currentMissionIndex == 0)  
            {  
                CompleteCurrentMission(); // 完成第一个任务  
                isPlayerInEvacuationZone = true;  
                timerText.gameObject.SetActive(true); // 激活计时器文本  
            }  
            else if (currentMissionIndex == 1)  
            {  
                isPlayerInEvacuationZone = true;  
                Debug.Log("重新进入撤离点，计时器恢复");  
            }  
        }  
    }  

    // 检测玩家离开撤离点  
    private void OnTriggerExit(Collider other)  
    {  
        if (other.CompareTag("Player") && currentMissionIndex == 1)  
        {  
            isPlayerInEvacuationZone = false;  
            Debug.Log("离开撤离点，计时器暂停");  
        }  
    }  

    void CompleteCurrentMission()  
    {  
        if (currentMissionIndex < missions.Length - 1)  
        {  
            currentMissionIndex++;  
            UpdateMissionDisplay();  
        }  
    }  

    void UpdateMissionDisplay()  
    {  
        missionText.text = $"当前任务 ({currentMissionIndex + 1}/{missions.Length}):\n{missions[currentMissionIndex]}";  
    }  

    void UpdateTimerDisplay()  
    {  
        timerText.text = $"撤离时间剩余时间: {rescueWaitTime} 秒"; // 显示整数  
    }  

    void ShowSuccessUI()  
    {  
        // 禁用玩家控制（保留左键）  
        // playerController.DisableControlsExceptPrimary();  

        // 销毁所有敌人  
        DestroyEnemies();  

        // 隐藏任务列表和计时器  
        missionListUI.SetActive(false);  
        timerText.gameObject.SetActive(false);  

        // 显示成功UI并播放视频  
        successUI.SetActive(true);  
        videoPlayer.Play();  
    }  

    void DestroyEnemies()  
    {  
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");  
        foreach (var enemy in enemies)  
        {  
            Destroy(enemy);  
        }  
    }  

   public void OnNextMapClicked()  
    {  
        // 调用UIchange的场景切换方法  
        // FindObjectOfType<UIchange>().LoadMap2();  

        Application.LoadLevel("demoscene");   
        // SceneManager.LoadScene("demoscene");  
    }  
}  