using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;

public class MissionManager3 : MonoBehaviour
{
    [Header("UI Settings")]
    public Canvas missionCanvas;
    public GameObject missioncanvaparent;
    public Text missionText;
    public Canvas bookBossClueCanvas;
    public string missionDescription =  "前往山野别墅调查线索。清除所有敌人。查找BOSS线索。根据线索找到BOSS寄主。击败BOSS并撤离。";
//public string missionDescriptionPhase2 = "根据线索找到BOSS寄主。";
 //   public string missionDescriptionFinal = "击败BOSS并撤离。";
    [Header("Task Settings")]
    public string taskOneTag = "taskone";
    public string enemyNamePrefix = "swat";
    public string bossClueTag = "findbossclue";
// public string bossClueTag = "findbossclue";
    public string bossTargetTag = "bossrebirth";
    public string manAreaTag = "man";
 
    private string[] missions;
    private int currentMissionIndex = 0;
    private bool isCheckingEnemies = false;
    private bool isInitialized = false;
    private bool isInVillaArea = false; // 新增：是否在别墅区域
    private bool hasFoundBossClue = false; // 新增：是否找到BOSS线索
  private bool taskTwoComplete = false;//action委托完成改true

    public MonsterSpawner3 monsterSpawner; // 引用 MonsterSpawner3
  [Header("线索提示UI")]
    public GameObject gameTextParent; // 新增：包含Text的Canvas父物体
    public Text cluePromptText;       // 新增："按Ctrl获取线索"文本

[Header("最终boss决战！！！")]   public bool isbossdie=false;
    public DistanceTracker distanceTracker;
    public GameObject bookBossUI;
    public Text countdownText;
    public GameObject bossPrefab;
    private Transform bossSpawnPoint;
private bool isBossSpawned = false;
    private float timeLimit = 600f; // 10分钟
    private bool isInBossArea = false;
public GameObject timeend;

    [Header("倒计时设置")]
    public float taskFourTimeLimit = 600f; // 任务四时间限制
    public float taskFiveTimeLimit = 60f;  // 任务五时间限制
    private Coroutine currentTimer;        // 当前运行的计时器协程
    private bool isTimerRunning = false;   // 计时器运行状态

    [Header("重置设置")]
    public GameObject lossGameUI;          // 失败UI界面
    public string playerRespawnTag = "cundang"; // 玩家重生点标签
    public string bossRespawnTag = "bossrebirth"; // BOSS重生点标签
   // public Text countdownText;
  [Header("爆炸设置")]
   public GameObject boom;   

    void Start()
    {
        missionCanvas.gameObject.SetActive(false);
        bookBossClueCanvas.gameObject.SetActive(false);
        StartCoroutine(SceneCheckRoutine());
        StartCoroutine(FindBossRebirth());
 //       StartCoroutine(swatcheck());
    }
IEnumerator FindBossRebirth()
{
    while (true)
    {
        // 查找场景中所有带有 "boss" 标签的对象
        GameObject[] bossObjects = GameObject.FindGameObjectsWithTag("bossrebirth");

        if (bossObjects.Length > 0)
        {
            // 如果找到至少一个带有 "boss" 标签的对象，获取其 Transform
            bossSpawnPoint = bossObjects[0].transform;
            Debug.Log("找到 BOSS 生成点: " + bossSpawnPoint.position);
            yield break; // 找到后退出协程
        }
        else
        {
            Debug.Log("未找到 BOSS 生成点，等待 3 秒后重试...");
            yield return new WaitForSeconds(3f); // 等待 3 秒后重试
        }
    }
}

    IEnumerator SceneCheckRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            if (!isInitialized && SceneManager.GetActiveScene().name == "Scene_A")
            {
                InitializeMission();
                isInitialized = true;
                yield break;
            }
        }
    }

    void InitializeMission()
    {
        missioncanvaparent.SetActive(true);
        missionCanvas.gameObject.SetActive(true);
        Debug.Log("任务3画布已激活");

        missions = missionDescription.Split(new[] { '。' }, System.StringSplitOptions.RemoveEmptyEntries);
        UpdateMissionDisplay();
        StartCoroutine(MissionFlow());

   if(monsterSpawner != null)
        {
            monsterSpawner.OnSwatReduced += HandleSwatReduced;
        }   else
    {
        Debug.LogWarning($"当前任务索引为 {currentMissionIndex}，不符合完成任务二的条件");
    }
/*
    if (bossPrefab != null)
        {
            bossHealth bossHealthComponent = bossPrefab.GetComponent<bossHealth>();
            if (bossHealthComponent != null)
            {
                bossHealthComponent.OnTaskfourend += HandleBossDeath;
                Debug.Log("已订阅 BOSS 死亡事件");
            }
            else
            {
                Debug.LogError("BOSS 预制体未找到 bossHealth 组件！");
            }
        }
*/


    }
