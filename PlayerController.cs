using UnityEngine;  

public class PlayerController : MonoBehaviour  
{  






 void Awake()  
    {  
  DontDestroyOnLoad(gameObject); 
    }  




    private CharacterController characterController;  
    public Vector3 moveDirection;  
private AudioSource audioSource;

    [Header("玩家设置")]  
    [Tooltip("行走速度")]  
    public float walkSpeed = 4f; 
    public float originalWalkSpeed=4f; 
    [Tooltip("奔跑速度")]  
    public float runSpeed = 6f;
    public float originalRunSpeed=6f;  
    [Tooltip("下蹲速度")]  
    public float crouchSpeed = 0.5f;  
    [Tooltip("跳跃力")]  
    public float jumpForce = 5f;  
    [Tooltip("重力强度")]  
    public float fallForce = 10f;  
    [Tooltip("下蹲时的高度")]  
    public float crouchHeight = 1f;  
    [Tooltip("站立时的高度")]  
    public float standHeight = 2f; // 应根据需要设置  

      [Tooltip("下蹲走的速度")] 
public float CrouchWalkSpeed =1f;

public float Speed;
    [Header("输入设置")]  
    [Tooltip("奔跑键")]  
    public KeyCode runInputName = KeyCode.LeftShift;  
    [Tooltip("跳跃键")]  
    public KeyCode jumpInputName = KeyCode.Space;  
    [Tooltip("下蹲键")]  
    public KeyCode crouchInputName = KeyCode.E;  

    [Header("玩家状态")]  
    public MovementState state;  
    public CollisionFlags collisionFlags;  
    public bool isWalk;  
    public bool isRun;  
    public bool isJump;  
    public bool isGround = true;  
    public bool isCanCrouch;  
    public bool isCrouching;  
  public bool wasjump;  
    public LayerMask crouchLayerMask;  
    public bool istransform=false;
 // 下蹲速度取当前行走速度的30%
[Header("音效")]
[Tooltip("行走音效")]public AudioClip walkSound;
[Tooltip("奔跑音效")]public AudioClip runningSound;
[Tooltip("跳跃音效")]public AudioClip JumpSound;


[Tooltip("血量UI")]private PlayerHealth playerHealth; 

    void Start()  
    {  


playerHealth = GetComponent<PlayerHealth>();
         isCrouching = false;
        characterController = GetComponent<CharacterController>();  
        standHeight = characterController.height; 
                audioSource=GetComponent<AudioSource>();
    }  

    void Update()  
    {  

   if (Input.GetKeyDown(KeyCode.BackQuote)) // BackQuote 在键盘上是 ~ 键  
    {  
        playerHealth.RequestInputFieldToggle(); // 切换输入框可见性  
    }  

    // 示例：当玩家摔落时扣血  
    if (transform.position.y < -10) // 假设低于 y = -10 视为摔落  
    {  
        playerHealth.TakeDamage(10); // 扣除100点血量  
        transform.position = new Vector3(0, 1, 0); // 重置位置（根据需要）  
    }  





        CanCrouch(); // 检测是否可以下蹲  
if(Input.GetKey(crouchInputName))
{
    Crouch(true);

}else{

    Crouch(false);
}
 PlayerFootSoundSet();
        Jump(); // 处理跳跃  
        Moving(); // 处理移动  
       
    }  


public void ReduceSpeed(float speedReduction)
    {
        walkSpeed -= speedReduction;
        runSpeed -= speedReduction;

        // 5 秒后恢复速度
        Invoke("RestoreSpeed", 8f);
    }

    // 恢复玩家速度
    private void RestoreSpeed()
    {
        walkSpeed = originalWalkSpeed;
        runSpeed = originalRunSpeed;
    }



