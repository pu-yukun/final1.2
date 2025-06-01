using UnityEngine;
using UnityEngine.UI;
using System;

public class swathealth : MonoBehaviour
{

    public bool IsDead { get; private set; } = false; // 默认值为 false

    private bool isDead = false; // 添加一个状态标识
    public PlayerKillStreak PlayerKillStreak;
    public float health = 123f; // 初始生命值
    public Animator animator; // 动画控制器
    public EnemySwat EnemySwat; // 引用 EnemySwat 脚本

    [Header("UI 设置")]
    public Image zhanImage; // 近战提示的 Image 组件

    [Header("Bug修复设置")]
    public string bugTriggerTag = "kabugreplace"; // 卡bug区域触发器标签
    public string deliverPointName = "deliver";    // 传送目标点名称

    private bool isInBugZone = false; // 是否在卡bug区域

    void Start()
    {
        health = 123f; // 强制覆盖初始值
        EnemySwat = GetComponent<EnemySwat>(); // 确保正确获取 EnemySwat 组件

        if (zhanImage == null)
        {
            Debug.LogError("zhanImage 未在 Inspector 中设置！");
        }
        else
        {
            SetZhanImageVisibility(false); // 初始隐藏
        }
    }

    void Update()
    {
        Debug.Log($"{gameObject.name} 测试生命值: {health}");

        if (!isDead && transform.position.y < -50f)
        {
            Debug.LogWarning($"{gameObject.name} 坠落死亡！当前位置Y: {transform.position.y}");
            destroy();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("受到攻击了！");

        if (other.CompareTag("flash"))
        {
            Debug.Log("吃闪了僵尸速度减少");
            TakeDamage(10);
            EnemySwat.ApplyFlashEffect();
        }
        else if (other.CompareTag("boom"))
        {
            Debug.Log("遭受爆炸攻击");
            Invoke("handlebbom", 1f);
        }
        else if (other.CompareTag("knife") || other.CompareTag("knifetwo"))
        {
            Debug.Log("近战攻击！！！！！！");
            TakeDamage(other.CompareTag("knife") ? 500 : 800);
            SetZhanImageVisibility(true);
            Invoke("HideZhanImageAfterDelay", 1.5f);
        }
        else if (other.CompareTag("Bullet") || other.CompareTag("RPG"))
        {
            inventory inv = FindObjectOfType<inventory>();
            if (inv != null)
            {
                int damage = inv.GetCurrentWeaponDamage();
                TakeDamage(damage);
                Debug.Log("受伤: " + damage);
            }
        }
        else if (other.CompareTag(bugTriggerTag))
        {
            Debug.Log($"{gameObject.name} 进入卡bug区域");
            isInBugZone = true;
            DeliverMonster();
        }
        else
        {
            Debug.Log("标签错误: " + other.tag);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        health = Mathf.Max(health, 0);

        Debug.Log($"{gameObject.name} 当前生命值: {health}，受到伤害: {damage}");

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetTrigger("isdeath"); // 播放死亡动画

        if (PlayerKillStreak.Instance != null)
        {
            PlayerKillStreak.Instance.RegisterKill();
        }
        else
        {
            Debug.LogWarning("未找到玩家连杀组件");
        }

        Invoke("DisableGameObject", 1f); // 延迟禁用对象
    }

    private void DisableGameObject()
    {
        gameObject.SetActive(false); // 禁用物体
        Invoke("destroy", 3f);
    }

    public void destroy()
    {
        Transform parent = transform.parent;
        if (parent != null)
        {
            Destroy(parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        MonsterSpawner spawner = FindObjectOfType<MonsterSpawner>();
        if (spawner != null)
        {
            spawner.OnMonsterDestroyed();
        }
        else
        {
            Debug.LogError("未找到怪物生成器！");
        }
    }

    private void SetZhanImageVisibility(bool isVisible)
    {
        if (zhanImage != null)
        {
            Color color = zhanImage.color;
            color.a = isVisible ? 1f : 0f;
            zhanImage.color = color;
        }
    }

    private void HideZhanImageAfterDelay()
    {
        SetZhanImageVisibility(false);
    }

    private void DeliverMonster()
    {
        GameObject deliverPoint = GameObject.Find(deliverPointName);
        if (deliverPoint == null)
        {
            Debug.LogWarning($"未找到名称为 {deliverPointName} 的传送目标点！");
            return;
        }

        Debug.Log($"正在传送 {gameObject.name} 到 {deliverPointName}...");
        transform.position = deliverPoint.transform.position;
        Debug.Log($"传送后怪物位置: {transform.position}");

        isInBugZone = false; // 重置状态
    }
}