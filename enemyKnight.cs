using UnityEngine;  
using UnityEngine.AI;  
using System.Collections;


public class EnemyKnight : MonoBehaviour  
{  

[Header("音效")]
[Tooltip("行走音效")]public AudioClip attackSound;
[Tooltip("跳跃攻击音效")]public AudioClip jumpSound;
[Tooltip("combo音效")]public AudioClip comboSound;

private AudioSource audioSource;
// public attackrangecombo attackRangeCombo; // 引用攻击范围组合 

    public GameObject attack; // 普通攻击碰撞体  
    public GameObject combo; // 连击攻击碰撞体  
    public GameObject jump; // 跳跃攻击碰撞体

    private Transform player; // 玩家物体  
    public NavMeshAgent agent; // 导航代理  
    public Animator animator; // 动画控制器  
    private BoxCollider boxCollider; // 碰撞体  
public int comboCount; 
    // 使用向量来设置各个攻击距离和冷却时间  
    public float jumpAttackCooldown = 6f; // 跳跃攻击的冷却时间  
    public float lastJumpAttackTime = 0f; // 上次跳跃攻击的时间  
  public float lastComboTime = 0f;
    public float walkSpeed = 2f; // 步行速度  
    public float runSpeed = 5f; // 奔跑速度  
    public float flashSpeed=0.5f;
    public float originSpeed=5f;
    public float attackDistance = 1.5f; // 普通攻击距离  
    public float comboDistance = 5.5f; // 连击距离  
    public float jumpDistance = 5f; // 跳跃攻击距离  
    public float comboCooldown = 10f; // 连击冷却时间  
    public float maintainDistance = 0.5f; // 保持的距离  
    public float switchToRunDistance = 20f; // 切换到奔跑的距离  
    public int maxHits = 3; // 最大受击次数  
    public float defendDuration = 5f; // 防御持续时间  
public int hitspeed=0;
    private int hitCount = 0; // 受击次数  
    private bool isAttacking = false; // 标记是否正在攻击  
    private bool isJumping = false; // 标记是否正在跳跃攻击  
    private bool isCombo = false; // 标记是否正在进行连击  
   // private int playerHitCountp=0; // 记录击中玩家的次数  
   void awake(){
initialjump();
initialcombo();
initialattack();


   }
public void initialjump(){
jump.SetActive(false);
Debug.Log("初始化");
}

public void initialcombo(){
combo.SetActive(false);
}

public void initialattack(){
attack.SetActive(false);
Debug.Log("初始化");
}






    void Start()  
    {         

   GameObject jump = GameObject.Find("jumpattackrange");  
    GameObject combo = GameObject.Find("comboattackrange"); 
    GameObject attack = GameObject.Find("attackrange"); 
    if (jump == null) Debug.LogError("Jump attack range not found!");  
    
    if (combo == null) Debug.LogError("Combo attack range not found!");  
    if(combo!=null)Debug.Log("find!!");
    if (attack == null) Debug.LogError("Attack range not found!");  

    player = GameObject.Find("Player")?.transform;  
    if (player == null) Debug.LogError("Player not found!");  



audioSource=GetComponent<AudioSource>();



       // player = GameObject.Find("Player").transform;  
        DontDestroyOnLoad(gameObject);  
        agent.speed = walkSpeed;  
        animator.SetBool("isWalking", true);  

        boxCollider = gameObject.GetComponent<BoxCollider>();  
        if (boxCollider == null)  
        {  
            boxCollider = gameObject.AddComponent<BoxCollider>();  
        }  
        boxCollider.isTrigger = true; // 设置为触发器  







    }  


    private void Update()  
    {  
        if (player == null)  
        {  
            Debug.LogWarning("Player not found!");  
            return;  
        }  else{
            Debug.Log("player");
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);  

        if (hitCount < maxHits)  
        {  
            TryJumpAttack(distanceToPlayer); // 尝试进行跳跃攻击  
            TryCombo(distanceToPlayer); // 尝试进行连击攻击  

            if (!isJumping && !isCombo)  
            {  
                if (distanceToPlayer <= attackDistance)  
                {  
                    if (!isAttacking)  
                    {  
                        Attack();  
                    }  
                }  
                else if (distanceToPlayer <= switchToRunDistance)  
                {  
                    if (distanceToPlayer > (attackDistance + maintainDistance))  
                    {  
                        RunTowardsPlayer();  
                    }  
                    else  
                    {  
                        MaintainDistance();  
                    }  
                }  
                else  
                {  
                    WalkTowardsPlayer();  
                }  
            }  
        }  
        else  
        {  
            Defend();  
        }  

        if (!isAttacking && !isJumping && !isCombo)  
        {  
            FacePlayer();  
        }  
    }  

private void Attack()  
{
     Invoke("attacksound", 0.7f);
    
    isAttacking = true;  
isJumping=false;
isCombo=false;
    agent.isStopped = true;  
    animator.SetBool("isAttacking", true);  
    animator.SetBool("isWalking", false);  
    animator.SetBool("isRunning", false);  
    // 这里可以用一个延迟调用结束攻击的状态  

 Invoke("enableattack", 0.75f);

    Invoke("ResetAttackState", 1.85f); // 假设1秒之后攻击结束  
}  
private void attacksound(){

      audioSource.PlayOneShot(attackSound);
}



