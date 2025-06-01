using UnityEngine;  
using UnityEngine.UI;  
using System.Collections;
public class AdvancedCarController : MonoBehaviour  
{  
    [Header("基本移动参数")]  
    [SerializeField] float maxNormalSpeed = 100f;    
    [SerializeField] float enginePower = 10f;       
    [SerializeField] float brakeDeceleration = 10f;   
    [SerializeField] float dragCoefficient = 0.05f;   

    [Header("转向系统")]  
    [SerializeField] Transform steeringPivot;      
    [SerializeField] float maxSteerAngle = 35f;   
 //   [SerializeField] float steeringTorque = 3f; // 降低转向力度  
    [SerializeField] float speedSensitivity = 0.8f;  

    [Header("氮气系统")]  
    [SerializeField] float nitroBoostForce = 50f;  
    [SerializeField] float nitroDuration = 1.5f;    
    [SerializeField] float driftNitroGain = 0.15f;   
    [SerializeField] Image nitroFillImage;         

    [Header("漂移系统")]  
    [SerializeField] float driftForce = 8f;        
    [SerializeField] float driftDrag = 2f;         

    // 状态变量  
    private Rigidbody rb;  
    private float currentSpeed;  
    private float nitroAmount = 0f;  
    private bool isDrifting = false;  
    private bool isNitroActive = false;  
    private float originalDrag;  

 [SerializeField] float minSteerMultiplier = 0.3f;    // 高速时最小转向系数
    [SerializeField] float driftAngleThreshold = 15f;    // 漂移角度阈值
    [SerializeField] float[] driftForceMultipliers = new float[2] {1f, -1f}; // 左右漂移力系数

