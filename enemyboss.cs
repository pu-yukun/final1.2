using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyboss : MonoBehaviour
{
    public bool isallowattack = true;

    private bossHealth bossHealth;
    [Header("音效")]
    [Tooltip("行走音效")] public AudioClip attackSound;
    [Tooltip("跳跃攻击音效")] public AudioClip jumpSound;
    [Tooltip("combo音效")] public AudioClip comboSound;
    private bool hasExecutedAttack = false;
    private AudioSource audioSource;

    public GameObject attack;
    public GameObject combo;
    public GameObject jump;

    private Transform player;
    public UnityEngine.AI.NavMeshAgent agent;
    public Animator animator;
    private BoxCollider boxCollider;
    public int comboCount;

    public float jumpAttackCooldown = 6f;
    public float lastJumpAttackTime = 0f;

    public float jumpComboCooldown = 8f;
    public float lastJumpComboTime = 0f;
    public float lastComboTime = 0f;
    public float walkSpeed = 6f;
    public float rushSpeed = 12f;
    public float flashSpeed=0.4f;
    public float originSpeed=6f;
    public float angryattackcooldown = 9f;
    public float rushCooldown = 12f;
    private float lastRushTime = 0f;
    private float lastangryattacktime = 0f;
    public float attackDistance = 4f;
    public float comboDistance = 6f;
    public float jumpDistance = 6f;
    public float angryattackdistance = 20f;
    public float switchToRushDistance = 16f;

    public float comboCooldown = 10f;
    public float maintainDistance = 1f;
public bool lianxie=false;
    public int maxHits = 3;
    public int hitspeed = 0;
    private int hitCount = 0;
    private bool isattackone = false;
    private bool iscombo = false;
    private bool isattacktwo = false;
    private bool iscombojump = false;
    private bool isrush = false;
    private bool isangryattack = false;
    private bool iswalk = false;
    public bool afterangry = false;
 public bool  shouldTriggerAngryAfterJump = false; 


[Header("BGM 控制")]
[Tooltip("最大音量距离")] public float minDistance = 0f;
[Tooltip("最小音量距离")] public float maxDistance = 30f;
[Tooltip("最小音量")] [Range(0,1)] public float minVolume = 0.3f;
[Tooltip("最大音量")] [Range(0,2)] public float maxVolume = 1.5f;
private Coroutine bgmCoroutine; // 用于控制协程


    public void SetStatus(string statusName)
    {
        isattackone = false;
        isattacktwo = false;
        iswalk = false;
        iscombo = false;
        iscombojump = false;
        isrush = false;
        isangryattack = false;

        switch (statusName)
        {
            case "isattackone":
                isattackone = true;
                break;
            case "isattacktwo":
                isattacktwo = true;
                break;
            case "iswalk":
                iswalk = true;
                break;
            case "iscombo":
                iscombo = true;
                break;
            case "iscombojump":
                iscombojump = true;
                break;
            case "isrush":
                isrush = true;
                break;
            case "isangryattack":
                isangryattack = true;
                break;
            default:
                Debug.LogWarning("Invalid status name");
                break;
        }

        UpdateAnimatorParameters();
    }

    private void UpdateAnimatorParameters()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("isattackone", isattackone);
            animator.SetBool("isattacktwo", isattacktwo);
            animator.SetBool("iswalk", iswalk);
            animator.SetBool("iscombo", iscombo);
            animator.SetBool("isrush", isrush);
            animator.SetBool("iscombojump", iscombojump);
            animator.SetBool("isangryattack", isangryattack);
        }
    }

    public void ResetAllStates()
    {
        isattackone = false;
        isattacktwo = false;
        iswalk = false;
        iscombo = false;
        iscombojump = false;
        isrush = false;
        isangryattack = false;
        UpdateAnimatorParameters();
    }

    void Start()
    {

        animator = GetComponent<Animator>();

        bossHealth = GetComponent<bossHealth>();
        if (bossHealth == null)
        {
            Debug.LogError("KnightHealth component not found!");
        }
StartCoroutine(findplayer());

        if (player == null) Debug.LogError("Player not found!");

        audioSource = GetComponent<AudioSource>();

        DontDestroyOnLoad(gameObject);
        agent.speed = walkSpeed;
        animator.SetBool("iswalk", true);

        boxCollider = gameObject.GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }
        boxCollider.isTrigger = true;
    }