   private void enableattack()  
    {  
 attack.SetActive(true);
        Invoke("disableattack", 0.35f); // 0.1秒后禁用  
    }  

    private void disableattack()  
    {  
         attack.SetActive(false);
        
    } 


private void ResetAttackState()  
{  
    isAttacking = false; // 可以继续追击  
    agent.isStopped = false; // 允许移动  
    animator.SetBool("isAttacking", false);   
}
    private void FacePlayer()  
    {  
        Vector3 direction = (player.position - transform.position).normalized;  
        Quaternion lookRotation = Quaternion.LookRotation(direction);  
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);  
    }  
public void ApplyFlashEffect(){

agent.speed=flashSpeed;
runSpeed=flashSpeed;
walkSpeed=flashSpeed;
    Invoke("EndFlash",3.5f);

}
public void EndFlash(){

agent.speed=originSpeed;
runSpeed=originSpeed;
walkSpeed=4f;
   // Invoke("EndFlash",3.5f);

}



    private void WalkTowardsPlayer()  
    {  
        if (!isJumping && !isCombo)  
        {  
            agent.speed = walkSpeed;  
            agent.SetDestination(player.position);  
            animator.SetBool("isWalking", true);  
            animator.SetBool("isRunning", false);  
            animator.SetBool("isAttacking", false);  
            isAttacking = false;  
        }  
    }  



    private void RunTowardsPlayer()  
    {  
        if (!isJumping && !isCombo)  
        {  
            agent.speed = runSpeed;  
            agent.SetDestination(player.position);  
            animator.SetBool("isRunning", true);  
            animator.SetBool("isWalking", false);  
            animator.SetBool("isAttacking", false);  
            isAttacking = false;  
        }  
    }  

    private void MaintainDistance()  
    {  
        if (!isJumping && !isCombo)  
        {  
            if (Vector3.Distance(transform.position, player.position) < maintainDistance)  
            {  
                agent.isStopped = true;  
                animator.SetBool("isWalking", false);  
                animator.SetBool("isRunning", false);  
                animator.SetBool("isAttacking", false);  
                isAttacking = false;  
            }  
            else  
            {  
                agent.isStopped = false;  
            }  
        }  
    }  

    private void Defend()  
    {  
    isAttacking=false;
    isJumping=false;
    isCombo=false;
        animator.SetBool("isDefending", true);  
        agent.isStopped = true;  
        Invoke("ResetDefendState", defendDuration);  
    }  

    private void ResetDefendState()  
    {  
        hitCount = 1;  
        animator.SetBool("isDefending", false);  
        agent.isStopped = false;  
      
    }  

    private void OnTriggerEnter(Collider other)  
    {  
        if (other.CompareTag("Bullet"))  
        {  

 Debug.Log("Bullet hit the knight!"); // 添加调试信息

            TakeHit();  
            Destroy(other.gameObject);  
        }  

  if (other.CompareTag("Player"))  
        {  
            // 如果攻击碰撞体与玩家碰撞，记录击中状态   
            //playerHitCount++;  
            // 在此您可以添加逻辑减少玩家的生命值  
         //   Debug.Log("Player hit! Total hits: " + playerHitCount);  
            // 这里可调用玩家的减少生命值方法，例如:  
            // player.GetComponent<PlayerHealth>().TakeDamage(damageAmount);  
        }  



    }  

    public void TakeHit()  
    {  
        hitCount++;  
   hitspeed++;
   if(hitspeed>=10){
    runSpeed=runSpeed-1;
  
    Invoke("hitspeedtest",1f);
   }
        if (hitCount >= maxHits)  
        {  
            Defend();  
        }  
    }  