    // 新增状态变量
    private float currentSteerMultiplier = 1f;
    private Vector3 initialForward;  // 漂移初始方向
    private float driftDirection;    // 漂移方向(1:右,-1:左)

[Header("漂移系统 - 重构版")]
    [SerializeField] Transform[] driftForcePoints = new Transform[2]; // 0:左后轮 1:右后轮
    [SerializeField] float minDriftSpeed = 15f;       // 可触发漂移的最低速度
    [SerializeField] float driftAcceleration = 5f;    // 漂移横向加速度  
    [SerializeField] float driftSteeringBoost = 2f;   // 漂移时转向增益
    [SerializeField] float driftExitSmoothness = 5f;  // 漂移结束平滑系数
 private bool isDriftKeyPressed;
    private float currentDriftFactor; // 0-1漂移强度
    private Vector3 preDriftForward;  // 漂移前方向


[Header("地面物理参数")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float[] frictionCoefficients = new float[3] { 
        1.2f, // 0:普通地面
        0.3f, // 1:漂移路面
        0.6f  // 2:中间过渡
    };
    [SerializeField] float raycastLength = 0.5f;
    [SerializeField] float wheelRadius = 0.3f;

    // 地面检测
    private float currentFriction = 1f;
    private int currentSurfaceType = 0;
    private RaycastHit groundHit;

[Header("转向系统改进")]
[SerializeField] float maxSteeringAngle = 36f; // 最大有效转向角度
[SerializeField] float angleDamping = 0.8f;    // 角度衰减系数
[SerializeField] AnimationCurve steeringCurve = new AnimationCurve(
    new Keyframe(0, 1),
    new Keyframe(30, 0.8f),
    new Keyframe(45, 0.6f)
);




private float currentSteeringAngle;


[Header("转向系统")]
[Tooltip("基础转向扭矩（建议值：3-5）")]
[SerializeField] float steeringTorque = 3f;

[Tooltip("静止时转向增益（速度=0时的转向倍数，建议值：1.5-2.5）")]
[SerializeField] float zeroSpeedSteering = 2f;

[Tooltip("高速时转向衰减（最大速度时的转向比例，建议值：0.1-0.5）")]
[SerializeField] float highSpeedSteeringMultiplier = 0.3f;

[Tooltip("转向响应曲线（X轴：速度比例，Y轴：转向系数）")]
[SerializeField] AnimationCurve steeringResponseCurve = new AnimationCurve(
    new Keyframe(0, 2f),    // 静止时200%转向
    new Keyframe(0.3f, 1f), // 30%速度时100%
    new Keyframe(1f, 0.3f)  // 全速时30%
);

[Tooltip("转向平滑时间（秒，建议值：0.1-0.3）")]
[SerializeField] float steeringSmoothTime = 0.2f;
private float currentSteeringVelocity;


[Header("切换视角")]
[Tooltip("第一人称")]
//public Camera oneperson;
public bool onekey=false;
public GameObject onegameObject;

    void Start()  
    {  
StartCoroutine(checkoneperson());
 
        rb = GetComponent<Rigidbody>();  
        originalDrag = rb.drag;  
        rb.centerOfMass = Vector3.down * 0.5f; // 确保重心位置合理  
         initialForward = transform.forward;
    }  

    void Update()  
    {  

        HandleInput();  
        UpdateNitroUI();  
          bool newDriftInput = Input.GetKey(KeyCode.LeftShift);
        if(newDriftInput != isDriftKeyPressed)
        {
            isDriftKeyPressed = newDriftInput;
            if(isDriftKeyPressed) TryStartDrift();
            else EndDrift();
        }

         DetectGroundSurface();

   if (Input.GetKeyDown(KeyCode.P))
    {
        ForceFullNitro();
    }


    }  
IEnumerator checkoneperson()
{
    while(true)
    {
        // 每帧检测输入
        if(Input.GetKeyDown(KeyCode.V))
        {
            ToggleCamera();
        }
        yield return null; // 等待下一帧
    }
}

void ToggleCamera()
{
    onekey = !onekey; // 切换状态
    onegameObject.SetActive(!onegameObject.activeSelf); // 直接根据当前状态切换
    
    // 调试日志
    Debug.Log($"摄像头状态切换至：{(onegameObject.activeSelf ? "启用" : "禁用")}");
}

void ForceFullNitro()
{
    nitroAmount = 1f; // 强制充满氮气
    UpdateNitroUI();  // 立即更新UI
    /*
    if(!isNitroActive) // 防止重复激活
    {
        StartCoroutine(ActivateNitro());
    }
    else // 如果已在加速则重置时间
    {
        StopCoroutine(ActivateNitro());
        StartCoroutine(ActivateNitro());
    }

    */
}


     void FixedUpdate()
    {

        ApplyFriction();
        ApplyEngineForce();
        ApplySteering();
        ApplyDrag();

        if(isDrifting)
        {
            UpdateDriftPhysics();
        }
    }


 void DetectGroundSurface()
    {
        if(Physics.Raycast(transform.position + Vector3.up * 0.1f, 
                          Vector3.down, 
                          out groundHit, 
                          raycastLength, 
                          groundLayer))
        {
            // 通过tag或材质识别地面类型
            if(groundHit.collider.CompareTag("DriftRoad"))
                currentSurfaceType = 1;
            else if(groundHit.collider.CompareTag("DirtRoad"))
                currentSurfaceType = 2;
            else
                currentSurfaceType = 0;
        }
    }

    void ApplyFriction()
    {
        // 动态摩擦系数计算
        float targetFriction = frictionCoefficients[currentSurfaceType];
        
        // 漂移时额外降低摩擦
        if(isDrifting) 
            targetFriction *= 0.4f;

        currentFriction = Mathf.Lerp(currentFriction, targetFriction, Time.fixedDeltaTime * 5f);

        // 应用物理摩擦
        Vector3 forwardFriction = -rb.velocity.normalized * 
                                Vector3.Dot(rb.velocity, transform.forward) * 
                                currentFriction;

        Vector3 sideFriction = -rb.velocity.normalized * 
                             Vector3.Dot(rb.velocity, transform.right) * 
                             currentFriction * 1.5f; // 侧向更高摩擦

        rb.AddForce(forwardFriction + sideFriction, ForceMode.Acceleration);

        // 轮胎滚动摩擦模拟
        ApplyWheelFriction();
    }

    void ApplyWheelFriction()
    {
        // 模拟四个轮胎的抓地力
        for(int i = 0; i < 4; i++)
        {
            Vector3 wheelPos = transform.position + 
                              transform.forward * ((i < 2) ? -1 : 1) * 1.2f +
                              transform.right * ((i % 2 == 0) ? -1 : 1) * 0.8f;

            if(Physics.SphereCast(wheelPos, wheelRadius, 
                                Vector3.down, 
                                out RaycastHit wheelHit, 
                                wheelRadius * 2f, 
                                groundLayer))
            {
                // 每个轮胎单独计算侧向力
                Vector3 wheelVel = rb.GetPointVelocity(wheelPos);
                Vector3 lateralFriction = -transform.right * 
                                         Vector3.Dot(wheelVel, transform.right) * 
                                         currentFriction * 2f;

                rb.AddForceAtPosition(lateralFriction, wheelPos, ForceMode.Acceleration);
            }
        }
    }








   void ApplyAdvancedSteering()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        
        // 动态转向系数（速度越高转向越迟钝）
        currentSteerMultiplier = Mathf.Lerp(1f, minSteerMultiplier, 
            rb.velocity.magnitude / maxNormalSpeed);
        
        // 物理转向
        float torqueAmount = horizontalInput * steeringTorque * currentSteerMultiplier;
        rb.AddTorque(transform.up * torqueAmount, ForceMode.VelocityChange);

        // 视觉转向平滑处理
        float targetAngle = horizontalInput * maxSteerAngle;
        float smoothAngle = Mathf.LerpAngle(
            steeringPivot.localEulerAngles.y, 
            targetAngle, 
            Time.fixedDeltaTime * 15f
        );
        steeringPivot.localRotation = Quaternion.Euler(0, smoothAngle, 0);
    }

