using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(MissionManager2), typeof(MissionManager3))]
public class PlayerMissionController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject hostageAppearCanvas;  // 拖拽赋值
    public GameObject mission2Canvas;      // 拖拽赋值
    public GameObject mission3Canvas;      // 拖拽赋值

    [Header("Debug Settings")]
    [SerializeField] private bool _showDebugLogs = true;

    private MissionManager2 _mission2;
    private MissionManager3 _mission3;

    void Awake()
    {
        // 初始化组件
        _mission2 = GetComponent<MissionManager2>();
        _mission3 = GetComponent<MissionManager3>();
        
        // 初始禁用所有任务
        SetMissionState(false, false);
        
        // 绑定场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        DebugLog("控制器初始化完成");
    }

private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Scene_A")
        {
            // 添加双重保障
            StartCoroutine(DelayedSceneCheck());
        }
    }

    private IEnumerator DelayedSceneCheck()
    {
        yield return new WaitForSeconds(0.5f);
        
        // 如果触发器未生效则强制执行
        if (mission3Canvas != null && mission3Canvas.activeSelf == false)
        {
            SetUIState(false, false, true);
            SetMissionState(false, true);
            Debug.LogWarning("<color=orange>[Fallback]</color> 触发器未生效，强制调整UI");
        }
    }
    private void SetUIState(bool hostageActive, bool mission2Active, bool mission3Active)
    {
        if (hostageAppearCanvas != null)
        {
            hostageAppearCanvas.SetActive(hostageActive);
            DebugLog($"HostageAppearCanvas: {hostageActive}");
        }

        if (mission2Canvas != null)
        {
            mission2Canvas.SetActive(mission2Active);
            DebugLog($"Mission2Canvas: {mission2Active}");
        }

        if (mission3Canvas != null)
        {
            mission3Canvas.SetActive(mission3Active);
            DebugLog($"Mission3Canvas: {mission3Active}");
        }
    }

    public void SetMissionState(bool mission2Active, bool mission3Active)
    {
        if (_mission2 != null)
        {
            _mission2.enabled = mission2Active;
            DebugLog($"Mission2: {mission2Active}");
        }

        if (_mission3 != null)
        {
            _mission3.enabled = mission3Active;
            DebugLog($"Mission3: {mission3Active}");
        }
    }

    private void DebugLog(string message)
    {
        if (_showDebugLogs) Debug.Log($"<color=cyan>[MissionController]</color> {message}");
    }

    void OnDestroy()
    {
        // 清理事件绑定
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}