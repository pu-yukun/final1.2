using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HelicopterController : MonoBehaviour
{
    [Header("References")]
    public AudioSource boostAudio;
    public ControllerPanel ControlPanel;
    public Rigidbody HelicopterModel;
    public HeliRotorController MainRotorController;
    public HeliRotorController SubRotorController;
    public float engineBoostMultiplier = 1.5f;

    [Header("Control Settings")]
    public float TurnForce = 5f;
    public float ForwardForce = 15f;
    public float ForwardTiltForce = 20f;
    public float TurnTiltForce = 30f;
    public float EffectiveHeight = 150f;

    [Header("Engine Settings")]
    public float EngineForce = 30f;
    public float EngineForceIncrement = 0.1f;

    [Header("Audio Settings")]
    public float liftThreshold = 50f;
    private bool _hasPlayedAudio;

    [Header("Trigger Settings")]
    public string triggerTag = "rescueleave";
    public Canvas controlCanvas;

    private Vector2 _moveInput = Vector2.zero;
    private bool _isOnGround = true;
    private float currentLift;

    private Camera _mainCamera;
    private Transform _originalCameraParent;
    private Vector3 _originalCameraPosition;
    private Quaternion _originalCameraRotation;

    private bool _isControlEnabled = false; // 新增控制权2s延迟检测标志
    void Start()
    {
        ControlPanel.KeyPressed += OnKeyPressed;
        controlCanvas.gameObject.SetActive(false);

        // 初始化摄像机状态
        _mainCamera = Camera.main;
        if (_mainCamera != null)
        {
            _originalCameraParent = _mainCamera.transform.parent;
            _originalCameraPosition = _mainCamera.transform.localPosition;
            _originalCameraRotation = _mainCamera.transform.localRotation;
        }

        Debug.Log("直升机控制器初始化完成");
    }

    void Update()
    {
        HandleInput();
        UpdateGroundState();

        // 升力检测
      /*  if (!_hasPlayedAudio && currentLift >= liftThreshold)
        {
            PlayBoostAudio();
            _hasPlayedAudio = true;
        }
        else if (currentLift < liftThreshold)
        {
            _hasPlayedAudio = false;
        }*/

        // 场景切换检测
        if (_isControlEnabled && Input.anyKey)
        {
            StartCoroutine(HandoverControlAndSwitchScene());
        }
    }

    void FixedUpdate()
    {
        LiftProcess();
        MoveProcess();
        TiltProcess();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            controlCanvas.gameObject.SetActive(true);

                  Invoke("EnableControlDetection", 2f);
            Debug.Log("已离开着陆区，显示操作界面");
        }
    }

    private void EnableControlDetection()
    {
        _isControlEnabled = true;
        Debug.Log("控制权检测已启用，等待玩家按键");
    }



    private IEnumerator HandoverControlAndSwitchScene()
    {

         _isControlEnabled = false;

        // 阶段1：恢复玩家控制
        RestorePlayerControl();

        // 阶段2：禁用直升机
        DisableHelicopter();

        // 阶段3：延迟切换场景
        yield return new WaitForSeconds(0.4f);
        
        LoadScene("Scene_A");
    }

    private void RestorePlayerControl()
    {
        // 通过 gamemanager 获取玩家对象
        GameObject player = gamemanager.Instance.player;

        if (player != null)
        {
            // 激活玩家
            player.SetActive(true);
            
            // 启用玩家控制器
            if (player.TryGetComponent<PlayerController>(out var pc))
            {
                pc.enabled = true;
                Debug.Log("玩家控制已恢复");
            }

            // 恢复摄像机
            if (_mainCamera != null)
            {
                _mainCamera.transform.SetParent(_originalCameraParent);
                _mainCamera.transform.localPosition = _originalCameraPosition;
                _mainCamera.transform.localRotation = _originalCameraRotation;
                Debug.Log("摄像机已恢复至玩家");
            }
        }
        else
        {
            Debug.LogError("未找到玩家对象！");
        }
    }

    private void DisableHelicopter()
    {
        // 禁用控制器
        enabled = false;
        Debug.Log("直升机控制器已禁用");

        // 冻结物理
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
        }

        // 隐藏模型
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void LoadScene(string sceneName)
    {
        try
        {
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                SceneManager.LoadScene(sceneName);
                Debug.Log($"已切换到场景：{sceneName}");
            }
            else
            {
                Debug.LogError($"场景未找到：{sceneName}");
                // 回退到玩家控制
                RestorePlayerControl();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"场景加载失败：{e.Message}");
            RestorePlayerControl();
        }
    }

 private void HandleInput()
    {
        // 实时检测按键状态
        if (Input.GetKey(KeyCode.W)) _moveInput.y = 10;
        else if (Input.GetKey(KeyCode.S)) _moveInput.y = -10;
        else _moveInput.y = 0;

        if (Input.GetKey(KeyCode.A)) _moveInput.x = -10;
        else if (Input.GetKey(KeyCode.D)) _moveInput.x = 10;
        else _moveInput.x = 0;

        Debug.Log($"输入状态：X={_moveInput.x}, Y={_moveInput.y}");
    }

    private void UpdateGroundState()
    {
        RaycastHit hit;
        float rayLength = 2f; // 检测距离
        bool wasOnGround = _isOnGround;

        // 向下发射射线检测地面
        _isOnGround = Physics.Raycast(transform.position, Vector3.down, out hit, rayLength);

        // 状态变化时记录日志
        if (_isOnGround != wasOnGround)
        {
            Debug.Log($"接地状态变化：{wasOnGround} -> {_isOnGround}");
        }
    }

    private void LiftProcess()
    {
        // 动态计算升力
        float heightFactor = Mathf.Clamp01(1 - transform.position.y / EffectiveHeight);
        float liftForce = EngineForce * heightFactor * HelicopterModel.mass;
        
        // 应用升力
        HelicopterModel.AddForce(Vector3.up * liftForce);
        
        Debug.Log($"升力：{liftForce}，高度因子：{heightFactor}");
    }

    private void MoveProcess()
    {
        // 应用前向力
        Vector3 forwardForce = transform.forward * _moveInput.y * ForwardForce;
        HelicopterModel.AddForce(forwardForce);

        // 应用转向扭矩
        float turnTorque = _moveInput.x * TurnForce * HelicopterModel.mass;
        HelicopterModel.AddTorque(Vector3.up * turnTorque);

        Debug.Log($"前向力：{forwardForce}，转向扭矩：{turnTorque}");
    }

    private void TiltProcess()
    {
        // 倾斜机身
        float tiltX = Mathf.Lerp(0, _moveInput.x * TurnTiltForce, Time.deltaTime);
        float tiltY = Mathf.Lerp(0, _moveInput.y * ForwardTiltForce, Time.deltaTime);
        transform.localRotation = Quaternion.Euler(tiltY, transform.localEulerAngles.y, -tiltX);

        Debug.Log($"倾斜角度：X={tiltX}, Y={tiltY}");
    }

    private void OnKeyPressed(PressedKeyCode[] pressedKeys)
    {
        foreach (var key in pressedKeys)
        {
            switch (key)
            {
                case PressedKeyCode.SpeedUpPressed:
                    EngineForce += EngineForceIncrement;
                    Debug.Log($"引擎功率增加：{EngineForce}");
                    break;
                case PressedKeyCode.SpeedDownPressed:
                    EngineForce -= EngineForceIncrement;
                    EngineForce = Mathf.Max(EngineForce, 0);
                    Debug.Log($"引擎功率减少：{EngineForce}");
                    break;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        _isOnGround = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        _isOnGround = false;
    }

    void OnDrawGizmos()
    {
        // 绘制升力方向
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * EngineForce);

        // 绘制前向力方向
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * _moveInput.y);

        // 绘制转向扭矩
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * _moveInput.x);
    }

    
   public void ApplyBoost()
    {
        EngineForce *= engineBoostMultiplier;
        ForwardForce *= engineBoostMultiplier;
        Debug.Log("直升机性能已提升");
    }

}