    void StartDrift()  
    {  
        if(Mathf.Abs(Input.GetAxis("Horizontal")) < 0.1f) return;

        isDrifting = true;
        initialForward = transform.forward;
        driftDirection = Mathf.Sign(Input.GetAxis("Horizontal"));
        StartCoroutine(AccumulateNitro());
        
        // 降低抓地力效果
        rb.drag = driftDrag;
        rb.angularDrag = 0.5f; 
    }

    void ApplyEnhancedDrift()
    {
        // 计算侧滑方向
        Vector3 currentDirection = transform.forward;
        float angleDelta = Vector3.SignedAngle(initialForward, currentDirection, Vector3.up);

        // 当角度超过阈值时自动维持漂移
        if(Mathf.Abs(angleDelta) > driftAngleThreshold)
        {
            // 施加与转向方向相反的离心力
            Vector3 counterForce = transform.right * driftForce * driftDirection * -1f;
            rb.AddForce(counterForce * rb.velocity.magnitude * 0.1f, ForceMode.Acceleration);

            // 增加旋转扭矩维持漂移姿态
            rb.AddTorque(transform.up * driftDirection * steeringTorque * 0.8f, ForceMode.Acceleration);
        }

        // 后轮滑移效果
        ApplyRearWheelSlip();
    }

    void ApplyRearWheelSlip()
    {
        // 模拟后轮失去抓地力
        Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
        localVel.z *= 0.95f; // 保持前进动量
        localVel.x *= 1.2f;  // 增强横向滑移
        rb.velocity = transform.TransformDirection(localVel);
    }

   void TryStartDrift()
    {
        // 仅当满足条件时开始漂移
        if(rb.velocity.magnitude > minDriftSpeed && 
           Mathf.Abs(Input.GetAxis("Horizontal")) > 0.3f)
        {
            isDrifting = true;
            preDriftForward = transform.forward;
            currentDriftFactor = 0f;
            
            // 物理参数调整
            rb.drag = driftDrag;
            rb.angularDrag = 0.2f;
            
            StartCoroutine(AccumulateNitro());
        }
    }