IEnumerator findplayer()
{
    while (player == null)
    {
        // 优先通过 GameManager 获取玩家
        if (gamemanager.Instance != null && gamemanager.Instance.player != null)
        {
            player = gamemanager.Instance.player.transform;
            Debug.Log("通过 GameManager 找到玩家");
        }
        // 备用方案：通过标签查找玩家
        else
        {
            player = GameObject.FindWithTag("Player")?.transform;
            if (player != null)
            {
                Debug.Log("通过标签找到玩家");
                // 将找到的玩家赋值给 GameManager（如果存在）
                if (gamemanager.Instance != null)
                {
                    gamemanager.Instance.player = player.gameObject;
                }
            }
        }

        // 如果仍未找到玩家，等待 1 秒后重试
        if (player == null)
        {
            Debug.LogWarning("未找到玩家，1 秒后重试");
            yield return new WaitForSeconds(1f);
        }
    }
if(bgmCoroutine != null) StopCoroutine(bgmCoroutine);
    bgmCoroutine = StartCoroutine(BGMVolumeControl());
    Debug.Log("玩家已找到：" + player.name);
}

IEnumerator BGMVolumeControl()
{
    // 确保音频组件存在
    if(audioSource == null)
    {
        Debug.LogError("AudioSource组件缺失！");
        yield break;
    }

    while(true)
    {
        // 安全检测
        if(player == null || audioSource == null)
        {
            Debug.LogWarning("玩家或音频源丢失，停止BGM控制");
            yield break;
        }

        // 计算距离
        float distance = Vector3.Distance(transform.position, player.position);
        
        // 计算音量比例（反向插值）
        float volumeRatio = Mathf.Clamp01(1 - (distance / maxDistance));
        
        // 计算最终音量
        float targetVolume = Mathf.Lerp(minVolume, maxVolume, volumeRatio);
        
        // 应用音量
        audioSource.volume = Mathf.Clamp(targetVolume, minVolume, maxVolume);

        // 调试输出（可选）
        // Debug.Log($"Distance: {distance}, Volume: {audioSource.volume}");

        yield return new WaitForSeconds(0.8f); // 更新间隔改为0.1秒更平滑
    }
}

// 在OnDestroy中停止协程
void OnDestroy()
{
    if(bgmCoroutine != null)
    {
        StopCoroutine(bgmCoroutine);
    }
}




