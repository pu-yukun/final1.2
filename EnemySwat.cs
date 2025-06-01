using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemySwat : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private float detectInterval = 1f;
    [SerializeField] private float followRange = 30f;
    [SerializeField] private float attackRange = 15f;
    [SerializeField] private float fastAttackRange = 6f;
    [SerializeField] private float moveSpeed = 3.5f;

    [Header("Combat Settings")]
    [SerializeField] private float normalFireDelay = 1.8f;
    [SerializeField] private float fastFireDelay = 0.3f;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform scar;

    [Header("Bullet Settings")]
    [SerializeField] private float bulletForce = 200f;

    [Header("Grenade Settings")]
    [SerializeField] private GameObject flashbangPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float throwCooldown = 5f;

    [Header("子弹偏移系数")]
    [SerializeField] public float baseYOffset = 0.2f;
    [SerializeField] public float distanceMultiplier = 0.03f;

    [Header("Flash Settings")]
    [SerializeField] private float flashDuration = 3.5f;


 [Header("巡逻系统")]
    [SerializeField] public string patrolAreaTag = "xunluo";
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float patrolPointDistance = 1f;

    // 新增私有变量
    private Collider patrolArea;
    private bool isPatrolling = false;
    private bool isChasing = false;
    private Vector3 lastKnownPosition;



    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;
    private Vector3 originalScarPosition;
    private bool isDead;
    private bool isThrowing;
    private bool canThrow = true;
    private Coroutine flashCoroutine;
    private Coroutine detectionCoroutine; // 用于存储 DetectionRoutine 协程

    // 新增：布尔常量控制闪光状态
    private bool isFlash = false;
    private readonly int IsFlashingParam = Animator.StringToHash("isFlashing");
    private bool canFire = true;

    // Animation Parameters
    private readonly int MoveParam = Animator.StringToHash("move");
    private readonly int FireParam = Animator.StringToHash("fire");
    private readonly int FastFireParam = Animator.StringToHash("fastfire");
    private readonly int ThrowParam = Animator.StringToHash("throw");
    private readonly int DieTrigger = Animator.StringToHash("die");

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (scar != null)
            originalScarPosition = scar.localPosition;

        agent.speed = moveSpeed;
    }

    private void Start()
    {
        StartCoroutine(GetPlayerRoutine());
        detectionCoroutine = StartCoroutine(DetectionRoutine()); // 启动 DetectionRoutine 并存储协程

  GameObject patrolObj = GameObject.FindGameObjectWithTag(patrolAreaTag);
        if (patrolObj != null) patrolArea = patrolObj.GetComponent<Collider>();

    }

    private void Update()
    {
        if (!isDead && player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private IEnumerator GetPlayerRoutine()
    {
        while (player == null && !isDead)
        {
            if (gamemanager.Instance != null && gamemanager.Instance.player != null)
            {
                player = gamemanager.Instance.player.transform;
            }
            else
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null) player = playerObj.transform;
            }
            yield return new WaitForSeconds(3f);
        }
    }

    public void ApplyFlashEffect()
    {
        if (isDead || isFlash) return; // 如果已经死亡或正在闪光，直接返回

        Debug.Log("已经接收到闪光信号");
        isFlash = true;
        canFire = false; // 禁止开火

        // 停止 DetectionRoutine 协程
        if (detectionCoroutine != null)
        {
            StopCoroutine(detectionCoroutine);
            Debug.Log("DetectionRoutine 已停止");
        }

        // 设置动画状态
        animator.SetBool("fire", false);
        animator.SetBool("isFlashing", true);
        animator.Update(0); // 强制立即更新动画状态机

        // 隐藏 scar
        if (scar != null)
        {
            scar.gameObject.SetActive(false);
        }

        // 启动闪光恢复逻辑
        Invoke("Flashtest", flashDuration);
    }

    public void Flashtest()
    {
        Debug.Log("闪光结束，恢复状态");

        // 恢复状态
        canFire = true;
        isFlash = false;

        // 设置动画状态
        animator.SetBool("fire", true);
        animator.SetBool("isFlashing", false);
        animator.Update(0); // 强制立即更新动画状态机

        // 恢复 scar
        if (scar != null)
        {
            scar.gameObject.SetActive(true);
        }

        // 恢复移动
        agent.isStopped = false;

        // 重启 DetectionRoutine 协程
        detectionCoroutine = StartCoroutine(DetectionRoutine());
        Debug.Log("DetectionRoutine 已重启");
    }

    private IEnumerator DetectionRoutine()
    {
        while (!isDead)
        {
            Debug.Log($"canFire 的值: {canFire}"); // 调试日志

            if (isFlash)
            {
                Debug.Log("协程闪光等待位置！！！");
                yield return new WaitForSeconds(detectInterval);
                continue; // 跳过当前检测
            }

            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.position);
                if (distance <= followRange)
                {
                    if (distance > attackRange)
                    {
                        SetMovementState(true);
                        agent.SetDestination(player.position);
                    }
                    else if (distance <= attackRange && distance > fastAttackRange)
                    {
                        SetMovementState(false);
                        StartCoroutine(FireAttackRoutine(false));
                    }
                    else
                    {
                        if (canThrow && !isThrowing)
                        {
                            StartCoroutine(FastFireWithThrowRoutine());
                        }
                        else
                        {
                            SetMovementState(false);
                            StartCoroutine(FireAttackRoutine(true));
                        }
                    }
                }
                else
                {
                     if (!isPatrolling && patrolArea != null)
                    {
                        StartCoroutine(PatrolRoutine());
                    }
                }
            }
            yield return new WaitForSeconds(detectInterval);
        }
    }