    void UpdateDriftPhysics()
    {
        // 平滑增加漂移强度
        currentDriftFactor = Mathf.Clamp01(currentDriftFactor + Time.fixedDeltaTime * 2f);
        
        // 计算侧滑方向（根据当前转向输入）
        float steerDirection = Mathf.Sign(Input.GetAxis("Horizontal"));
        Vector3 driftForceDir = steerDirection * transform.right;

        // 在两个施力点施加力
        foreach(Transform point in driftForcePoints)
        {
            rb.AddForceAtPosition(
                driftForceDir * driftAcceleration * currentDriftFactor,
                point.position,
                ForceMode.Acceleration
            );
        }

        // 转向增强
        float steeringBoost = 1f + (driftSteeringBoost * currentDriftFactor);
        ApplyDriftSteering(steeringBoost);

        // 速度保持（防止失速）
        MaintainDriftSpeed();
    }

    void ApplyDriftSteering(float boostMultiplier)
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 torque = transform.up * 
                         horizontalInput * 
                         steeringTorque * 
                         boostMultiplier;
        
        rb.AddTorque(torque, ForceMode.VelocityChange);
        
        // 视觉转向
        steeringPivot.localRotation = Quaternion.Euler(
            0, 
            maxSteerAngle * horizontalInput, 
            0
        );
    }

    void MaintainDriftSpeed()
    {
        // 保持70%原速 + 30%当前速度
        Vector3 forwardVel = transform.forward * 
                           Vector3.Dot(rb.velocity, transform.forward);
        rb.velocity = Vector3.Lerp(rb.velocity, forwardVel, 0.3f);
    }

    void EndDrift()
    {
        if(!isDrifting) return;
        
        isDrifting = false;
        rb.drag = originalDrag;
        rb.angularDrag = 1f;
        
        // 平滑过渡回正常状态
        StartCoroutine(SmoothDriftExit());
    }

    IEnumerator SmoothDriftExit()
    {
        float progress = 0f;
        Vector3 startVel = rb.velocity;
        Vector3 targetVel = transform.forward * startVel.magnitude;

        while(progress < 1f && !isDrifting)
        {
            rb.velocity = Vector3.Lerp(
                startVel, 
                targetVel, 
                progress
            );
            
            progress += Time.deltaTime * driftExitSmoothness;
            yield return null;
        }
    }



    void HandleInput()  
    {  
        if (Input.GetKeyDown(KeyCode.Space) && nitroAmount >= 1f)  
        {  
            StartCoroutine(ActivateNitro());  
        }  

        if (Input.GetKeyDown(KeyCode.LeftShift))  
        {  
            StartDrift();  
        }  
        if (Input.GetKeyUp(KeyCode.LeftShift))  
        {  
            EndDrift();  
        }  
    }  
