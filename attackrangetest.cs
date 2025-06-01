using UnityEngine;  

public class AttackRangeTrigger : MonoBehaviour  
{  
    public int damageAmount = 10; // 攻击造成的伤害  
    private EnemyKnight enemyKnight; // 引用敌方骑士  

    private void Start()  
    {  
        Debug.Log("攻击范围触发器已启动!"); // 确认脚本被加载  
        // 获取敌方骑士的引用  
        enemyKnight = GetComponentInParent<EnemyKnight>();  
        
        if (enemyKnight == null)  
        {  
            Debug.LogError("未能找到敌方骑士代理，请检查层级设置!");  
        }  
    }  

    private void OnTriggerEnter(Collider other)  
    {  
        Debug.Log("触发器被激活，碰撞体: " + other.gameObject.name); // 打印激活触发器的物体  

        if (other.CompareTag("Player"))  
        {  
            Debug.Log("玩家被攻击范围击中!");  
            // 这里调用玩家的受伤方法  
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();  
            if (playerHealth != null)   
            {  
                playerHealth.TakeDamage(damageAmount);  
            }  
            else  
            {  
                Debug.LogError("未能找到 PlayerHealth 组件，确保玩家上有该组件!");  
            }  
        }  
        else  
        {  
            Debug.Log("未击中玩家: " + other.gameObject.name);  
        }  
    }  

    private void OnTriggerExit(Collider other)  
    {  
        Debug.Log("触发器被退出，物体: " + other.gameObject.name);  
    }  
}