private void hitspeedtest(){

    hitspeed=hitspeed-3;
      runSpeed=runSpeed+1;
}
    private void TryJumpAttack(float distanceToPlayer)  
    {  
        if (distanceToPlayer <= jumpDistance && !isJumping && Time.time >= lastJumpAttackTime + jumpAttackCooldown)  
        {  
            JumpAttack();  
            lastJumpAttackTime = Time.time;  
        }  
    }  

    private void JumpAttack()  
    {     Invoke("jumpsound", 1.2f);
        isJumping = true;  
        isAttacking=false;
        isCombo=false;
        agent.isStopped = true;  
        animator.SetBool("isJumping", true);  
        Invoke("EndJumpAttack", 1.8f);  
   Invoke("enablejump", 1.15f); // 假设0.5秒后激活跳跃攻击碰撞体  
            Invoke("ResetJumpState", 2.3f); // 假设1秒后恢复状态 



    }  

private void jumpsound(){
audioSource.PlayOneShot(jumpSound);

}
   private void enablejump()  
    {  
     jump.SetActive(true);
        Invoke("disablejump", 0.35f); // 0.1秒后禁用  
    }  

    private void disablejump()  
    {  

         jump.SetActive(false);
       
    }  






 private void EndJumpAttack()  
{  
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);  
    isJumping = false; // 重置跳跃状态  
    animator.SetBool("isJumping", false);   

    if (distanceToPlayer < attackDistance)  
    {  
        if (!isCombo && Time.time >= lastComboTime + comboCooldown)  
        {  
            TryCombo(distanceToPlayer); // 尝试连击  
        }  
        else  
        {  
            Attack(); // 进行攻击  
        }  
    }  
    else  
    {  
        RunTowardsPlayer(); // 跑向玩家  
    }  
    
    // 确保允许移动  
    agent.isStopped = false;   
}

    private void TryCombo(float distanceToPlayer)  
    {  
        if (distanceToPlayer <= comboDistance && !isCombo && Time.time >= lastComboTime + comboCooldown)  
        {  
            ComboAttack();  
        }  
    }  

    public void ComboAttack()  
    {     Invoke("combosound", 0.5f);
        isCombo = true;  
        isJumping=false;
        isAttacking=false;
        lastComboTime = Time.time; 
        comboCount=0; 
        animator.SetBool("isCombo", true);  
        agent.isStopped = true;  
       

 //StartCoroutine(PerformComboAttack()); 
PerformComboAttack()  ;

Invoke("EndCombo", 1.9f); 

    }  
private void combosound(){

     audioSource.PlayOneShot(comboSound);
}
/*
private IEnumerator PerformComboAttack()  
{  

     yield return new WaitForSeconds(0.2f); // 激活时间范围 
    EnableComboAttackCollider(new Vector3(0.1f, 0, 0)); // 改变碰撞体中心  
    Debug.Log("Combo Attack 1!");  
    yield return new WaitForSeconds(0.2f); // 激活时间范围  
     CheckPlayerCollision();
    DisableComboAttackCollider(); // 禁用碰撞体  

    // 等待到第二次攻击  
    yield return new WaitForSeconds(0.15f); // 等待到下一个攻击  
    EnableComboAttackCollider(new Vector3(0.1f, 0, 0)); // 改变碰撞体中心  
    Debug.Log("Combo Attack 2!");  
    yield return new WaitForSeconds(0.3f); // 激活时间范围  
     CheckPlayerCollision();
    DisableComboAttackCollider(); // 禁用碰撞体  

    // 等待到第三次攻击  
    yield return new WaitForSeconds(0.2f); // 等待到下一个攻击  
    EnableComboAttackCollider(new Vector3(0.1f, 0, 0)); // 改变碰撞体中心  
    Debug.Log("Combo Attack 3!");  
    yield return new WaitForSeconds(0.35f); // 激活时间范围  
     CheckPlayerCollision();
DisableComboAttackCollider(); // 禁用碰撞体  

    ResetComboState();  
}
private void EnableComboAttackCollider(Vector3 offset)  
{  
    comboAttackCollider.enabled = true;  
    Debug.Log("Combo Attack Collider Enabled"); // 调试信息  
    comboAttackCollider.center = offset; // 设置新的中心位置  
}  
private void DisableComboAttackCollider()  
{  
    comboAttackCollider.enabled = false;  
    Debug.Log("Combo Attack Collider Disabled"); // 调试信息  
}*/

 private void ResetComboState()  
    {  
        isCombo = false;  
        agent.isStopped = false;  
        animator.SetBool("isCombo", false);  
    }  