public void ApplyFlashEffect(){
    walkSpeed=flashSpeed;
    agent.speed=flashSpeed;
    Invoke("EndFlash",2.5f);
    
}
public void EndFlash(){
    walkSpeed=originSpeed;
    agent.speed=originSpeed;
}





    void Update()
{ Debug.Log($"当前状态: shouldTriggerAngryAfterJump={shouldTriggerAngryAfterJump}, afterangry={afterangry}");

    if (shouldTriggerAngryAfterJump)
    {
        Debug.Log("检测到跳跃后需要触发愤怒攻击！");
        angryattack();
        shouldTriggerAngryAfterJump = false;
        return;
    }
   if (isangryattack) return;




      AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
    if (clipInfo.Length > 0)
    {
        Debug.Log("当前动画: " + clipInfo[0].clip.name);
    }

 if (animator.GetCurrentAnimatorStateInfo(0).IsName("jumpattack")) // 替换为你的跳跃动画名称  
    {  
        isJumping = true; // 正在跳跃  
    }  
    else  
    {  
        isJumping = false; // 不在跳跃  
    }  



        Debug.Log("iscombo: " + iscombo + ", iscombojump: " + iscombojump);

        if (bossHealth.health < 0.5 * 2000)
        {
            angrytransstate();
            afterangry = true;
        }
if(bossHealth.health==0){
bossHealth.Die();
    
}
        if (player == null)
        {
            Debug.LogWarning("Player not found!");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        TryCombo(distanceToPlayer);

        if (Time.time - lastRushTime >= rushCooldown && distanceToPlayer <= switchToRushDistance)
        {
            RunTowardsPlayer();
            return;
        }

        Tryangry(distanceToPlayer);

        if (!iscombo && !isangryattack && !iscombojump && !isrush)
        {
            if (distanceToPlayer <= attackDistance)
            {
                if (!isattackone && !isattacktwo && isallowattack)
                {
                    Attack(distanceToPlayer);
                }
                if(lianxie){
                    Attack(distanceToPlayer);
                }
            }
            else
            {
                WalkTowardsPlayer();
            }
        }

        if (!isattackone && !isattacktwo && !iscombo && !isangryattack && !iscombojump && !isrush)
        {
            FacePlayer();
        }
    }

    private void WalkTowardsPlayer()
    {
        agent.speed = walkSpeed;
        SetStatus("iswalk");
        agent.SetDestination(player.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Debug.Log("Bullet hit the knight!");
            TakeHit();
            angryspeed();
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Player"))
        {
        }
    }
public void angryspeed(){  while(walkSpeed<=9){
    walkSpeed=walkSpeed+1;}
  if(walkSpeed>=8){
    unangryspeed();
  }
   
    Debug.Log("受到攻击！boss移速增加");
}
public void unangryspeed(){
    walkSpeed=4f;
}


    public void TakeHit()
    {
        hitCount++;
        hitspeed++;
        if (hitspeed >= 10)
        {
            walkSpeed = walkSpeed - 1;
            Invoke("hitspeedtest", 1f);
        }
    }

    private void hitspeedtest()
    {
        walkSpeed = walkSpeed + 1;
    }

    private void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }



    public void angrytransstate()
    {
        if (bossHealth.health < 0.5f * 2000 && !afterangry)
        {
            animator.SetTrigger("isangry");
            afterangry = true;
            Debug.Log("进入愤怒状态！");
            StartCoroutine(TriggerAngryAttackAfterAnimation());
        }
    }

    private IEnumerator TriggerAngryAttackAfterAnimation()
    {
        yield return new WaitForSeconds(2.8f);
        angryattack();
    }

    public void angryattack()
    {
        ResetAllStates();
    jumpAttackCooldown = Mathf.Max(0, jumpAttackCooldown - 1f);
        comboCooldown = Mathf.Max(0, comboCooldown - 1f);
        walkSpeed += 1f;

        SetStatus("isangryattack");
        isangryattack = true;
        Debug.Log("触发愤怒攻击！");

        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        if (attack != null) attack.SetActive(true);

        StartCoroutine(EndAngryAttack());
    }

    private IEnumerator EndAngryAttack()
    {
        yield return new WaitForSeconds(3.2f);
        if (attack != null) attack.SetActive(false);

        ResetAllStates();
        isallowattack = true;
        isangryattack = false;

        Debug.Log("愤怒攻击结束，恢复正常攻击");
    }

    public void Tryangry(float distanceToPlayer)
    {
        // 仅在非跳跃后愤怒攻击时触发，且 isangryattack 为 false
        if (!shouldTriggerAngryAfterJump && 
            distanceToPlayer <= angryattackdistance && 
            Time.time >= lastangryattacktime + angryattackcooldown && 
            afterangry && 
            !isangryattack) // 新增条件
        {
            angryattack();
            lastangryattacktime = Time.time;
        }
    }
/*
    public void angrytransstate()
    {
        // 仅在血量低于50%且未进入愤怒状态时触发
        if (bossHealth.health < 0.5f * 2000 && !afterangry)
        {
            animator.SetTrigger("isangry");
            afterangry = true; // 标记已进入愤怒状态
            Debug.Log("进入愤怒状态！");
            Invoke("angrystate", 2.8f);
        }
    }

    private void angrystate()
    {
        afterangry = true;
        jumpAttackCooldown = Mathf.Max(0, jumpAttackCooldown - 1f);
        comboCooldown = Mathf.Max(0, comboCooldown - 1f);
        walkSpeed += 1f;
    }
*/
    public void Attack(float distanceToPlayer)
    {
        if (distanceToPlayer <= attackDistance)
        {
            ExecuteAttackOne();
        }
        else
        {
            Debug.Log("未到距离！");
            agent.isStopped = false;
            WalkTowardsPlayer();
            return;
        }
    }

    public void ExecuteAttackOne()
    {
        Debug.Log("攻击1");
        isattackone = true;
        isattacktwo = false;
        SetStatus("isattackone");
        Invoke("ExecuteAttackTwo", 1.1f);
    }

    public void ExecuteAttackTwo()
    {
        isattackone = false;
        isattacktwo = true;
        SetStatus("isattacktwo");
        Invoke("ResetAllStates", 2.5f);
    }

    public void RunTowardsPlayer()
    {
        if (Time.time - lastRushTime < rushCooldown)
        {
            return;
        }

        agent.speed = rushSpeed;
        agent.SetDestination(player.position);
        SetStatus("isrush");
        Invoke("EndRush", 2f);
    }

    private void EndRush()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        lastRushTime = Time.time;

        ResetAllStates();
        isrush = false;

        if (distanceToPlayer < attackDistance)
        {
            if (!iscombo && Time.time >= lastComboTime + comboCooldown)
            {
                TryCombo(distanceToPlayer);
            }
            else
            {
                Attack(distanceToPlayer);
            }
        }
        else
        {
            WalkTowardsPlayer();
        }

        agent.isStopped = false;
    }

