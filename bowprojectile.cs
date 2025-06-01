using UnityEngine;

public class BowProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 20f; // 箭的速度，再预制体里面修改才是正常的值
    public int damage = 5; // 箭的伤害值

    private Rigidbody rb; // 刚体组件
    private Vector3 direction; // 飞行方向

    void Start()
    {
        // 获取刚体组件
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("箭矢预制体缺少 Rigidbody 组件！");
            return;
        }

        // 初始化方向（如果未通过 EnemyBower 设置）
        if (direction == Vector3.zero)
        {
            InitializeDirection(transform.forward);
        }

        // 应用初始速度
      //  rb.velocity = direction * speed;
      rb.velocity = direction * speed;
    }

    // 由 EnemyBower 调用以传递方向
    public void InitializeDirection(Vector3 dir)
    {
        direction = dir.normalized;

        // 强制应用初始旋转 (90, 0, 180)
        transform.rotation = Quaternion.Euler(90, 0, 180) * Quaternion.LookRotation(dir);

  // 确保刚体存在后设置速度
        if (rb != null)
        {
            rb.velocity = direction * 50;
        }
        else
        {
            Debug.LogError("未找到 Rigidbody 组件！");
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        // 如果碰撞到玩家
        if (other.CompareTag("Player"))
        {
            Debug.Log("碰到玩家了");

            // 通过 gamemanager 获取玩家引用
            if (gamemanager.Instance != null && gamemanager.Instance.player != null)
            {
                PlayerHealth playerHealth = gamemanager.Instance.player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    Debug.Log("远程攻击掉血");
                    playerHealth.TakeDamage(damage); // 调用玩家的受伤函数
                }
                else
                {
                    Debug.LogError("玩家对象未找到 PlayerHealth 组件！");
                }
            }
            else
            {
                Debug.LogError("未找到玩家引用！");
            }
        }

        // 碰撞后销毁箭
   //     Destroy(gameObject);
    }
}