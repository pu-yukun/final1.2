using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using System;
public class gamemanager : MonoBehaviour
{
    public static gamemanager Instance;

    public GameObject player;  // Player 是跨场景的
    private GameObject _currentHostage; // 当前场景的 hostage
     private GameObject _currentHelicopter; // 新增直升机引用
     public static bool ismonsterready=false;
    private Image _zhanImage; // 私有变量存储 zhan UI 元素
  private MissionManager2 _missionManager2;//任务目标随场景禁用
    private MissionManager3 _missionManager3;




//public static event Action OnSceneALoaded;

    void Awake()
    {


        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // 绑定事件
            Debug.Log("gamemanager 初始化完成，已绑定 sceneLoaded 事件");
        }
        else
        {
            Destroy(gameObject);
        }
    }

void Start(){

     DisableAllMissionManagers();
}

    // 当场景加载完成时调用
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {


      

        Debug.Log($"场景加载完成: {scene.name}");
        if (scene.name == "DemoScene")
        {
 _missionManager2 = player.GetComponent<MissionManager2>();
     _missionManager2.enabled = true;
  FindZhanImage();

            ismonsterready=true;

            Debug.Log("正在查找 hostage");
            _currentHostage = GameObject.FindGameObjectWithTag("hostage");
            if (_currentHostage != null)
            {
                Debug.Log("找到 hostage: " + _currentHostage.name);
            }
            else
            {
                Debug.Log("未找到 hostage");
            }
        }
           else if (scene.name == "Scene_A") // 场景三
        {

 //OnSceneALoaded?.Invoke();

            // 初始化任务3
             _missionManager2 = player.GetComponent<MissionManager2>();
            _missionManager3 = player.GetComponent<MissionManager3>();
            if (_missionManager3 != null)
            {
                _missionManager3.enabled = true;
                Debug.Log("任务3管理器已启用");
            }
            else
            {
                Debug.LogWarning("玩家对象上未找到MissionManager3组件");
            }

            // 确保任务2被禁用
            if (_missionManager2 != null)
            {
                _missionManager2.enabled = false;
                Debug.Log("任务2管理器已禁用");
            }
        }
        else
        {
            _currentHostage = null;
        }

        // 调试输出当前状态
     //   LogMissionStatus();
    }

  private void DisableAllMissionManagers()
    {
        // 禁用任务2
        if (_missionManager2 != null)
        {
            _missionManager2.enabled = false;
        }

        // 禁用任务3
        if (_missionManager3 != null)
        {
            _missionManager3.enabled = false;
        }
    }

/*
   private void LogMissionStatus()
    {
        StringBuilder status = new StringBuilder("当前任务状态：\n");
        status.AppendLine($"任务2: {(_missionManager2 != null && _missionManager2.enabled ? "启用" : "禁用")}");
        status.AppendLine($"任务3: {(_missionManager3 != null && _missionManager3.enabled ? "启用" : "禁用")}");
        Debug.Log(status.ToString());
    }
*/


    // 提供给其他脚本获取当前 hostage
    public GameObject GetCurrentHostage()
    {
        return _currentHostage;
    }


  private void FindZhanImage()
    {
        GameObject zhanObject = GameObject.FindWithTag("zhan"); // 通过标签查找
        if (zhanObject != null)
        {
            _zhanImage = zhanObject.GetComponent<Image>();
            if (_zhanImage != null)
            {
                Debug.Log("找到 zhan UI 元素");
            }
            else
            {
                Debug.Log("找到的 zhan 对象没有 Image 组件！");
            }
        }
        else
        {
            Debug.LogError("未找到标签为 'zhan' 的UI元素！");
        }
    }



  public Image GetZhanImage()
    {
        if (_zhanImage == null)
        {
            Debug.LogWarning("zhan UI 元素为空，尝试重新查找");
            FindZhanImage();
        }
        return _zhanImage;
    }






}