/*
IEnumerator swatcheck(){
while(true){
 if(monsterSpawner != null)
        {
            monsterSpawner.OnSwatReduced += HandleSwatReduced;
        }
yield return new WaitForSeconds(0.5f);

}

}*/

   public void HandleBossDeath()
    {
        Debug.Log("接收到 BOSS 死亡事件");
       isbossdie=true;// 设置 isBossSpawned 为 false
      //  CompleteMission(3);    // 完成任务四
    }




  private void HandleSwatReduced()
    {
        Debug.Log("收到SWAT减少事件");
        if(currentMissionIndex == 1)
        {
         //   CompleteMission(1);
         taskTwoComplete=true;
               //  distanceTracker.ChangeTargetTag(bossClueTag);
        }
    }


    IEnumerator MissionFlow()
    {
        yield return StartCoroutine(TaskOne());


        yield return StartCoroutine(TaskTwo());
        yield return StartCoroutine(TaskThree());
         yield return StartCoroutine(TaskFour());
        yield return StartCoroutine(TaskFive());
    }

    IEnumerator TaskOne()
    {
        /*
        bool taskCompleted = false;
    
        while (!taskCompleted)
        {
            // 检测是否在别墅区域
            if (currentMissionIndex == 0 && isInVillaArea)
            {
                taskCompleted = true;
            }
            yield return new WaitForSeconds(2f);
        }*/
  yield return new WaitUntil(() => isInVillaArea);
    Debug.Log("任务一完成");
     
        CompleteMission(0);

    }

    IEnumerator TaskTwo()
    {
        Debug.Log("tasktwo开始");
        isCheckingEnemies = true;
       // StartCoroutine(CheckSwatCountRoutine());
  Debug.Log($"当前 SWAT 已经清扫");

        while (currentMissionIndex == 1&& !taskTwoComplete)
        {
            yield return null;
        }

  if (taskTwoComplete)
        {
            Debug.Log("确认完成任务二");
            CompleteMission(1);
        }

    }
/*
    IEnumerator CheckSwatCountRoutine()
    {
            Debug.Log("tasktwo开始");
        while (currentMissionIndex == 1)
        {     Debug.Log("tasktwo成立");
            int swatCount = GameObject.FindGameObjectsWithTag("swat").Length;
            Debug.Log($"当前 SWAT 数量: {swatCount}");

            if (swatCount <= 9)
            {
                CompleteMission(1);
                yield break;
            }
            yield return new WaitForSeconds(2f);
        }
    }
*/


