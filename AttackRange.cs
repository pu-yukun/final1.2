using UnityEngine;  

public class AttackRange : MonoBehaviour  
{  
    public int damage = 1; // 攻击伤害值 
public int totalDamage;
        public int smallHurtBonus = 4;   // smallhurt 附加伤害
    public int middleHurtBonus = 5; // middlehurt 附加伤害
    public int bigHurtBonus = 7;    // bighurt 附加伤害

    private GameObject player; // 玩家对象  
    private PlayerHealth playerHealth; // 引用 PlayerHealth 脚本   

  public GameObject attackComboJumpRange;
    public GameObject jumpRange;
    public GameObject angryAttack;
    public GameObject angryShout;
    public GameObject attackCombo;
    public GameObject attackOneRange;
    public GameObject attackTwoRange;

    // 将所有攻击范围对象存储在一个数组中，方便批量操作
    private GameObject[] attackRanges;

    private void Start()
    {
        // 初始化攻击范围对象数组
        attackRanges = new GameObject[]
        {
            attackComboJumpRange,
            jumpRange,
            angryAttack,
            angryShout,
            attackCombo,
            attackOneRange,
            attackTwoRange
        };



 ActivateAllRanges();
    CheckAttackRangeObjects();







        // 查找玩家对象  
        player = GameObject.Find("Player");  
        if (player == null)  
        {  
            Debug.LogError("玩家未找到！请确保玩家对象存在并命名为 'Player'。");  
            return;  
        }  

        // 获取 PlayerHealth 组件  
        playerHealth = player.GetComponent<PlayerHealth>();  
        if (playerHealth == null)  
        {  
            Debug.LogError("PlayerHealth 组件未找到！请确保玩家对象上添加了 PlayerHealth。");  
        }   DeactivateAllRanges();

    }  

    
 public void ActivateAttackRange(GameObject attackRangeObject)
    {
        if (attackRangeObject != null)
        {
            attackRangeObject.SetActive(true);
            Debug.Log(attackRangeObject.name + " 攻击范围已激活。");
        }
        else
        {
            Debug.LogWarning("攻击范围对象未赋值！");
        }
    }

    // 禁用指定攻击范围对象
    public void DeactivateAttackRange(GameObject attackRangeObject)
    {
        if (attackRangeObject != null)
        {
            attackRangeObject.SetActive(false);
            Debug.Log(attackRangeObject.name + " 攻击范围已禁用。");
        }
        else
        {
            Debug.LogWarning("攻击范围对象未赋值！");
        }
    }















public void CheckAttackRangeObjects()
{
    string[] attackRangeNames = { "attackcombojumprange", "jumprange", "angryattack", "angryshout", "attackcombo", "attackonerange", "attacktworange" };
    foreach (string name in attackRangeNames)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null)
        {
            Debug.Log("找到所有！！！！！！！攻击范围对象: " + name);
        }
        else
        {
            Debug.LogWarning("未找到攻击范围对象: ！！！！！！" + name);
        }
    }

}

