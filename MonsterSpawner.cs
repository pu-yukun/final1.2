using UnityEngine;
using System.Collections;
public class MonsterSpawner : MonoBehaviour
{
    public Transform monsterPrefab; // 怪物预制体
        public Transform monsterPrefab2;   // 新增宫监预制体
    public float minDistance = 5f; // 最小生成距离
    public float maxDistance = 10f; // 最大生成距离
    public float spawnInterval = 1f; // 生成间隔时间
    public float maxHeightAboveGround = 0.5f; // 怪物距地面的最大高度

    private Transform playerTransform; // 玩家位置
    private float timer; // 计时器
    private LayerMask groundLayer; // 地面层的掩码

    private int currentMonsterCount = 0; // 当前场景中的怪物数量
    private int totalMonsterCount = 0; // 整个游戏的怪物总数
    private bool isInBookZone = false; // 玩家是否在 book 触发器区域
public bool isrebirth=false;//复活点重新出怪

[Header("障碍物检测设置")]
public LayerMask obstacleLayer; // 在Inspector中设置为墙壁等障碍物的层
public float obstacleCheckRadius = 1f; // 检测半径
public int maxSpawnAttempts = 5; // 最大生成尝试次数
 [Header("精英怪设置")]
    public GameObject eliteMonsterPrefab1; // 骑士精英怪预制体
  
 private bool isBookSpawnActive = false;
    private bool isDefendSpawnActive = false;
    private int defendSpawnCount = 0;
private int bookSpawnCount = 0;    // 图书区计数器

    void Start()
    {
        // 初始化玩家引用
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerTransform == null)
        {
            Debug.LogError("未找到玩家对象！");
            this.enabled = false; // 禁用脚本
            return;
        }

        // 初始化地面层
        groundLayer = LayerMask.GetMask("Ground");
        if (groundLayer == 0)
        {
            Debug.LogError("Ground 层未定义！");
            this.enabled = false; // 禁用脚本
            return;
        }

        Debug.Log("怪物生成器初始化完成");
    }

    void Update()
    {  Debug.Log($"当前 ismonsterready 状态: {gamemanager.ismonsterready}");
        // 如果玩家在 book 区域，直接返回
        if (isInBookZone)
        {
            Debug.Log("玩家在 book 区域，停止生成怪物");
            return;
        }

        // 如果当前怪物数量 >= 15 或总数 >= 50，直接返回
        if (currentMonsterCount >= 20 || totalMonsterCount >= 70)
        {
            Debug.Log($"怪物数量限制：当前 {currentMonsterCount}，总数 {totalMonsterCount}");
            return;
        }

        // 如果当前怪物数量 < 6，开始生成
        if (currentMonsterCount < 8)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval&&gamemanager.ismonsterready)
            {
                timer = 0f;
                SpawnMonster();

                  Debug.Log("生成怪物jpg");
                   Debug.Log($"怪物数量限制：当前 {currentMonsterCount}");
            }
        }
    }

void SpawnMonster()
{
    bool spawnSuccess = false;
    int attempts = 0;

    while (!spawnSuccess && attempts < maxSpawnAttempts)
    {
        attempts++;

        // 随机生成水平方向（360度随机角度）
        float randomAngle = Random.Range(0f, 360f);
        float randomDistance = Random.Range(minDistance, maxDistance);

        // 将角度转换为方向向量
        Vector3 randomDirection = new Vector3(
            Mathf.Sin(randomAngle * Mathf.Deg2Rad),
            0f,
            Mathf.Cos(randomAngle * Mathf.Deg2Rad)
        ).normalized;

        // 计算候选位置
        Vector3 candidatePosition = playerTransform.position + randomDirection * randomDistance;
        candidatePosition.y += 200f; // 从高空开始检测

        // 地面检测
        RaycastHit groundHit;
        bool hasGround = Physics.Raycast(candidatePosition, Vector3.down, out groundHit, Mathf.Infinity, groundLayer);

        // 障碍物检测（球体检测）
        bool hasObstacle = Physics.CheckSphere(
            groundHit.point + Vector3.up * 0.5f, // 检测点抬升防止地面穿透
            obstacleCheckRadius,
            obstacleLayer
        );

        // 位置有效性验证
        if (hasGround && !hasObstacle)
        {
            // 最终生成位置
            Vector3 spawnPosition = groundHit.point + Vector3.up * Random.Range(-1f, maxHeightAboveGround);
 // 随机选择预制体类型（50%概率）
                Transform selectedPrefab = Random.Range(0, 2) == 0 ? 
                                         monsterPrefab : 
                                         monsterPrefab2;

                // 生成怪物（支持两种预制体）
                Transform newMonster = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
                newMonster.name = "enemy_" + System.Guid.NewGuid().ToString();
                newMonster.SetParent(null);
/*
           zombiehealth healthScript = newMonster.GetComponent<zombiehealth>();
            if (healthScript != null)
            {
                healthScript.bugTriggerTag = bugTriggerTag;
                healthScript.deliverPointName = deliverPointName;
            }
            else
            {
                Debug.LogWarning($"怪物 {newMonster.name} 未找到 zombiehealth 组件！");
            }
*/

            // 更新计数
            currentMonsterCount++;
            totalMonsterCount++;
            Debug.Log($"成功生成怪物，位置: {spawnPosition}");
            spawnSuccess = true;
        }
        else
        {
            Debug.LogWarning($"生成尝试 {attempts} 失败: " + 
                           (hasGround ? "发现障碍物" : "无地面"));
        }
    }

    if (!spawnSuccess)
    {
        Debug.LogError($"经过 {maxSpawnAttempts} 次尝试后仍无法找到有效生成位置");
    }
}


