using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
public class MonsterSpawner3 : MonoBehaviour
{
    private gamemanager gameManager;
    [Header("普通怪物设置")]
    public Transform monsterPrefab; // 普通怪物预制体
    public Transform monsterPrefab2; // 新增宫监预制体
    public float minDistance = 5f; // 最小生成距离
    public float maxDistance = 10f; // 最大生成距离
    public float spawnInterval = 1f; // 生成间隔时间
    public float maxHeightAboveGround = 0.5f; // 怪物距地面的最大高度

    [Header("SWAT 设置")]
    public Transform swatPrefab; // SWAT 预制体
    public int swatSpawnCount = 3; // 一次性生成的 SWAT 数量
    public int maxSwatCount = 5; // 最大 SWAT 数量
    public int totalSwatLimit = 10; // SWAT 总数限制

    private Transform playerTransform; // 玩家位置
    private float timer; // 普通怪物计时器
    private LayerMask groundLayer; // 地面层的掩码

    private int currentMonsterCount = 0; // 当前场景中的普通怪物数量
    private int totalMonsterCount = 0; // 整个游戏的普通怪物总数
    private int currentSwatCount = 0; // 当前场景中的 SWAT 数量
    private int totalSwatCount = 0; // 整个游戏的 SWAT 总数

    private bool isInBookZone = false; // 玩家是否在 book 触发器区域
    private bool isInTaskOneZone = false; // 玩家是否在 taskone 触发器区域

    private bool isSpawningEnabled = false; // 是否允许生成怪物

    public event Action OnSwatReduced;

    [Header("SWAT Settings")]
    public int swatReduceStandard = 9; // 需要减少到的目标数量
    public float checkInterval = 1f;    // 检测间隔
    private bool isChecking = false;

    void Start()
    {
        gameManager = gamemanager.Instance;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerTransform == null)
        {
            Debug.LogError("未找到玩家对象！");
            return;
        }

        groundLayer = LayerMask.GetMask("Ground");
        if (groundLayer == 0)
        {
            Debug.LogError("Ground 层未定义！");
            return;
        }

        Debug.Log("怪物生成器初始化完成");
        StartCoroutine(SceneCheckRoutine());
    }

    IEnumerator SceneCheckRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            // 检测当前场景是否为 Scene_A
            if (SceneManager.GetActiveScene().name == "Scene_A")
            {
                // 允许生成怪物
                isSpawningEnabled = true;
                Debug.Log("检测到 Scene_A，允许生成怪物");
                yield break; // 停止协程
            }
        }
    }

    void Update()
    {
        // 如果生成未启用，直接返回
        if (!isSpawningEnabled)
        {
            Debug.Log("生成未启用");
            return;
        }

        // 普通怪物生成逻辑
        if (!isInBookZone && currentMonsterCount < 8 && totalMonsterCount < 70)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                timer = 0f;
                SpawnMonster();
                Debug.Log("生成普通怪物");
            }
        }
    }

    void SpawnMonster()
    {
        bool spawnSuccess = false;
        int attempts = 0;

        while (!spawnSuccess && attempts < 5)
        {
            attempts++;

            // 随机生成水平方向（360度随机角度）
            float randomAngle = UnityEngine.Random.Range(0f, 360f);
            float randomDistance = UnityEngine.Random.Range(minDistance, maxDistance);

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

            // 暂时禁用障碍物检测
            bool hasObstacle = false; // Physics.CheckSphere(groundHit.point + Vector3.up * 0.5f, 1f, LayerMask.GetMask("Obstacle"));

            if (hasGround)
            {
                // 确保生成的怪物在地面以上
                Vector3 spawnPosition = groundHit.point + Vector3.up * UnityEngine.Random.Range(0f, maxHeightAboveGround); // 移除负值
                Transform selectedPrefab = UnityEngine.Random.Range(0, 2) == 0 ? monsterPrefab : monsterPrefab2;
                Transform newMonster = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
                newMonster.name = "enemy_" + System.Guid.NewGuid().ToString();

                currentMonsterCount++;
                totalMonsterCount++;
                spawnSuccess = true;
                Debug.Log($"成功生成怪物，位置: {spawnPosition}");
            }
            else
            {
                Debug.LogWarning($"生成尝试 {attempts} 失败: 无地面");
            }
        }

        if (!spawnSuccess)
        {
            Debug.LogError($"经过 5 次尝试后仍无法找到有效生成位置");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("book"))
        {
            isInBookZone = true;
            Debug.Log("玩家进入 book 区域，停止生成普通怪物");
        }
        else if (other.CompareTag("taskone"))
        {
            isInBookZone = true;
            isInTaskOneZone = true;
            Debug.Log("玩家进入 taskone 区域，开始生成 SWAT");

            // 一次性生成多个 SWAT
            if (currentSwatCount < maxSwatCount && totalSwatCount < totalSwatLimit)
            {
                StartCoroutine(SpawnMultipleSwats());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("book"))
        {
            isInBookZone = false;
        }
        else if (other.CompareTag("taskone"))
        {
            isInTaskOneZone = false;
            isInBookZone = false;
        }
    }

    IEnumerator SpawnMultipleSwats()
    {
        GameObject[] rebirthPoints = GameObject.FindGameObjectsWithTag("rebirth");
        if (rebirthPoints.Length == 0)
        {
            Debug.LogWarning("未找到 rebirth 标记点！");
            yield break;
        }
        StartCoroutine(DelayedCheckStart());
        for (int i = 0; i < swatSpawnCount; i++)
        {
            if (currentSwatCount >= maxSwatCount || totalSwatCount >= totalSwatLimit)
            {
                Debug.Log("达到 SWAT 数量限制，停止生成");
                yield break;
            }

            // 随机选择一个 rebirth 点
            Transform spawnPoint = rebirthPoints[UnityEngine.Random.Range(0, rebirthPoints.Length)].transform;

            // 生成 SWAT
            Transform newSwat = Instantiate(swatPrefab, spawnPoint.position, Quaternion.identity);
            newSwat.name = "swat_" + (totalSwatCount + 1); // 使用数字命名
            currentSwatCount++;
            totalSwatCount++;

            Debug.Log($"成功生成 SWAT，位置: {spawnPoint.position}, 名称: {newSwat.name}");

            yield return new WaitForSeconds(0.1f); // 每次生成间隔 0.5 秒
        }
    }

    IEnumerator DelayedCheckStart()
    {
        yield return new WaitForSeconds(5f); // 等待5秒后开始检测
        StartCoroutine(SwatReductionCheck());
    }

    IEnumerator SwatReductionCheck()
    {
        Debug.Log("开始SWAT数量监控");
        isChecking = true;

        while (isChecking)
        {
            int currentSwat = GameObject.FindGameObjectsWithTag("swat").Length;
            Debug.Log($"[MonsterSpawner] 当前SWAT数量: {currentSwat} (目标: {swatReduceStandard})");

            if (currentSwat <= swatReduceStandard)
            {
                Debug.Log("达到SWAT减少标准，触发事件");
                OnSwatReduced?.Invoke();
                isChecking = false;
                yield break;
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    void OnDestroy()
    {
        // 清理事件订阅
        OnSwatReduced = null;
    }

    public void OnMonsterDestroyed()
    {
        currentMonsterCount--;
        Debug.Log($"普通怪物被销毁，当前数量: {currentMonsterCount}");
    }

    public void OnSwatDestroyed()
    {
        currentSwatCount--;
        Debug.Log($"SWAT 被销毁，当前数量: {currentSwatCount}");
    }
}