/*
    void ApplyEngineForce()  
    {  
        float verticalInput = Input.GetAxis("Vertical");  
        Vector3 engineDirection = transform.forward;  

        float targetSpeed = Mathf.Clamp(  
            currentSpeed + verticalInput * enginePower * Time.fixedDeltaTime,  
            -maxNormalSpeed,  // 允许后退的速度为负值  
            isNitroActive ? maxNormalSpeed * 1.5f : maxNormalSpeed  
        );  

        if (verticalInput != 0)  
        {  
            rb.AddForce(engineDirection * enginePower * verticalInput, ForceMode.Acceleration);  
        }  
        else  
        {  
            ApplyBraking();  
        }  
    }  
*/
/*
void ApplyEngineForce()  
{  
    float verticalInput = Input.GetAxis("Vertical");  
    Vector3 engineDirection = transform.forward;  

    // 新增反向速度限制
    float maxReverseSpeed = maxNormalSpeed * 0.6f; // 倒车最高速度为前进的60%
    float effectiveMaxSpeed = verticalInput > 0 ? 
        (isNitroActive ? maxNormalSpeed * 1.5f : maxNormalSpeed) : 
        maxReverseSpeed;

    float targetSpeed = Mathf.Clamp(  
        currentSpeed + verticalInput * enginePower * Time.fixedDeltaTime,  
        -effectiveMaxSpeed,  // 应用反向限制
        effectiveMaxSpeed  
    );  

    // 修改引擎力施加方式
    if (verticalInput != 0)  
    {  
        float powerMultiplier = verticalInput > 0 ? 1f : 0.6f; // 倒车动力减少40%
        rb.AddForce(engineDirection * enginePower * verticalInput * powerMultiplier, 
                  ForceMode.Acceleration);  
    }
    // ... [保持原有刹车逻辑]
 else  
        {  
            ApplyBraking();  
        }  

}
*/
void ApplyEngineForce()  
{
    float verticalInput = Input.GetAxis("Vertical");
    
    // 删除所有targetSpeed相关逻辑
    if (verticalInput != 0)  
    {
        float powerMultiplier = verticalInput > 0 ? 1f : 0.5f; // 倒车动力减半
        Vector3 forceDirection = verticalInput > 0 ? transform.forward : -transform.forward;
        
        // 直接基于刚体速度计算
        float currentForwardSpeed = Vector3.Dot(rb.velocity, transform.forward);
        float speedRatio = Mathf.Clamp01(Mathf.Abs(currentForwardSpeed) / maxNormalSpeed);
        float effectivePower = enginePower * (1 - speedRatio * 0.7f); // 速度越高动力衰减
        
        rb.AddForce(forceDirection * effectivePower * powerMultiplier, 
                  ForceMode.Acceleration);
    }
    else
    {
        // 增强版刹车
        ApplySmartBraking();
    }
}
void ApplySmartBraking()
{
    // 计算实际移动方向
    Vector3 realMoveDir = rb.velocity.normalized;
    float speed = rb.velocity.magnitude;
    
    // 动态刹车强度
    float brakePower = brakeDeceleration * 
                      Mathf.Clamp01(speed / maxNormalSpeed) *
                      Time.fixedDeltaTime;

    // 方向敏感的刹车力
    Vector3 brakeDirection = Vector3.Dot(realMoveDir, transform.forward) > 0 ? 
                           -transform.forward : 
                           transform.forward;
                           
    rb.AddForce(brakeDirection * brakePower * speed, 
              ForceMode.Acceleration);

    // 自动回正辅助
    if(speed < 2f)
    {
        Vector3 antiSpinTorque = -rb.angularVelocity * 5f;
        rb.AddTorque(antiSpinTorque, ForceMode.Acceleration);
    }
}




/*
void ApplySteering()  
{  
    float horizontalInput = Input.GetAxis("Horizontal");  

    // 施加转向扭矩，保持施加在 Y 轴  
    float steerAmount = horizontalInput * steeringTorque;  
    Vector3 torque = new Vector3(0, steerAmount, 0);  
    rb.AddTorque(torque, ForceMode.VelocityChange);  

    // 视觉转向  
    // 使用 Mathf.Abs()来确保方向键松开后立即归零  
    steeringPivot.localRotation = Quaternion.Euler(0, Mathf.Abs(horizontalInput) < 0.1f ? 0 : maxSteerAngle * horizontalInput, 0);  
}  
*/


/*
void ApplySteering()  
{  
    float horizontalInput = Input.GetAxis("Horizontal");  
    
    // 计算当前实际转向角度
    currentSteeringAngle = Vector3.SignedAngle(initialForward, transform.forward, Vector3.up);
    
    // 动态转向系数（包含速度和角度因素）
    float angleFactor = 1 - Mathf.Clamp01(Mathf.Abs(currentSteeringAngle) / maxSteeringAngle) * angleDamping;
    float speedFactor = Mathf.Lerp(1f, minSteerMultiplier, rb.velocity.magnitude / maxNormalSpeed);
    
    // 综合转向系数
    float finalSteeringMultiplier = steeringCurve.Evaluate(Mathf.Abs(currentSteeringAngle)) * speedFactor;
    
    // 施加转向扭矩
    Vector3 torque = transform.up * horizontalInput * steeringTorque * finalSteeringMultiplier;
    rb.AddTorque(torque, ForceMode.VelocityChange);  

    // 视觉转向（保留原始最大角度显示）
    steeringPivot.localRotation = Quaternion.Euler(0, maxSteerAngle * horizontalInput, 0);  
}
*/

