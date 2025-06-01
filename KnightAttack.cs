using UnityEngine;  

public class KnightAttack : MonoBehaviour  
{  
    public Transform bulletPrefab;  // 子弹预制体  
    public Transform firePoint;      // 发射点  
    public float attackRate = 1f;    // 攻击频率  
    private float nextAttackTime = 0f;  

    void Update()  
    {  
        if (Time.time >= nextAttackTime && Input.GetMouseButtonDown(0)) // 左键开火  
        {  
            Attack(); // 调用攻击  
            nextAttackTime = Time.time + 1f / attackRate;  
        }  
    }  

    private void Attack()  
    {  
        // 查找激活的武器  
        for (int i = 0; i < 5; i++)  
        {  
            GameObject weapon = GameObject.Find("Weapon" + i); // 假设武器命名为 Weapon0, Weapon1, ...  
            if (weapon != null && weapon.activeSelf) // 检查武器是否激活  
            {  
                // 实例化子弹  
                Transform bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);  
                bullet.SetParent(weapon.transform); // 设置子弹的父物体为当前武器，以便后续检查  
                bullet.name = weapon.name; // 将子弹名称设置为武器类型名称  
                bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 10f; // 设置子弹速度（根据需要调整）  
                break; // 找到一个激活的武器后退出循环  
            }  
        }  
    }  
}