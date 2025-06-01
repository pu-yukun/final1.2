using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
public class MissionManager2 : MonoBehaviour
{   public static event Action OnHelicopterControlReady;
    public static MissionManager2 Instance;

    [Header("UI Components")]
    public Text missionText;
    public GameObject missionCanvas;
    public string missionDescription;

    [Header("Task Settings")]
    public float finalTaskDuration = 30f;
    public string helicopterTag = "man";

    [Header("Player Control")]
    public GameObject playerObject; // 拖入玩家对象
    public GameObject inventoryUI; // 拖入背包UI对象
    public Camera playerCamera;     // 拖入主摄像头

    // 新增任务状态属性
    public bool IsMission3Completed { get; private set; }

    private string[] missions;
    private int currentMissionIndex = 0;
    private bool isInHelicopterArea = false;
    private float stayTimer = 0f;
    private DistanceTracker distanceTracker;

    void Awake()
    {


        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (!string.IsNullOrEmpty(missionDescription))
        {
            missions = missionDescription.Split(new[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            Debug.LogError("任务描述未设置！");
        }

        distanceTracker = GetComponent<DistanceTracker>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        hostageui.OnDialogueEnd += OnHostageDialogueEnd;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        hostageui.OnDialogueEnd -= OnHostageDialogueEnd;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "DemoScene")
        {
            missionCanvas.SetActive(true);
            UpdateMissionDisplay();
            
            // 确保获取最新对象引用
            playerObject = GameObject.FindGameObjectWithTag("Player");
            inventoryUI = GameObject.Find("InventoryPanel");
            playerCamera = Camera.main;
        }
    }

    void Update()
    {
        // 任务3倒计时
        if (currentMissionIndex == 2 && isInHelicopterArea)
        {
            stayTimer += Time.deltaTime;
            missionText.text = $"坚守撤离点: {Mathf.CeilToInt(finalTaskDuration - stayTimer)}秒";

            if (stayTimer >= finalTaskDuration)
            {
                CompleteMission(2);
            }
        }

        // 检测进入直升机控制
        if (IsMission3Completed && Input.GetKeyDown(KeyCode.L))
        {
            if(CheckCanEnterHelicopter())
            {
                SwitchToHelicopterControl();
            }
        }
    }

    void OnHostageDialogueEnd()
    {
        CompleteMission(0);
        StartMission(1);
    }

    public void StartMission(int index)
    {
        if (index < 0 || index >= missions.Length) return;

        currentMissionIndex = index;
        UpdateMissionDisplay();

        switch (index)
        {
            case 0:
                Debug.Log("开始任务 1: 解救人质");
                break;
            case 1:
                Debug.Log("开始任务 2: 前往直升机撤离点");
                StartCoroutine(ActivateHelicopterAfterDelay(3f));
                break;
            case 2:
                Debug.Log("开始任务 3: 坚守撤离点,等待直升机起飞!");
                break;
        }
    }

    IEnumerator ActivateHelicopterAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject helicopter = GameObject.FindWithTag(helicopterTag);
 DistanceTracker distanceTracker = playerObject.GetComponentInChildren<DistanceTracker>(true);
 if(distanceTracker != null)
        {
            distanceTracker.ChangeTargetTag(helicopterTag); // 切换到直升机tag
            Debug.Log("已切换导航目标至直升机");
        }else{
               Debug.Log("没获得ui组件");
        }



if (helicopter != null)
    {
        helicopter.SetActive(true);
        // 新增：初始禁用控制器
        HelicopterController hc = helicopter.GetComponent<HelicopterController>();
        if(hc != null) hc.enabled = false;
        Debug.Log("直升机已激活（控制器禁用）");
    }
    else
    {
        Debug.LogError("未找到直升机对象！");
    }
    }

    void UpdateMissionDisplay()
    {
        if (currentMissionIndex < missions.Length)
        {
            missionText.text = missions[currentMissionIndex];
        }
    }

    public void CompleteMission(int missionIndex)
    {
        if (missionIndex != currentMissionIndex) return;

        Debug.Log($"完成任务: {missionIndex + 1}");

        if (missionIndex == 1)
        {
            StartMission(2);
        }
        else if (missionIndex == 2)
        {
            IsMission3Completed = true;
            missionText.text = "按 L 进入直升机！进行操控！空格起飞！wasd操控！c下落尽可能离开这个地方！！！！";
            

  OnHelicopterControlReady?.Invoke();
            Debug.Log("已发出直升机控制准备事件");
            
           // enabled = false;


            // 禁用任务管理器自身
            enabled = false;
        }
    }

    bool CheckCanEnterHelicopter()
    {
        if (playerObject == null || inventoryUI == null)
        {
            Debug.LogError("玩家或背包引用丢失！");
            return false;
        }

        GameObject helicopter = GameObject.FindWithTag(helicopterTag);
        return helicopter != null && 
               Vector3.Distance(playerObject.transform.position, 
                               helicopter.transform.position) < 3f;
    }

    void SwitchToHelicopterControl()
    {
        // 禁用玩家控制组件
        PlayerController playerController = playerObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // 禁用背包系统
        inventoryUI.SetActive(false);

        // 转移摄像头
        GameObject helicopter = GameObject.FindWithTag(helicopterTag);
        if (helicopter != null && playerCamera != null)
        {
            // 将玩家设为直升机的子对象
            playerObject.transform.SetParent(helicopter.transform);
            playerObject.transform.localPosition = Vector3.zero;
            
            // 设置摄像机
            playerCamera.transform.SetParent(helicopter.transform);
            playerCamera.transform.localPosition = new Vector3(0, 2, -5);
            playerCamera.transform.localRotation = Quaternion.Euler(20, 0, 0);
            
            // 激活直升机控制器
            HelicopterController hc = helicopter.GetComponent<HelicopterController>();
            if (hc != null)
            {
                hc.enabled = true;
                Debug.Log("直升机控制已激活");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(helicopterTag))
        {
            isInHelicopterArea = true;
            stayTimer = 0f;
            Debug.Log("进入直升机区域");

            if (currentMissionIndex == 1)
            {
                CompleteMission(1);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(helicopterTag))
        {
            isInHelicopterArea = false;
            stayTimer = 0f;
            Debug.Log("离开直升机区域");
        }
    }
}