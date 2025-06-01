using System.Collections;  
using System.Collections.Generic;  
using UnityEngine;  

// 注意：不需要 BigRookGames.Weapons 命名空间，除非你确实需要它  
using BigRookGames.Weapons;  

public class boomrangea : MonoBehaviour  
{  
    public Collider explosionCollider; // 指向爆炸碰撞体的引用  
    public float delay = 0.1f;  
    private ProjectileController projectileController; // 添加 ProjectileController 的引用  

    private void Start()  
    {  
        explosionCollider = GetComponent<Collider>(); // 获取碰撞体组件  
        if (explosionCollider == null)  
        {  
            Debug.LogError("Boomrange 对象上没有找到碰撞体组件！");  
        }  

        // 初始状态下禁用碰撞体  
        explosionCollider.enabled = false;  

        // 获取 ProjectileController 组件  
        projectileController = GetComponentInParent<ProjectileController>(); // 假设 boomrange 是 projectile 的子对象  
        if (projectileController == null)  
        {  
            Debug.LogError("在父对象上没有找到 ProjectileController 脚本！");  
        }  
    }  

    private void Update()  
    {  
        // 检查 ProjectileController 是否存在且已爆炸  
        if (projectileController != null && projectileController.hasExploded)  
        {  
            StartCoroutine(ActivateColliderWithDelay());  
            projectileController.hasExploded = false; // 只激活一次  
        }  
    }  

    private IEnumerator ActivateColliderWithDelay()  
    {  
        yield return new WaitForSeconds(delay);  
        explosionCollider.enabled = true; // 激活碰撞体  
        Destroy(gameObject, 5f);  
    }  

    private void OnTriggerEnter(Collider other)  
    {  /*
        // 确保你碰撞的是玩家  
        if (other.gameObject.name == "Player")  
        {  
            // 获取玩家的 PlayerHealth 脚本  
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();  

            // 检查是否成功获取了 PlayerHealth 脚本  
            if (playerHealth != null)  
            {  
                // 对玩家造成伤害  
                playerHealth.TakeDamage(10);  
            }  
            else  
            {  
                Debug.LogError("在 Player 对象上没有找到 PlayerHealth 脚本！");  
            }  
        }  */
    }  
}