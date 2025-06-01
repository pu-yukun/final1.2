using UnityEngine;  
using UnityEngine.UI;  
using UnityEngine.EventSystems;  

public class PlayerControllerapk : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler  
{  
    private CharacterController characterController;  
    public Vector3 moveDirection;  
    private AudioSource audioSource;  

    [Header("玩家设置")]  
    public float walkSpeed = 4f;            // 行走速度  
    public float runSpeed = 6f;             // 奔跑速度  
    public float crouchSpeed = 0.5f;        // 下蹲速度  
    public float jumpForce = 5f;            // 跳跃力度  
    public float fallForce = 10f;           // 下落力度  
    public float crouchHeight = 1f;         // 下蹲高度  
    public float standHeight = 2f;          // 站立高度  
    public float Speed;  

    [Header("输入设置")]  
    public Button buttonJump;               // 跳跃按钮  
    public Button buttonCrouch;             // 下蹲按钮  
    public RectTransform joystickBackground; // 摇杆背景  
    public RectTransform joystickHandle;    // 摇杆手柄  
    private Vector2 joystickInput;          // 摇杆输入  
    public Camera playerCamera;              // 引用摄像头  

    [Header("玩家状态")]  
    public MovementState state;  
    public CollisionFlags collisionFlags;  
    public bool isWalk;  
    public bool isRun;  
    public bool isJump;  
    public bool isGround = true;  
    public bool isCanCrouch;  
    public bool isCrouching;  
    public bool wasJump; // 是否已经跳跃  
    public LayerMask crouchLayerMask;  

    [Header("音效")]  
    public AudioClip walkSound;            // 行走音效  
    public AudioClip runningSound;         // 奔跑音效  
    public AudioClip jumpSound;            // 跳跃音效  

    private float runTimer = 0f; // 计时器，用于检测摇杆输入持续时间  
    private const float runThreshold = 1f; // 进入奔跑状态的时间阈值  

    void Start()  
    {  
        isCrouching = false;  
        characterController = GetComponent<CharacterController>();  
        audioSource = GetComponent<AudioSource>();  

        // 为按钮添加事件  
        buttonJump.onClick.AddListener(Jump);  
        buttonCrouch.onClick.AddListener(ToggleCrouch);  
    }  

    void Update()  
    {  
        CanCrouch(); // 检测是否可以下蹲  
        PlayerFootSoundSet(); // 设置脚步音效  
        Moving(); // 处理移动  
    }  

    public void Move(Vector2 input)  
    {  
        if (isGround)  
        {  
            // 获取摄像头的前方向和右方向  
            Vector3 cameraForward = playerCamera.transform.forward;  
            Vector3 cameraRight = playerCamera.transform.right;  

            // 将摄像头的前方向和右方向投影到水平面（Y轴为0）  
            cameraForward.y = 0;  
            cameraRight.y = 0;  

            // 归一化方向  
            cameraForward.Normalize();  
            cameraRight.Normalize();  

            // 计算移动方向  
            Vector3 desiredMoveDirection = (cameraForward * input.y + cameraRight * input.x).normalized;  
            moveDirection = desiredMoveDirection;  

            // 根据状态设置速度  
            Speed = isCrouching ? crouchSpeed : (isRun ? runSpeed : walkSpeed);  
            characterController.Move(moveDirection * Speed * Time.deltaTime);  
            state = isRun ? MovementState.running : MovementState.walking;  
        }  
    }  

public void Jump()  
{  
    if (isGround)  
    {  
        isJump = true;  
        wasJump = true;  
        isGround = false;  
        jumpForce = 5f; // 确保跳跃力在每次跳跃时重置  
        audioSource.PlayOneShot(jumpSound);  
    }  
}  

void FixedUpdate()  
{  
    if (!isGround)  
    {  
        jumpForce -= fallForce * Time.deltaTime;  
        Vector3 jumpMovement = new Vector3(moveDirection.x * (isRun ? runSpeed : walkSpeed), jumpForce, moveDirection.z * (isRun ? runSpeed : walkSpeed));  
        collisionFlags = characterController.Move(jumpMovement * Time.deltaTime);  

        // 检查是否接触地面  
        if (collisionFlags == CollisionFlags.Below)  
        {  
            isGround = true;  
            jumpForce = 0f;  
            wasJump = false; // 重置跳跃状态  
        }  
    }  
}  