   public void Moving(){ 


    if(!isWalk){
         moveDirection = Vector3.zero; 
    }
    
    float h = Input.GetAxisRaw("Horizontal"); 
   float v = Input.GetAxisRaw("Vertical");
    isRun = Input.GetKey(runInputName);
     isWalk = (Mathf.Abs(h) >0 || Mathf.Abs(v) >0); 
     Vector3 cameraForward = Camera.main.transform.forward; cameraForward.y =0; //让Y分量为0，保持在水平面上 
     cameraForward.Normalize(); //归一化 
     Vector3 cameraRight = Camera.main.transform.right; cameraRight.y =0; //让Y分量为0 
     cameraRight.Normalize(); // 检查是否下蹲并且在地面上 
     if (isCrouching && isWalk && isGround && Input.GetKey(crouchInputName)) { Speed = crouchSpeed; // 设置为下蹲速度
      state = MovementState.crouching;
       moveDirection = (cameraRight * h + cameraForward * v).normalized;
        characterController.Move(moveDirection * Speed * Time.deltaTime);
         } else if (isRun && isGround) {
             state = MovementState.running; 
             moveDirection = (cameraRight * h + cameraForward * v).normalized;
              characterController.Move(moveDirection * runSpeed * Time.deltaTime); }
               else if (isWalk && isGround) { state = MovementState.walking;
                moveDirection = (cameraRight * h + cameraForward * v).normalized;
                 characterController.Move(moveDirection * walkSpeed * Time.deltaTime); 
                 } else { state = MovementState.idle; }
                  if (isCrouching) { characterController.height = crouchHeight;
                   characterController.center = new Vector3(0, crouchHeight /2 ,0); // 调整中心位置


 /*
 if(istransform){
  transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
istransform=false;
 }*/
                    } else { characterController.height = standHeight; //还原为站立高度
                     characterController.center = new Vector3(0, standHeight /2,0); //还原中心位置 
                       /* istransform=true;

                 if(istransform){
  transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
istransform=false;
 }*/
 
                     }
                     
                     
                     
                     }
public void Jump()  
{  
    isJump = Input.GetKeyDown(jumpInputName);  
    if (isJump && isGround)  
    {  wasjump=true;
        isGround = false;  
        jumpForce = 4f;  
audioSource.PlayOneShot(JumpSound);
Debug.Log("isGround: " + isGround);
    }  else if(!isJump&&isGround){
        isGround=false;

    }
 



    if (!isGround)  
    {  
        jumpForce -= fallForce * Time.deltaTime;  

        // 获取当前的水平移动方向并结合Y轴的跳跃力  
        Vector3 jumpMovement = new Vector3(moveDirection.x * (isRun ? runSpeed : (isCrouching ? crouchSpeed : walkSpeed)), jumpForce, moveDirection.z * (isRun ? runSpeed : (isCrouching ? crouchSpeed : walkSpeed)));  
        collisionFlags = characterController.Move(jumpMovement * Time.deltaTime);  // 注意这里乘以Time.deltaTime  
Debug.Log("flag:"+collisionFlags);
        if (collisionFlags == CollisionFlags.Below)  
        {  
            isGround = true;  
            jumpForce = -4f;  
         
        }  
/*
        if (collisionFlags == CollisionFlags.None)  
        {  
            isGround = false;  
        }  */
    }  
}

    public void CanCrouch()  
    {  
        // 计算头顶的位置  
        Vector3 sphereLocation = transform.position + Vector3.up * standHeight;  

        // 检测头顶是否有物体  
        isCanCrouch = (Physics.OverlapSphere(sphereLocation, characterController.radius, crouchLayerMask).Length == 0);  
        Debug.Log("isCanCrouch: " + isCanCrouch);  

        // 切换下蹲状态  
        if (Input.GetKeyDown(crouchInputName) && isCanCrouch)  
        {  
            isCrouching = !isCrouching; // 切换下蹲状态  
        }  
    }  

public void Crouch(bool newCrouching){
    Debug.Log("是否允许下蹲");
      float heightDelta = standHeight - crouchHeight; // 通常=1
if(!isCanCrouch) return;
isCrouching =newCrouching;
characterController.height=isCrouching? crouchHeight : standHeight;
characterController.center=characterController.height/2.0f *Vector3.up;
 // 直接控制物体位置偏移
      transform.position += Vector3.up * (isCrouching ? heightDelta/2 : -heightDelta/2);
}








    public void PlayerFootSoundSet(){
 if (isGround && moveDirection.sqrMagnitude >0)  
 {  
 audioSource.clip = isRun ? runningSound : walkSound;  
 if (!audioSource.isPlaying)  
 {  
 audioSource.Play();  
 }  
 }  
 else {  
 if (audioSource.isPlaying)  
 {  
 audioSource.Pause();  
 }  
 }  

 if (isCrouching)  
 {  
 if (audioSource.isPlaying)  
 {  
 audioSource.Pause();  
 }  
 }  

   if (isJump && !audioSource.isPlaying )  
 {  
 audioSource.PlayOneShot(JumpSound);  
 }  

}  


    public enum MovementState  
    {  
        walking,  
        running,  
        crouching,  
        idle  
    }  



    // 其他更新代码...  

    // 检测 ~ 键  
 
}  






