using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterBirth : MonoBehaviour
{
    [Header("生成设置")]
    public int initialSpawn = 15;    // 初始生成数量
    public int maxTotalSpawn = 60;    // 最大总生成数
    public float respawnInterval = 2f;// 补充间隔
    public float minHorizontalDistance = 4f; // 最小水平距离
    public float maxHorizontalDistance = 30f; // 最大水平距离
    public float maxVerticalDistance = 20f;   // 最大垂直距离（相对于玩家高度）

    [Header("引用")]
    private Transform playerTransform; // 玩家位置
    public GameObject[] monsterPrefabs; // 怪物预制体
    public Collider spawnArea; // 生成区域

    private int currentAlive;     // 当前存活数
    private int totalSpawned;     // 总生成数
    private List<GameObject> activeMonsters = new List<GameObject>();
    private Coroutine respawnCoroutine;

    void Start()
    {
        Debug.Log("MonsterBirth: Start called.");
        TryGetPlayerTransform();

        // 订阅全局死亡事件
      //  zombiehealth.onDead += HandleMonsterDeath;
        Debug.Log("MonsterBirth: Subscribed to zombiehealth.onDead event.");
    }

    void Update()
    {
        if (playerTransform == null)
        {
            TryGetPlayerTransform();
        }
    }

    void OnDestroy()
    {
   //     zombiehealth.onDead -= HandleMonsterDeath;
        Debug.Log("MonsterBirth: Unsubscribed from zombiehealth.onDead event.");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter called with: " + other.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger area.");
            StopAllCoroutines();
            SetMonstersActive(false);
        }
    }

    void TryGetPlayerTransform()
    {
        if (gamemanager.Instance != null && gamemanager.Instance.player != null)
        {
            playerTransform = gamemanager.Instance.player.transform;
            Debug.Log("MonsterBirth: Player transform assigned: " + playerTransform);
        }
        else
        {
            Debug.LogWarning("MonsterBirth: Player transform is null or gamemanager.Instance is null.");
        }
    }

    void SpawnInitialMonsters()
    {
        Debug.Log("MonsterBirth: Spawning initial monsters.");
        int spawnAmount = Mathf.Min(initialSpawn, maxTotalSpawn);
        SpawnMonsters(spawnAmount);
    }

    void SpawnMonsters(int count)
    {
        Debug.Log("MonsterBirth: Attempting to spawn " + count + " monsters.");
        for (int i = 0; i < count; i++)
        {
            if (totalSpawned >= maxTotalSpawn)
            {
                Debug.LogWarning("MonsterBirth: Max total spawn reached: " + maxTotalSpawn);
                return;
            }

            Vector3 spawnPos = GetValidSpawnPosition();
            if (spawnPos != Vector3.zero)
            {
                GameObject monster = CreateMonster(spawnPos);
                if (monster != null)
                {
                    TrackMonster(monster);
                    totalSpawned++;
                    currentAlive++;
                    Debug.Log("MonsterBirth: Monster spawned at: " + spawnPos);
                }
            }
            else
            {
                Debug.LogWarning("MonsterBirth: No valid spawn position found.");
            }
        }
    }

    GameObject CreateMonster(Vector3 position)
    {
        if (monsterPrefabs == null || monsterPrefabs.Length == 0)
        {
            Debug.LogError("Monster prefabs array is empty or not assigned!");
            return null;
        }

        GameObject prefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Length)];
        if (prefab == null)
        {
            Debug.LogError("Selected monster prefab is null!");
            return null;
        }

        GameObject monster = Instantiate(prefab, position, Quaternion.identity);
        Debug.Log("Monster created: " + monster.name);
        return monster;
    }

    void HandleMonsterDeath()
    {
        Debug.Log("Handling monster death. Current alive: " + currentAlive);
        currentAlive--;
        if (ShouldStartRespawn())
        {
            Debug.Log("Starting respawn routine.");
            respawnCoroutine = StartCoroutine(RespawnRoutine());
        }
    }

    IEnumerator RespawnRoutine()
    {
        Debug.Log("Respawn routine started.");
        while (ShouldContinueRespawn())
        {
            yield return new WaitForSeconds(respawnInterval);
            Debug.Log("Attempting to respawn a monster.");
            if (GetValidSpawnPosition() != Vector3.zero)
            {
                SpawnMonsters(1);
            }
            else
            {
                Debug.LogWarning("No valid spawn position found for respawn.");
            }
        }
        Debug.Log("Respawn routine ended.");
        respawnCoroutine = null;
    }

    bool ShouldStartRespawn()
    {
        bool shouldStart = currentAlive < 7 && 
                           totalSpawned < maxTotalSpawn && 
                           respawnCoroutine == null;
        Debug.Log("Should start respawn? " + shouldStart);
        return shouldStart;
    }

    bool ShouldContinueRespawn()
    {
        bool shouldContinue = currentAlive < initialSpawn && 
                              totalSpawned < maxTotalSpawn;
        Debug.Log("Should continue respawn? " + shouldContinue);
        return shouldContinue;
    }

    Vector3 GetValidSpawnPosition(int maxAttempts = 30)
    {
        if (spawnArea == null)
        {
            Debug.LogError("Spawn area is not assigned!");
            return Vector3.zero;
        }

        Debug.Log("Attempting to find a valid spawn position within bounds: " + spawnArea.bounds);
        for (int i = 0; i < maxAttempts; i++)
        {
            // 随机生成X和Z坐标
            Vector3 randomPoint = new Vector3(
                Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                playerTransform.position.y, // 基于玩家高度生成
                Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z)
            );

            // 调整Y轴高度，确保怪物不会生成在地面以下
            randomPoint.y = Mathf.Max(randomPoint.y, playerTransform.position.y);

            if (IsPositionValid(randomPoint))
            {
                Debug.Log("Valid spawn position found: " + randomPoint);
                return randomPoint;
            }
        }
        Debug.LogWarning("No valid spawn position found after " + maxAttempts + " attempts.");
        return Vector3.zero;
    }

    bool IsPositionValid(Vector3 position)
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("Player transform is null, cannot validate position.");
            return false;
        }

        Vector3 playerPos = playerTransform.position;

        // 计算水平距离
        float horizontalDist = Vector3.Distance(
            new Vector3(position.x, 0, position.z),
            new Vector3(playerPos.x, 0, playerPos.z)
        );

        // 计算垂直距离
        float verticalDist = position.y - playerPos.y;

        // 验证条件
        bool isValid = horizontalDist >= minHorizontalDistance &&
                      horizontalDist <= maxHorizontalDistance &&
                      verticalDist >= 0 && // 怪物不能低于玩家
                      verticalDist <= maxVerticalDistance; // 怪物不能高于玩家20f

        Debug.Log($"Position: {position}, Player Position: {playerPos}, " +
                  $"Horizontal Distance: {horizontalDist}, Vertical Distance: {verticalDist}, " +
                  $"Is Valid: {isValid}");

        return isValid;
    }

    void TrackMonster(GameObject monster)
    {
        activeMonsters.Add(monster);
        Debug.Log("Monster tracked: " + monster.name);
    }

    void ClearExistingMonsters()
    {
        Debug.Log("Clearing existing monsters.");
        foreach (var monster in activeMonsters)
        {
            if (monster != null)
            {
                Destroy(monster);
                Debug.Log("Monster destroyed: " + monster.name);
            }
        }
        activeMonsters.Clear();
        currentAlive = 0;
        Debug.Log("All monsters cleared.");
    }

    void SetMonstersActive(bool active)
    {
        Debug.Log("Setting monsters active: " + active);
        foreach (var monster in activeMonsters)
        {
            if (monster != null) monster.SetActive(active);
        }
    }
}