/*
public void CheckPlayerCollision()  
{  
    Vector3 center = comboAttackCollider.transform.position + comboAttackCollider.center;  
    float radius = comboAttackCollider.bounds.extents.magnitude; // 使用半径  

    // 使用 Sphere 方法来验证碰撞  
    Collider[] hitColliders = Physics.OverlapSphere(center, radius);  

    foreach (var collider in hitColliders)  
    {  
        Debug.Log($"Detected Collider: {collider.name}, Tag: {collider.tag}");  
        if (collider.CompareTag("Player"))  
        {  
            Debug.Log($"Player hit detected: {collider.name}");  
            // 调用 AttackRangeCombo 的 OnTriggerEnter 方法，处理玩家受伤逻辑  
            attackRangeCombo.OnTriggerEnter(collider);  
        }  
    }  
}*/



private void PerformComboAttack()  
{  
    // 第一次攻击  
   // EnableComboAttackCollider(new Vector3(0.1f, 0, 0)); // 改变碰撞体中心  
    Debug.Log("Combo Attack 1!");  
     Invoke("enablefirstcombo", 0.3f);
    //Invoke("CheckAndDisableCollider", 0.5f); // 0.5秒后检查碰撞并禁用   

    // 等待到第二次攻击  
    
}  
private void enablefirstcombo(){
combo.SetActive(true);
 Invoke("disablefirstcombo", 0.2f);

}
private void disablefirstcombo(){
combo.SetActive(false);
 Invoke("enablesecondcombo", 0.1f);
}

private void enablesecondcombo(){
combo.SetActive(true);
 Invoke("disablesecondcombo", 0.2f);

}
private void disablesecondcombo(){
combo.SetActive(false);
 Invoke("enablethirdcombo", 0.25f);}

private void enablethirdcombo(){
combo.SetActive(true);
 Invoke("disablethirdcombo", 0.2f);

}
private void disablethirdcombo(){
combo.SetActive(false);
 }



/*
private void SecondComboAttack()  
{  
    EnableComboAttackCollider(new Vector3(0.1f, 0, 0)); // 改变碰撞体中心  
    Debug.Log("Combo Attack 2!");  
    Invoke("CheckAndDisableCollider", 0.5f); // 0.5秒后检查碰撞并禁用   

    // 等待到第三次攻击  
    Invoke("ThirdComboAttack", 0.65f); // 在 0.65 秒后触发第三次攻击（0.5s 加上 0.15s 等待）  
}  

private void ThirdComboAttack()  
{  
    EnableComboAttackCollider(new Vector3(0.1f, 0, 0)); // 改变碰撞体中心  
    Debug.Log("Combo Attack 3!");  
    Invoke("CheckAndDisableCollider", 0.5f); // 0.5秒后检查碰撞并禁用   

    ResetComboState();  
}  

private void CheckAndDisableCollider()  
{  
    CheckPlayerCollision();  
    DisableComboAttackCollider(); // 禁用碰撞体  
}  

private void EnableComboAttackCollider(Vector3 offset)  
{  
    comboAttackCollider.enabled = true;  
    Debug.Log("Combo Attack Collider Enabled"); // 调试信息  
    comboAttackCollider.center = offset; // 设置新的中心位置  
}  

private void DisableComboAttackCollider()  
{  
    comboAttackCollider.enabled = false;  
    Debug.Log("Combo Attack Collider Disabled"); // 调试信息  
}  

private void ResetComboState()  
{  
    isCombo = false;  
    agent.isStopped = false;  
    animator.SetBool("isCombo", false);  
}  
/*
public void CheckPlayerCollision()  
{  
    Vector3 center = comboAttackCollider.transform.position + comboAttackCollider.center;  
    float radius = comboAttackCollider.bounds.extents.magnitude; // 使用半径  

    // 使用 Sphere 方法来验证碰撞  
    Collider[] hitColliders = Physics.OverlapSphere(center, radius);  

    foreach (var collider in hitColliders)  
    {  
        Debug.Log($"Detected Collider: {collider.name}, Tag: {collider.tag}");  
        if (collider.CompareTag("Player"))  
        {  
            Debug.Log($"Player hit detected: {collider.name}");  
            // 调用 AttackRangeCombo 的 OnTriggerEnter 方法，处理玩家受伤逻辑  
            attackRangeCombo.OnTriggerEnter(collider);  
        }  
    }  
}
*/



    private void EndCombo()  
    {  agent.isStopped = false;
    
        isCombo = false;  
        animator.SetBool("isCombo", false);  
    }  
}