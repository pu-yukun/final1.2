using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyBower : MonoBehaviour
{
    [Header("Attack Settings")]
    public float keepDistance = 8f; // 保持距离
    public float meleeDistance = 2f; // 近战距离
    public float meleeCooldown = 5f;
    public float rangedCooldown = 3f;
    public int meleeDamage = 5;

    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip meleeSound;
    public AudioClip bowSound;
    public GameObject arrowPrefab; // 箭的预制体
    public Transform shootPoint; // 箭的生成点

    private Transform player; // 通过 gamemanager 获取玩家
    private float lastMeleeTime;
    private float lastRangedTime;
    private bool isAttacking;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent.stoppingDistance = keepDistance; // 设置保持距离

        // 初始状态设置为 idle
        animator.SetBool("ismove", true);
        agent.isStopped = false;

        // 通过 gamemanager 获取玩家引用
        if (gamemanager.Instance != null && gamemanager.Instance.player != null)
        {
            player = gamemanager.Instance.player.transform;
        }
        else
        {
            Debug.LogError("未找到玩家引用！");
        }
    }

    void Update()
    {
        if (player == null) return; // 如果玩家引用为空，直接返回

        float distance = Vector3.Distance(transform.position, player.position);
        FacePlayer();

        // 近战攻击判断
        if (distance <= meleeDistance && CanMeleeAttack())
        {
            StartMeleeAttack();
        }
        // 远程攻击判断
        else if (distance <= keepDistance + 2f && CanRangedAttack())
        {
            StartRangedAttack();
        }
        else if (distance > keepDistance + 2f)
        {
            // 如果玩家不在攻击范围内，则移动
            MoveToPlayer();
        }
        else
        {
            // 如果玩家在保持距离内，则停止移动
            StopMovement();
        }
    }

    bool CanMeleeAttack()
    {
        return Time.time >= lastMeleeTime + meleeCooldown && !isAttacking;
    }

    bool CanRangedAttack()
    {
        return Time.time >= lastRangedTime + rangedCooldown && !isAttacking;
    }

    void StartMeleeAttack()
    {
        isAttacking = true;
        lastMeleeTime = Time.time;
        agent.isStopped = true;

        animator.SetBool("isready", false); // 确保远程攻击状态关闭
        animator.SetBool("iskick", true); // 触发踢人动画
        audioSource.PlayOneShot(meleeSound);
        
        // 通过动画事件调用实际攻击
        Invoke("ResetMeleeAttack", 1.5f); // 根据动画长度调整
    }

    void ResetMeleeAttack()
    {
        animator.SetBool("iskick", false); // 结束踢人动画
        ResetAttack();
    }

    void StartRangedAttack()
    {
        isAttacking = true;
        agent.isStopped = true;

        animator.SetBool("isready", true); // 进入准备状态
        StartCoroutine(RangedAttackSequence());
    }

    IEnumerator RangedAttackSequence()
{
    // 等待进入准备状态（bowready）
    yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("bowready"));
    
    // 等待准备动画播放完成
    yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

    // 直接播放射击动画（假设动画状态名为 "bowshoot"）
    animator.Play("bowshoot"); // 直接播放动画，无需触发器

    // 等待射击动画播放完成
    yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("bowshoot"));
    yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

    // 触发射击
    audioSource.PlayOneShot(bowSound);
    ShootArrow();
    Debug.Log("射击完成");

    // 重置状态
    lastRangedTime = Time.time;
    
    // 返回移动状态
    animator.SetBool("isready", false);
    Debug.Log("动画状态重置");

    ResetAttack();
}

    void ShootArrow()
    {
        if (arrowPrefab && shootPoint && player != null)
        {
            // 计算方向（从射击点到玩家位置）
            Vector3 direction = (player.position - shootPoint.position).normalized;

            // 定义初始旋转 (90, 0, 180)
            Quaternion baseRotation = Quaternion.Euler(90, 0, 180);

            // 生成箭矢并应用旋转
            GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, baseRotation);

            // 设置箭矢属性
            BowProjectile projectile = arrow.GetComponent<BowProjectile>();
            if (projectile != null)
            {
                projectile.InitializeDirection(direction);
            }
            else
            {
                Debug.LogError("箭矢预制体缺少 BowProjectile 组件！");
            }
        }
    }
    void ResetAttack()
    {
        Debug.Log("重置状态");
        isAttacking = false;
        agent.isStopped = false;

        // 重置动画状态
        animator.SetBool("isready", false);
        animator.SetBool("iskick", false);

        // 恢复移动
        MoveToPlayer();
    }

    public void MoveToPlayer()
    {
        if (isAttacking || player == null) return;
        
        // 保持距离移动
        agent.SetDestination(player.position);
        animator.SetBool("ismove", true);
    }

    void StopMovement()
    {
        agent.isStopped = true;
        animator.SetBool("ismove", true);
    }

    void FacePlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // 保持水平旋转
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // 近战攻击伤害检测（通过动画事件调用）
    public void MeleeHit()
    {
        if (player == null) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * 1f, 1f);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerHealth>().TakeDamage(meleeDamage);
            }
        }
    }
}