/*
  IEnumerator TaskTwo()
    {
        Debug.Log("任务二开始，等待SWAT减少事件");
        
        // 保存初始任务索引
        int initialMissionIndex = currentMissionIndex;
        
        // 等待直到任务索引变化或超时（安全机制）
        float timeout = 60f; // 最长等待60秒
        float timer = 0f;
        
        while(currentMissionIndex == initialMissionIndex && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if(timer >= timeout)
        {
            Debug.LogError("任务二超时未完成！");
        }
    }
*/
    void OnDestroy()
    {
        // 取消事件订阅
        if(monsterSpawner != null)
        {
            monsterSpawner.OnSwatReduced -= HandleSwatReduced;
        }
    }


    IEnumerator TaskThree()
    {
        // 改变distance tracker的目标
        distanceTracker.ChangeTargetTag(bossClueTag);

        bool clueFound = false;
        while (!clueFound)
        {
            // 当玩家在触发区域内且按下Ctrl
            if (hasFoundBossClue && Input.GetKeyDown(KeyCode.LeftControl))
            {
                // 隐藏提示UI
                gameTextParent.SetActive(false);
                
                // 显示线索详情UI
                bookBossUI.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                // 等待鼠标点击
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

                // 关闭线索UI
                bookBossUI.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                CompleteMission(2);
                clueFound = true;
            }
            yield return null;
        }

        yield return new WaitForSeconds(2f);
    }

  IEnumerator TaskFour()
{
    // 改变目标
    distanceTracker.ChangeTargetTag(bossTargetTag);

    // 启动倒计时
    currentTimer = StartCoroutine(CountdownTimer(taskFourTimeLimit));

    // 检测进入 man 区域
    bool bossSpawned = false;
    GameObject boss = null;
    bossHealth bossHealthComponent = null;

    while (!bossSpawned)
    {
        if (isInBossArea)
        {
            // 生成 BOSS
            boss = Instantiate(bossPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation);
            boss.tag = "boss"; // 确保标签正确
            bossHealthComponent = boss.GetComponent<bossHealth>();

            if (bossHealthComponent != null)
            {
                Debug.Log("BOSS 已生成，开始检测死亡状态");
            }
            else
            {
                Debug.LogError("BOSS 预制体未找到 bossHealth 组件！");
            }

            isBossSpawned = true;
            bossSpawned = true;
        }
        yield return null;
    }

    // 检查 BOSS 是否被击败
    while (isBossSpawned)
    {
        if (boss == null || (bossHealthComponent != null && bossHealthComponent.isBossDead))
        {
            Debug.Log("BOSS 已被击败，完成任务四");
            CompleteMission(3); // 完成任务四
            isBossSpawned = false;
            if (currentTimer != null) StopCoroutine(currentTimer);
            yield break;
        }
        yield return new WaitForSeconds(0.1f); // 提高检测频率
    }
}

    IEnumerator TaskFive()
    {
        timeend.SetActive(true);
         Invoke("Bageyazida", 61f);
        // 启动任务五倒计时
        currentTimer = StartCoroutine(CountdownTimer(taskFiveTimeLimit));

        while (true)
        {
            if (!isInBossArea||isbossdie)
            {
                CompleteMission(4); // 完成任务五
                if(currentTimer != null) StopCoroutine(currentTimer);
                yield break;
                timeend.SetActive(false);
              
            }
            yield return null;
        }
    }

public void Bageyazida()
{
    Debug.Log("61秒后触发爆炸");

    // 找到 tag 为 "man" 的位置
    GameObject manArea = GameObject.FindGameObjectWithTag("man");
    if (manArea != null)
    {
        // 生成爆炸粒子系统
        GameObject explosionPrefab = Resources.Load<GameObject>("ExplosionParticleSystem"); // 加载预制体
        if (explosionPrefab != null)
        {
            // 生成粒子效果
            GameObject explosion = Instantiate(explosionPrefab, manArea.transform.position, Quaternion.identity);
            Debug.Log("爆炸粒子系统已生成");

            // 找到已存在的 boom 触发器并激活
            GameObject boomTrigger = GameObject.FindGameObjectWithTag("boom");
            if (boomTrigger != null)
            {
                boomTrigger.SetActive(true); // 激活 boom 触发器
                Debug.Log("已激活 boom 触发器");
            }
            else
            {
                Debug.LogError("未找到 tag 为 'boom' 的触发器！");
            }
        }
        else
        {
            Debug.LogError("未找到爆炸粒子系统预制体！");
        }
    }
    else
    {
        Debug.LogError("未找到 tag 为 'man' 的区域！");
    }
}

private void EndGame()
{
    Debug.Log("游戏结束，激活 end 物体的子物体");

    // 查找 tag 为 "End" 的父物体
    GameObject endParent = GameObject.FindGameObjectWithTag("End");
    if (endParent != null)
    {
        Debug.Log($"找到 tag 为 'End' 的父物体: {endParent.name}");

        // 激活所有子物体
        for (int i = 0; i < endParent.transform.childCount; i++)
        {
            Transform child = endParent.transform.GetChild(i);
            child.gameObject.SetActive(true);
            Debug.Log($"已激活子物体: {child.name}");
        }

        // 获取按钮组件并绑定点击事件
        Button endButton = endParent.GetComponentInChildren<Button>();
        if (endButton != null)
        {
            endButton.onClick.AddListener(LoadEndGameScene);
            Debug.Log("已绑定按钮点击事件");
        }
        else
        {
            Debug.LogError("未找到按钮组件！");
        }

        // 添加左键点击检测
        StartCoroutine(CheckForLeftClick());
    }
    else
    {
        Debug.LogError("未找到 tag 为 'End' 的父物体！");
    }
}

// 加载 endgame 场景
public void LoadEndGameScene()
{

 Debug.Log("加载 endgame 场景");

    // 查找玩家对象
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player != null)
    {
        Debug.Log("销毁玩家对象");
        Destroy(player); // 销毁玩家对象
    }



    Debug.Log("加载 endgame 场景");
    SceneManager.LoadScene("endgame");
}