public bool isJumping = false;  

public void TryJumpAttack()  
{  
    isallowattack = false;  
    UpdateAnimatorParameters();  
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);  
    
    if (distanceToPlayer <= jumpDistance && Time.time >= lastJumpAttackTime + jumpAttackCooldown)  
    {  
        lastJumpAttackTime = Time.time;  
        animator.SetTrigger("triggerJump");  
        Debug.Log("触发跳跃攻击！");  

        // 标记需要触发一次愤怒攻击
        shouldTriggerAngryAfterJump = true; // 新增此行

        Invoke("ResetJumpingState", 2f);  
    }  
    else  
    {  
        Debug.Log("不满足跳跃攻击条件");  
        ResetJumpingState();  
    }  
}
private void ResetJumpingState()  
{  
    isJumping = false; // 重置跳跃状态  
    isallowattack = true; // 允许其他攻击  
    ResetAllStates(); // 重置所有状态  
}
    public void TryCombo(float distanceToPlayer)
    {
        if (distanceToPlayer <= comboDistance && Time.time >= lastComboTime + comboCooldown)
        {
            Debug.Log("Combo条件满足，触发连击");
            ComboAttack();
        }
    }

    public void ComboAttack()
    {
        iscombo = true;
        SetStatus("iscombo");
        Debug.Log("组合键1");
        StartCoroutine(ComboSequence());
    }

    private IEnumerator ComboSequence()
    {
        yield return new WaitForSeconds(1.5f);
        iscombo = false;
        iscombojump = true;
        SetStatus("iscombojump");
        Debug.Log("组合键2");
        yield return new WaitForSeconds(2f);
        endcombo();
    }

    public void endcombo()
    {
        lastComboTime = Time.time;
        ResetAllStates();
        TryJumpAttack(); // 连击结束后触发跳跃
        iscombojump = false;
        Debug.Log("进入冷却！");
    }
}