private void ActivateAllRanges()
{
    string[] attackRangeNames = { "attackcombojumprange", "jumprange", "angryattack", "angryshout", "attackcombo", "attackonerange", "attacktworange" };
    foreach (string name in attackRangeNames)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null)
        {
            obj.SetActive(true); // 激活对象
            Debug.Log("激活攻击范围对象: " + name);
        }
        else
        {
            Debug.LogWarning("未找到攻击范围对象: " + name);
        }
    }
}
private void DeactivateAllRanges()
{
    string[] attackRangeNames = { "attackcombojumprange", "jumprange", "angryattack", "angryshout", "attackcombo", "attackonerange", "attacktworange" };
    foreach (string name in attackRangeNames)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null)
        {
            obj.SetActive(false); // 禁用对象
            Debug.Log("禁用攻击范围对象: " + name);
        }
        else
        {
            Debug.LogWarning("未找到攻击范围对象: " + name);
        }
    }
}
    // 激活 attackcombojumprange 空物体  
   public void ActivateAttackComboJumpRange()
    {
        Debug.Log("ActivateAttackComboJumpRange 被调用");
        ActivateAttackRange(attackComboJumpRange);
    }

    // 示例：禁用 attackcombojumprange
    public void DeactivateAttackComboJumpRange()
    {
        Debug.Log("DeactivateAttackComboJumpRange 被调用");
        DeactivateAttackRange(attackComboJumpRange);
    }

    // 示例：激活 jumprange
    public void ActivateJumpRange()
    {
        Debug.Log("ActivateJumpRange 被调用");
        ActivateAttackRange(jumpRange);
    }

    // 示例：禁用 jumprange
    public void DeactivateJumpRange()
    {
        Debug.Log("DeactivateJumpRange 被调用");
        DeactivateAttackRange(jumpRange);
    }

    // 示例：激活 angryattack
    public void ActivateAngryAttack()
    {
        Debug.Log("ActivateAngryAttack 被调用");
        ActivateAttackRange(angryAttack);
    }

    // 示例：禁用 angryattack
    public void DeactivateAngryAttack()
    {
        Debug.Log("DeactivateAngryAttack 被调用");
        DeactivateAttackRange(angryAttack);
    }

    // 示例：激活 angryshout
    public void ActivateAngryShout()
    {
        Debug.Log("ActivateAngryShout 被调用");
        ActivateAttackRange(angryShout);
    }

    // 示例：禁用 angryshout
    public void DeactivateAngryShout()
    {
        Debug.Log("DeactivateAngryShout 被调用");
        DeactivateAttackRange(angryShout);
    }

    // 示例：激活 attackcombo
    public void ActivateAttackCombo()
    {
        Debug.Log("ActivateAttackCombo 被调用");
        ActivateAttackRange(attackCombo);
    }

    // 示例：禁用 attackcombo
    public void DeactivateAttackCombo()
    {
        Debug.Log("DeactivateAttackCombo 被调用");
        DeactivateAttackRange(attackCombo);
    }

    // 示例：激活 attackonerange
    public void ActivateAttackOneRange()
    {
        Debug.Log("ActivateAttackOneRange 被调用");
        ActivateAttackRange(attackOneRange);
    }

    // 示例：禁用 attackonerange
    public void DeactivateAttackOneRange()
    {
        Debug.Log("DeactivateAttackOneRange 被调用");
        DeactivateAttackRange(attackOneRange);
    }

    // 示例：激活 attacktworange
    public void ActivateAttackTwoRange()
    {
        Debug.Log("ActivateAttackTwoRange 被调用");
        ActivateAttackRange(attackTwoRange);
    }

    // 示例：禁用 attacktworange
    public void DeactivateAttackTwoRange()
    {
        Debug.Log("DeactivateAttackTwoRange 被调用");
        DeactivateAttackRange(attackTwoRange);
    }

    // 检测与玩家的碰撞
      private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("攻击命中玩家！攻击范围: " + gameObject.name); // 打印攻击范围对象的名字

            // 计算总伤害：基础伤害 + 附加伤害
    
            switch (gameObject.tag)
            {
                case "smallhurt":
                    totalDamage += smallHurtBonus;
                    break;
                case "middlehurt":
                    totalDamage += middleHurtBonus;
                    break;
                case "bighurt":
                    totalDamage += bigHurtBonus;

  PlayerController playerController = other.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.ReduceSpeed(2.5f); // 降低速度 1，5 秒后恢复
                }
                else
                {
                    Debug.LogWarning("PlayerController 组件未找到！");
                }
                break;


                    break;
                default:
                    Debug.LogWarning("未知攻击类型: " + gameObject.tag);
                    break;
            }

            Debug.Log("总伤害: " + totalDamage); // 打印总伤害值

            // 调用 PlayerHealth 的 TakeDamage 方法
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(totalDamage);
            }
            else
            {
                Debug.LogWarning("PlayerHealth 组件未找到！");
            }
        }
    }

    
}