    public void ToggleCrouch()  
    {  
        if (isCanCrouch)  
        {  
            isCrouching = !isCrouching;  
            characterController.height = isCrouching ? crouchHeight : standHeight;  
            characterController.center = characterController.height / 2.0f * Vector3.up;  
        }  
    }  

    public void CanCrouch()  
    {  
        Vector3 sphereLocation = transform.position + Vector3.up * standHeight;  
        isCanCrouch = (Physics.CheckSphere(sphereLocation, 0.5f, crouchLayerMask) == false);  
    }  

    // 玩家脚步音效设置    
    void PlayerFootSoundSet()  
    {  
        audioSource.clip = isRun ? runningSound : walkSound;  
        if (!audioSource.isPlaying)  
        {  
            audioSource.Play();  
        }  
        else if (isCrouching)  
        {  
            if (audioSource.isPlaying)  
            {  
                audioSource.Pause();  
            }  
        }  
        else  
        {  
            if (audioSource.isPlaying && !isCrouching)  
            {  
                audioSource.Pause();  
            }  
        }  

        // 跳跃音效  
        if (isJump && !audioSource.isPlaying)  
        {  
            audioSource.PlayOneShot(jumpSound);  
            isJump = false;  // 重置跳跃状态  
        }  
    }  

    // 添加移动状态更新  
    void Moving()  
    {  
        if (isGround)  
        {  
            Move(joystickInput); // 使用摇杆输入移动  

            // 检查摇杆输入是否持续不变  
            if (joystickInput.magnitude > 0.1f)  // 根据需要调整灵敏度阈值  
            {  
                // 检测摇杆输入的方向  
                Vector2 currentInput = joystickInput.normalized;  

                // 检查输入是否保持不变  
                if (currentInput == joystickInput.normalized)  
                {  
                    runTimer += Time.deltaTime;  // 增加计时器  
                }  
                else  
                {  
                    runTimer = 0f;  // 重置计时器  
                }  

                // 如果保持不变超过 1 秒，则切换到奔跑状态  
                if (runTimer >= runThreshold)  
                {  
                    isRun = true;  // 切换到奔跑状态  
                }  
            }  
            else  
            {  
                runTimer = 0f; // 如果没有输入则重置计时器  
                isRun = false; // 如果没有输入则不为奔跑状态  
            }  
        }  
    }  

    // 更新摇杆输入   
    public void OnDrag(PointerEventData eventData)  
    {  
        Vector2 input = eventData.position - (Vector2)joystickBackground.position; // 获取相对输入  
        joystickInput = Vector2.ClampMagnitude(input.normalized, 1f); // 限制在圆形范围内  
        joystickHandle.anchoredPosition = joystickInput * (joystickBackground.sizeDelta.x / 2); // 更新摇杆手柄位置  
    }  

    public void OnPointerDown(PointerEventData eventData)  
    {  
        OnDrag(eventData); // 按下时立即处理拖动，用于初始化手柄位置  
    }  

    public void OnPointerUp(PointerEventData eventData)  
    {  
        joystickInput = Vector2.zero; // 释放时重置摇杆输入  
        joystickHandle.anchoredPosition = Vector2.zero; // 重置摇杆手柄位置  
        runTimer = 0f; // 释放时重置计时器  
        isRun = false; // 释放时停止奔跑状态  
    }  

    public enum MovementState  
    {  
        walking,  
        running,  
        crouching,  
        idle  
    }  
}