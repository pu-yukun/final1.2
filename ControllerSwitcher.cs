using UnityEngine;

public class ControlSwitcher : MonoBehaviour
{
    [Header("Settings")]
    public float maxSwitchDistance = 3f;
    public Vector3 cameraOffset = new Vector3(0, 2, -5);

    private GameObject _player;
    private Camera _mainCamera;
    private bool _isReady;
  public AudioSource HelicopterSound;
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _mainCamera = Camera.main;
        
        // 订阅事件
        MissionManager2.OnHelicopterControlReady += EnableSwitch;
        Debug.Log("已订阅控制权切换事件");
    }

    void OnDestroy()
    {
        MissionManager2.OnHelicopterControlReady -= EnableSwitch;
    }

    void Update()
    {
        if(_isReady && Input.GetKeyDown(KeyCode.L))
        {
            TrySwitchControl();


        }
    }

    private void EnableSwitch()
    {
        _isReady = true;
        Debug.Log("已收到控制权切换准备信号");
    }

    private void TrySwitchControl()
    {
        GameObject helicopter = GameObject.FindWithTag("man");
        
        if(helicopter == null)
        {
            Debug.LogError("未找到直升机对象！请检查：\n" +
                          "- Tag是否设置为'man'\n" +
                          "- 是否在DemoScene场景");
            return;
        }

        // 动态距离检测
        float distance = Vector3.Distance(
            _player.transform.position,
            helicopter.transform.position
        );

        if(distance > maxSwitchDistance)
        {
            Debug.Log($"切换失败！当前距离：{distance}，要求距离：{maxSwitchDistance}");
            return;
        }

        PerformControlSwitch(helicopter);
    }

    private void PerformControlSwitch(GameObject helicopter)
    {
        // 禁用玩家控制
        if(_player.TryGetComponent<PlayerController>(out var pc))
        {
            pc.enabled = false;
            Debug.Log("玩家控制已禁用");
        }

        // 转移摄像头
        if(_mainCamera != null)
        {
            _mainCamera.transform.SetParent(helicopter.transform);
            _mainCamera.transform.localPosition = cameraOffset;
            _mainCamera.transform.localRotation = Quaternion.Euler(20, 0, 0);
            Debug.Log("摄像机已转移至直升机");
        }

        // 激活直升机控制
        if(helicopter.TryGetComponent<HelicopterController>(out var hc))
        {
            hc.enabled = true;
            Debug.Log("直升机控制器已激活");
        }
        else
        {
            Debug.LogError("直升机缺少控制器组件！");
        }

        // 隐藏玩家
        _player.SetActive(false);
        Debug.Log("玩家模型已隐藏");
    }
}