using UnityEngine;
using UnityEngine.AI;

public class EnemyZombie : MonoBehaviour
{
    [Header("Settings")]
    public float originSpeed=3.5f;
    public float moveSpeed = 3.5f;
    public float attackDistance = 2f;
    public float attackCooldown = 5f;
    public int attackDamage = 5;
public float flashSpeed=0.2f;
//public int flashtime=3f;
    [Header("References")]
    private Transform player;
    public NavMeshAgent agent;
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip attackSound;
public GameObject zombieattackrange;

    private float lastAttackTime;
    private bool isAttacking;
    private zombiehealth zombieHealth;

    void Start()
    {
        animator = GetComponent<Animator>();
        zombieattackrange.SetActive(false);
        zombieHealth = GetComponent<zombiehealth>();
        agent.speed = moveSpeed;
        animator.SetBool("ismove", true);
        agent.isStopped = false;
    }
/*
void Update(){

  if (gamemanager.Instance != null && gamemanager.Instance.player != null)
        {
            player = gamemanager.Instance.player.transform;
        }
        else
        {
            Debug.LogError("无法通过 GameManager 获取玩家引用！");
            enabled = false; // 禁用脚本
            return;
        }

 if (transform.position.y < -15f)
        {
            Destroy(gameObject);
            return;
        }

        // 原有逻辑（略作调整）
        if (zombieHealth.health <= 0) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);



    
}

*/


 public void ApplyFlashEffect(){
    moveSpeed = flashSpeed;
        agent.speed = flashSpeed;
        Invoke("EndFlash",3f) ;
 }
public void EndFlash(){
moveSpeed=originSpeed;
agent.speed=originSpeed;


}

    void Update()
    {
     //   if (zombieHealth.health <= 0) return;


  if (gamemanager.Instance != null && gamemanager.Instance.player != null)
        {
            player = gamemanager.Instance.player.transform;
        }
        else
        {
            Debug.LogError("无法通过 GameManager 获取玩家引用！");
            enabled = false; // 禁用脚本
            return;
        }
/*
 if (transform.position.y < -10f)
        {
            Destroy(gameObject);
            return;
        }
*/
        // 原有逻辑（略作调整）
        if (zombieHealth.health <= 0) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);







       // float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackDistance)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                StartAttack();
            }
            else
            {
                StopMovement();
            }
        }
        else
        {
            MoveToPlayer();
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        agent.isStopped = true;

        animator.SetBool("IsAttacking", true); // 使用 bool 类型控制攻击动画
        audioSource.PlayOneShot(attackSound);
Debug.Log("attack");




        // 延迟重置攻击状态
        Invoke("ResetAttack", 3.5f);
    }

 void ResetAttack()
{
    isAttacking = false;
    agent.isStopped = false;
    animator.SetBool("IsAttacking", false);

    Debug.Log("Agent isStopped: " + agent.isStopped);
    MoveToPlayer();
}
   public  void MoveToPlayer()
    {
        if (isAttacking) return;

        agent.SetDestination(player.position);
        animator.SetBool("ismove", true);
        agent.isStopped = false;
          Debug.Log("Moving to player, ismove: true"); // 调试日志
    }

    void StopMovement()
    {
        agent.isStopped = true;
        animator.SetBool("ismove", false);
        FacePlayer();
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // 攻击碰撞体的触发检测
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by zombie attack!");
            other.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        }
    }
}