// 检测左键点击
private IEnumerator CheckForLeftClick()
{
    while (true)
    {
        if (Input.GetMouseButtonDown(0)) // 左键点击
        {
            Debug.Log("检测到左键点击，加载 endgame 场景");
            LoadEndGameScene();
            yield break; // 退出协程
        }
        yield return null; // 每帧检测一次
    }
}


    IEnumerator CountdownTimer(float timeLimit)
    {
        isTimerRunning = true;
        float currentTime = timeLimit;
        
        while (currentTime > 0 && isTimerRunning)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay(currentTime);

            if (currentTime <= 0)
            {
                HandleTimeout();
                yield break;
            }
            yield return null;
        }
    }

    void UpdateTimerDisplay(float currentTime)
    {
        countdownText.text = $"剩余时间: {Mathf.FloorToInt(currentTime / 60):00}:{currentTime % 60:00}";
    }

    void HandleTimeout()
    {
        if(currentMissionIndex == 3) // 任务四超时
        {
            ResetBossAndPlayer();
            StartCoroutine(RestartTaskFour());
        }
        else if(currentMissionIndex == 4) // 任务五超时
        {
            // 添加任务五失败处理

   ResetBossAndPlayer();
            StartCoroutine(RestartTaskFour());

            Debug.Log("撤离超时！任务失败！");
        }
    }

    void ResetBossAndPlayer()
    {
        // 重置BOSS
        GameObject boss = GameObject.FindGameObjectWithTag("boss");
        if(boss != null)
        {
            // 传送BOSS
            GameObject respawnPoint = GameObject.FindGameObjectWithTag(bossRespawnTag);
            if(respawnPoint != null)
            {
                boss.transform.position = respawnPoint.transform.position;
                boss.GetComponent<bossHealth>().ResetBossHealth();
            }
            
            // 重置玩家
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            GameObject playerSpawn = GameObject.FindGameObjectWithTag(playerRespawnTag);
            if(player != null && playerSpawn != null)
            {
                player.transform.position = playerSpawn.transform.position;
            }

            // 显示失败UI
            if(lossGameUI != null) lossGameUI.SetActive(true);
        }
    }

    IEnumerator RestartTaskFour()
    {
        yield return new WaitForSeconds(3f);
        if(lossGameUI != null) lossGameUI.SetActive(false);
        currentTimer = StartCoroutine(CountdownTimer(taskFourTimeLimit));
    }


    void UpdateMissionDisplay()
    {
        string statusText = "";
        for (int i = currentMissionIndex; i < missions.Length; i++)
        {
            string status = i == currentMissionIndex ? "▶ " : "○ ";
            statusText += $"{status}{missions[i]}。\n";
        }
        missionText.text = statusText;
    }

    void CompleteMission(int index)
    {
        if (index == currentMissionIndex)
        {
            currentMissionIndex++;
            UpdateMissionDisplay();

            if (currentMissionIndex >= missions.Length)
            {
                missionCanvas.gameObject.SetActive(false);
                Debug.Log("所有任务已完成！");
  Invoke("EndGame",5f);

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(taskOneTag))
        {
            if (currentMissionIndex == 0)
            {
                isInVillaArea = true; // 标记进入别墅区域
                Debug.Log("玩家进入 taskone 区域，开始生成 SWAT");
                Destroy(other.gameObject); // 销毁触发器
                StartCoroutine(CompleteTaskWithDelay(0,0.5f));
            }
        }
        else if (other.CompareTag(bossClueTag))
        {
            if (currentMissionIndex == 2)
            {
                   // 显示提示UI
                gameTextParent.SetActive(true);
                cluePromptText.text = "按 Ctrl 调查线索";
                hasFoundBossClue = true;
                
                // 销毁触发器避免重复触发
                Destroy(other.gameObject);
            }
        }
        else if (other.CompareTag(manAreaTag))
        {
            isInBossArea = true;
        }

        else if (other.CompareTag("boom"))
    {
        Debug.Log("玩家进入爆炸区域，触发任务失败");
        HandleTimeout();
    }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(taskOneTag))
        {
          //  isInVillaArea = false; // 标记离开别墅区域
            Debug.Log("玩家离开 taskone 区域");
        }
        else if (other.CompareTag(manAreaTag))
        {
            isInBossArea = false;
            Debug.Log("玩家离开 boss 区域成功撤离");
        }
          else if (other.CompareTag(bossClueTag))
        {
            // 隐藏提示UI（即使玩家提前离开区域）
            gameTextParent.SetActive(false);
        }
    }

    IEnumerator CompleteTaskWithDelay(int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        CompleteMission(index);
    }
}