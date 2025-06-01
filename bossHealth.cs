using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class bossHealth : MonoBehaviour
{
    public enemyboss enemyboss;
    // Start is called before the first frame update
public float maxHealth=2000f;
    public float health = 2000f; // 初始生命值  
    public Animator animator; // 动画控制器  
    public GameObject healthBarUI; // 整个血量UI GameObject（包含健康条和文本）  
    public Image healthBarFill; // 健康值UI填充  
    public Text healthText; // 用于显示当前生命值的文本  
public GameObject text;
    private bool isHealthBarActive = false; // 用于控制血量条的激活状态  
 public bool isBossDead = false; // 新增：标记 BOSS 是否死亡用于mission3task4
 [Header("UI 设置")]

    public Image zhanImage; // 近战提示的 Image 组件

   private bool isDisplayZhan;  

   public event Action OnTaskfourend;

    void Start()  
    {  
        enemyboss=GetComponent<enemyboss>();
        healthBarUI.SetActive(false); // 初始时隐藏血量 UI  

if (zhanImage == null)  
        {  
            Debug.LogError("zhanImage 未在 Inspector 中设置！");  
        }  
        else  
        {  
            SetZhanImageVisibility(false); // 初始隐藏  
            // 你可以设置 zhanImage 的位置，例如  
            zhanImage.transform.position = new Vector3(520, 60, 0); // 固定位置（根据需要调整）  
        }  


    }  

    // 武器对应的伤害值  
    private readonly float[] weaponDamage = { 25f, 45f, 60f, 0f, 15f, 600f,1500f };  

    private void OnTriggerEnter(Collider other)  
    {  
if (other.CompareTag("flash"))  
        {  
          TakeDamage(3);
          enemyboss.ApplyFlashEffect();
            
        }  

   if (other.CompareTag("boom"))  
        {  
            Debug.Log("遭受爆炸攻击");  
            Invoke("handlebbom",1f);
            
        }   
 if (other.CompareTag("knife"))  
        {  
            Debug.Log("近战攻击！！！！！！");  
          TakeDamage(400);
              SetZhanImageVisibility(true); // 显示图标  
            Invoke("HideZhanImageAfterDelay", 1.5f);
        } 
  if (other.CompareTag("knifetwo"))  
        {  
            Debug.Log("近战攻击！！！！！！");  
          TakeDamage(520);
            SetZhanImageVisibility(true); // 显示图标  
            Invoke("HideZhanImageAfterDelay", 1.5f);
        } 

        Debug.Log("受到攻击了！");  

        
         if (other.CompareTag("Bullet")||other.CompareTag("RPG"))  
        {  
            inventory inv = FindObjectOfType<inventory>();  
        if (inv != null)  
        {  
            int damage = inv.GetCurrentWeaponDamage(); // 获取当前武器的伤害  
            TakeDamage(damage); // 使用当前武器的伤害  
            Debug.Log("受伤: " + damage);  
        }  
        } 

         

        else  
        {  
            // 处理其他标签情况  
            Debug.Log("标签错误: " + other.tag); // 输出标签，查找问题  
        }  
    }  
    public void handlebbom(){

         Debug.Log("遭受爆炸碎片持续攻击！！");
          TakeDamage(1199); // 使用当前武器的伤害  
    }
    private int GetWeaponIndex(string weaponName)  
    {  
        // 根据武器名称返回对应的伤害索引  
        switch (weaponName)  
        {  
            case "0": return 0; // 对应 AK  
            case "1": return 1; // 对应 M4  
            case "2": return 2; // 对应其他武器  
            case "4": return 4; // 对应其他武器  
            case "5": return 5;
            case "6": return 6; // 对应其他武器  
            default: return -1; // 若无匹配，返回-1  
        }  
    }  

    public void TakeDamage(float damage)  
    {  
        health -= damage; // 减少生命值  
        health = Mathf.Max(health, 0); // 确保生命值不低于0  
        UpdateHealthUI(); // 更新血量UI  
        Debug.Log("Health before damage: " + health);  
        Debug.Log("Damage taken: " + damage);  
        if (health <= 0)  
        {  
            Die(); // 触发死亡逻辑  
        }  
        if (health < 0.9f * 20000f) // 假设最大生命值为500  
        {  
            ActivateHealthBar(); // 激活血量 UI   
        }  
    }  
        private void ActivateHealthBar()
    {
        if (healthBarUI != null)
        {
            isHealthBarActive = true;
            healthBarUI.SetActive(true); // 显示血条
            text.SetActive(true);
        }
    }
   private void UpdateHealthUI()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = health / maxHealth; // 更新填充
        }

        if (healthText != null)
        {
            healthText.text = "Boss Health: " + health.ToString("F0"); // 更新文本
        }
    }


 private void SetZhanImageVisibility(bool isVisible)  
    {  
        Color color = zhanImage.color;  
        color.a = isVisible ? 1f : 0f; // 设为全透明或不透明  
        zhanImage.color = color; // 应用透明度变化  
        isDisplayZhan = isVisible; // 更新显示状态  
    }  


  private void HideZhanImageAfterDelay()  
    {  
        SetZhanImageVisibility(false); // 隐藏图标  
    }  


public void Die()
    {   isBossDead = true;

        animator.SetTrigger("isdeath"); // 播放死亡动画

        // 隐藏血条
        if (healthBarUI != null)
        {
            healthBarUI.SetActive(false);
        }
OnTaskfourend?.Invoke();
        Invoke("DisableGameObject", 4f); // 5秒后禁用 Boss 对象
    }

    // 禁用 Boss 对象

    /*
    public void DisableGameObject()
    {
        gameObject.SetActive(false); // 禁用 Boss
        Debug.Log("boss销毁");
        Destroy(gameObject);
    }
*/

// 禁用 Boss 对象及其父物体  
public void DisableGameObject()  
{  
    // 首先禁用 Boss 对象  
    gameObject.SetActive(false); // 禁用 Boss  
    Debug.Log("boss禁用");  

    // 销毁父物体和Boss对象  
    if (transform.parent != null)  
    {  
        Debug.Log("销毁父物体");  
        Destroy(transform.parent.gameObject);  
    }  
    else  
    {  
        Debug.Log("没有父物体，直接销毁Boss对象");  
        Destroy(gameObject);  
    }  
}  


 public void ResetBossHealth()
    {
        health = maxHealth;
        UpdateHealthUI();
        gameObject.SetActive(true);
        Debug.Log("BOSS已重置，血量恢复满值");
        
        // 重置动画状态
        animator.Rebind();
        animator.Update(0f);
    }


    void Update()
    {
        // 如果血条激活，将其固定在屏幕底部
        if (isHealthBarActive && healthBarUI != null)
        {
            // 将血条固定在屏幕底部
            RectTransform rectTransform = healthBarUI.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(0.5f, 0); // 水平居中，底部对齐
                rectTransform.anchorMax = new Vector2(0.5f, 0);
                rectTransform.pivot = new Vector2(0.5f, 0);
                rectTransform.anchoredPosition = new Vector2(0, 50f); // 调整 Y 轴偏移
            }
        }
    }
}