// 替换原有方向生成代码为更均匀的分布算法
Vector3 GetRandomDirection()
{
    // 使用单位圆均匀分布算法
    float angle = Random.Range(0f, Mathf.PI * 2);
    float radius = Mathf.Sqrt(Random.Range(0f, 1f)); // 确保均匀分布

    return new Vector3(
        Mathf.Cos(angle) * radius,
        0f,
        Mathf.Sin(angle) * radius
    ).normalized;
}



    // 触发器进入事件
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("book"))
        {
            isInBookZone = true;
            Debug.Log("玩家进入 book 区域");//进入book区域进入协程进行刷怪
     if (!isBookSpawnActive&& bookSpawnCount++<=2)
            {
                StartCoroutine(HandleBookSpawn());
            }


        }

// 撤离点坚守检测
        if (other.CompareTag("defendplace")) // 注意检查标签拼写一致性
        {
            Debug.Log("玩家进入坚守区域");
            
            // 启动防守区生成逻辑
            if (!isDefendSpawnActive && defendSpawnCount <= 3)
            {
                StartCoroutine(HandleDefendSpawn());
            }
        }


    }
//进入book tag区域进行相应的查找名字为knightrebirth的复活的在此利用instantiate复活
 IEnumerator HandleBookSpawn()
    {
        isBookSpawnActive = true;
        
        // 等待1秒
        yield return new WaitForSeconds(1f);

     for (int i = 0; i < 2; i++)
        {
            // 查找生成点
            GameObject spawnPoint = GameObject.Find("knightrebirth1");
            if (spawnPoint != null && eliteMonsterPrefab1 != null)
            {
                Instantiate(eliteMonsterPrefab1, spawnPoint.transform.position, Quaternion.identity);
                 bookSpawnCount++; // 使用正确的计数器
                Debug.Log($"生成book区精英怪 ({   bookSpawnCount++}/2)");
            }
            else
            {
                Debug.LogWarning("找不到knightrebirth2生成点或未设置预制体");
                break;
            }

            // 第一次等待，第二次不刷新
            if (i < 1) yield return new WaitForSeconds(5f);

        }
        isBookSpawnActive = false;
    }
//坚守点为怪物设置for循环生成一次后再次生成，i值为生成怪物数量

    IEnumerator HandleDefendSpawn()
    {
        isDefendSpawnActive = true;
   yield return new WaitForSeconds(1f);
        for (int i = 0; i < 3; i++)
        {
            // 查找生成点
            GameObject spawnPoint = GameObject.Find("knightrebirth2");
            if (spawnPoint != null && eliteMonsterPrefab1 != null)
            {
                Instantiate(eliteMonsterPrefab1, spawnPoint.transform.position, Quaternion.identity);
                defendSpawnCount++;
                Debug.Log($"生成防守区精英怪 ({defendSpawnCount}/3)");
            }
            else
            {
                Debug.LogWarning("找不到knightrebirth2生成点或未设置预制体");
                break;
            }
isrebirth=true;
            // 最后一次不等待
            if (i < 2) yield return new WaitForSeconds(7f);
        }

        isDefendSpawnActive = false;
    }





    // 触发器离开事件
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("book"))
        {
            isInBookZone = false;
            Debug.Log("玩家离开 book 区域");
        }
    }

    // 怪物销毁时调用
    public void OnMonsterDestroyed()
    {
        currentMonsterCount--;
        Debug.Log($"怪物被销毁，当前数量: {currentMonsterCount}");
    }
}