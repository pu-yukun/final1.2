using UnityEngine;  
using UnityEngine.UI;  // 用于UI功能  

public class KnightHealth : MonoBehaviour  

{    
    
    public EnemyKnight EnemyKnight;
      public PlayerKillStreak PlayerKillStreak;
    private bool isDead=false;
    public float health = 2500f; // 初始生命值  
    public Animator animator; // 动画控制器  
    public GameObject healthBarUI; // 整个血量UI GameObject（包含健康条和文本）  
    public Image healthBar; // 健康值UI填充  
    public Text healthText; // 用于显示当前生命值的文本  

    private bool isHealthBarActive = false; // 用于控制血量条的激活状态  

 [Header("UI 设置")]

    public Image zhanImage; // 近战提示的 Image 组件

   private bool isDisplayZhan;  





    void Start()  
    {    EnemyKnight=GetComponent<EnemyKnight>();
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
     
        
        
        
        
          PlayerKillStreak    PlayerKillStreak=GetComponent<PlayerKillStreak>();
        healthBarUI.SetActive(false); // 初始时隐藏血量 UI  
    }  

    // 武器对应的伤害值  
    private readonly float[] weaponDamage = { 30f, 45f, 60f, 0f, 12f, 300f,1200f };  

    private void OnTriggerEnter(Collider other)  
    {  

if (other.CompareTag("flash"))  
        {  
          TakeDamage(3);
          EnemyKnight.ApplyFlashEffect();
            
        }  


   if (other.CompareTag("boom"))  
        {  
            Debug.Log("遭受爆炸攻击");  
            Invoke("handlebbom",1f);
            
        }   

 if (other.CompareTag("knife"))  
        {  
            Debug.Log("骑士近战攻击！！！！！！");  
          TakeDamage(400);
              SetZhanImageVisibility(true); // 显示图标  
            Invoke("HideZhanImageAfterDelay", 1.5f);
        } 
  if (other.CompareTag("knifetwo"))  
        {  
            Debug.Log("骑士遭受近战攻击！！！！！！");  
          TakeDamage(600);
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




    public void handlebbom(){

         Debug.Log("遭受爆炸碎片持续攻击！！");
          TakeDamage(589); // 使用当前武器的伤害  
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
            case "5": return 5; // 对应其他武器  
            case "6": return 6; // 对应其他武器  
            default: return -1; // 若无匹配，返回-1  
        }  
    }  

    public void TakeDamage(float damage)  
    {  if (isDead) return; 
        health -= damage; // 减少生命值  
        health = Mathf.Max(health, 0); // 确保生命值不低于0  
        UpdateHealthUI(); // 更新血量UI  
        Debug.Log("Health before damage: " + health);  
        Debug.Log("Damage taken: " + damage);  
        if (health <= 0)  
        {  
            Die(); // 触发死亡逻辑  
        }  
        if (health < 2000f) // 假设最大生命值为2000  
        {  
            ActivateHealthBar(); // 激活血量 UI   
        }  
    }  

    private void ActivateHealthBar()  
    {  
        if (!isHealthBarActive)  
        {  
            isHealthBarActive = true; // 设置状态为已激活  
            healthBarUI.SetActive(true); // 显示血量 UI  
        }  
    }  

    private void UpdateHealthUI()  
    {  
        if (healthBar != null)  
        {  
            healthBar.fillAmount = health / 2500f; // 假设最大生命值为500  
        }  

        if (healthText != null)  
        {  
            healthText.text = "Health: " + health; // 更新文本信息  
        }  
    }  

    public void Die()  
    {  if (isDead) return; 
        animator.SetTrigger("isdeath"); // 播放死亡动画  
        Invoke("DisableGameObject", 5f); // 2秒后禁用游戏对象  

       if (PlayerKillStreak.Instance != null)
    {
        PlayerKillStreak.Instance.RegisterKill(); isDead = true;
    }
    else
    {
        Debug.LogWarning("未找到玩家连杀组件");
    }



    }  

  private void DisableGameObject()  
    {  
        gameObject.SetActive(false); // 禁用物体  
        Invoke("destroy",3f);
    }  
    public void destroy(){
       Transform parent = transform.parent; 
 if (parent != null)  
        {  
            // 销毁父物体  
            Destroy(parent.gameObject);  
        }  
        else  
        {  
            Debug.LogWarning("父物体不存在，无法销毁！");  
            // 如果父物体不存在，销毁自身  
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
    void Update()  
    {  
        // 将血量 UI 跟随敌人  
        if (isHealthBarActive)  
        {  
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);  
            healthBarUI.transform.position = screenPosition + new Vector3(0, 50, 0); // 调整偏移量  
        }  
    }   
}