private IEnumerator PatrolRoutine()
    {
        isPatrolling = true;
        agent.speed = patrolSpeed;
        
        while (!isDead && player != null && 
              Vector3.Distance(transform.position, player.position) > followRange)
        {
            Vector3 randomPoint = GetRandomPatrolPoint();
            agent.SetDestination(randomPoint);
            SetMovementState(true);

            // 等待到达目标点
            while (!isDead && 
                  agent.pathPending || 
                  agent.remainingDistance > patrolPointDistance)
            {
                yield return null;
            }
            
            // 短暂停留
            SetMovementState(false);
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
        
        isPatrolling = false;
        agent.speed = moveSpeed;
    }

    // 新增：获取随机巡逻点
    private Vector3 GetRandomPatrolPoint()
    {
        if (patrolArea == null) return transform.position;

        Bounds bounds = patrolArea.bounds;
        Vector3 randomPoint = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            transform.position.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }


// 在EnemySwat类中添加
public void TakeDamage()
{
    if (!isDead)
    {
        // 强制进入追击状态
        isChasing = true;
        
        // 如果正在巡逻则停止
        if (isPatrolling)
        {
            StopCoroutine(PatrolRoutine());
            isPatrolling = false;
            agent.speed = moveSpeed;
        }
        
        Debug.Log("受到伤害，进入追击状态");
    }
}

    private IEnumerator FastFireWithThrowRoutine()
    {
        float timer = 0f;

        while (timer < 3f && !isDead)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (Vector3.Distance(transform.position, player.position) <= fastAttackRange)
        {
            StartCoroutine(ThrowFlashbang());
        }
    }

    private IEnumerator ThrowFlashbang()
    {
        canThrow = false;
        isThrowing = true;
        SetMovementState(false);
        animator.SetBool(FireParam, false);
        animator.SetBool(FastFireParam, false);
        animator.SetTrigger(ThrowParam);

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("throwflash"));

        if (flashbangPrefab && throwPoint)
        {
            GameObject grenade = Instantiate(flashbangPrefab, 
                throwPoint.position, 
                Quaternion.LookRotation(player.position - throwPoint.position));

            if (grenade.TryGetComponent<Rigidbody>(out var rb))
            {
                Vector3 throwDirection = (player.position - throwPoint.position).normalized;
                rb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);
                rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
            }
        }

        yield return new WaitForSeconds(1f);
        isThrowing = false;

        yield return new WaitForSeconds(throwCooldown);
        canThrow = true;
    }

    private IEnumerator FireAttackRoutine(bool isFastFire)
    {
        // 检查是否可以开火
        while (!canFire)
        {
            Debug.Log("无法攻击，吃闪中");
            yield return null; // 暂停协程，等待下一帧
        }

        // 如果可以开火，继续执行攻击逻辑
        animator.SetBool(FireParam, !isFastFire);
        animator.SetBool(FastFireParam, isFastFire);

        while (!isDead && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            // 再次检查是否可以开火
            if (!canFire)
            {
                Debug.Log("无法攻击，吃闪中");
                yield return null; // 暂停协程，等待下一帧
                continue; // 跳过当前循环
            }

            float fireDelay = isFastFire ? fastFireDelay : normalFireDelay;

            if (scar != null)
                scar.localPosition = originalScarPosition - new Vector3(0, 0, 0.02f);

            yield return new WaitForSeconds(fireDelay);

            if (bulletPrefab && bulletSpawnPoint)
            {
                Vector3 playerPos = player.position;
                Vector3 enemyPos = transform.position;
                float horizontalDistance = Vector3.Distance(
                    new Vector3(playerPos.x, 0, playerPos.z),
                    new Vector3(enemyPos.x, 0, enemyPos.z)
                );

                float yOffset = baseYOffset + horizontalDistance * distanceMultiplier;
                Vector3 spawnPosition = bulletSpawnPoint.position;
                spawnPosition.y += 0.01f;
                Vector3 targetPoint = playerPos + new Vector3(0, yOffset, 0);

                GameObject bullet = Instantiate(bulletPrefab, spawnPosition, bulletSpawnPoint.rotation);

                if (bullet.TryGetComponent<Rigidbody>(out var rb))
                {
                    Vector3 direction = (targetPoint - spawnPosition).normalized;
                    rb.AddForce(direction * bulletForce, ForceMode.Impulse);
                }

                if (bullet.TryGetComponent<Projectile>(out var projectile))
                {
                    projectile.OnProjectileHit += (collision) => 
                    {
                        if (collision.gameObject.CompareTag("Player") && 
                            collision.gameObject.TryGetComponent<PlayerHealth>(out var health))
                        {
                            health.TakeDamage(1);
                        }
                    };
                }
            }

            yield return new WaitForSeconds(0.1f);

            if (scar != null)
                scar.localPosition = originalScarPosition;

            yield return new WaitForSeconds(isFastFire ? 0.1f : 0.5f);
        }

        // 结束射击状态
        animator.SetBool(FireParam, false);
        animator.SetBool(FastFireParam, false);
    }

    private void SetMovementState(bool isMoving)
    {
        animator.SetBool(MoveParam, isMoving);
        agent.isStopped = !isMoving;
        agent.speed = isMoving ? moveSpeed : 0f;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        StopAllCoroutines();
        agent.isStopped = true;
        animator.SetTrigger(DieTrigger);
        enabled = false;
    }
}