void ApplySteering()  
{  
    float horizontalInput = Input.GetAxis("Horizontal");  

    // 计算速度比例（0-1）
    float speedRatio = Mathf.Clamp01(rb.velocity.magnitude / maxNormalSpeed);
    
    // 通过曲线获取转向系数
    float steeringMultiplier = steeringResponseCurve.Evaluate(speedRatio);
    
    // 平滑过渡转向系数
    float smoothMultiplier = Mathf.SmoothDamp(
        currentSteerMultiplier, 
        steeringMultiplier, 
        ref currentSteeringVelocity, 
        steeringSmoothTime
    );
    currentSteerMultiplier = smoothMultiplier;

    // 计算最终扭矩
    float finalTorque = horizontalInput * steeringTorque * currentSteerMultiplier;
    
    // 施加扭矩
    rb.AddTorque(transform.up * finalTorque, ForceMode.VelocityChange);

    // 视觉转向更新
    UpdateVisualSteering(horizontalInput);
}

void UpdateVisualSteering(float input)
{
    float targetAngle = input * maxSteerAngle;
    float smoothAngle = Mathf.LerpAngle(
        steeringPivot.localEulerAngles.y, 
        targetAngle, 
        Time.fixedDeltaTime * 15f
    );
    steeringPivot.localRotation = Quaternion.Euler(0, smoothAngle, 0);
}



    void ApplyBraking()  
    {  
        float brakeForce = Mathf.Min(brakeDeceleration * Time.fixedDeltaTime, rb.velocity.magnitude);  
        rb.AddForce(-rb.velocity.normalized * brakeForce, ForceMode.VelocityChange);  
    }  

    void ApplyDrag()  
    {  
              rb.drag = dragCoefficient * rb.velocity.magnitude;  
    }  
/*
    void StartDrift()  
    {  
        isDrifting = true;  
        StartCoroutine(AccumulateNitro());  
        rb.drag = driftDrag;  
    }  

    void EndDrift()  
    {  
        isDrifting = false;  
        rb.drag = originalDrag;  
    }  
*/
    void ApplyDriftPhysics()  
    {  
        // 施加侧向漂移力  
        Vector3 driftDirection = Vector3.Cross(rb.velocity.normalized, Vector3.up);  
        rb.AddForce(driftDirection * driftForce, ForceMode.Acceleration);  
    }  

    System.Collections.IEnumerator AccumulateNitro()  
    {  
        while (isDrifting)  
        {  
            nitroAmount = Mathf.Clamp01(nitroAmount + driftNitroGain);  
            yield return new WaitForSeconds(0.3f);  
        }  
    }  

    System.Collections.IEnumerator ActivateNitro()  
    {  
        isNitroActive = true;  
        float timer = 0f;  

        // 应用初始爆发力  
        rb.AddForce(transform.forward * nitroBoostForce, ForceMode.Impulse);  

        while (timer < nitroDuration)  
        {  
            // 持续推力  
            rb.AddForce(transform.forward * (nitroBoostForce * 0.2f), ForceMode.Acceleration);  
            nitroAmount -= Time.deltaTime / nitroDuration;  
            timer += Time.deltaTime;  
            yield return null;  
        }  

        isNitroActive = false;  
    }  

    void UpdateNitroUI()  
    {  
        nitroFillImage.fillAmount = nitroAmount;  
        nitroFillImage.color = nitroAmount >= 1f ? Color.cyan : Color.blue;  
    }  
}  