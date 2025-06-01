using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIchange : MonoBehaviour
{
    public GameObject homeCanvas; // 引用HomePage Canvas
    public GameObject mapSelectionCanvas; // 引用地图选择Canvas
    public GameObject player; // 引用玩家对象
    public GameObject loadingScreen; // 加载动画的UI面板
    public Slider progressBar; // 进度条
    public Image loadingImage; // 加载图片（可选）

    private bool isHomeCanvasActive = true; // Home画布开始时激活
    private AsyncOperation currentOperation; // 当前异步加载操作
    private bool isLoading = false; // 是否正在加载

    void Start()
    {
        // 初始状态是显示Home画布
        homeCanvas.SetActive(isHomeCanvasActive);
        mapSelectionCanvas.SetActive(false); // 避免地图选择画布初始时显示
        loadingScreen.SetActive(false); // 初始时隐藏加载动画
    }

    void Update()
    {
        // 检测ESC键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleHomeCanvas();
        }

        // 如果正在加载，更新进度条
        if (isLoading && currentOperation != null)
        {
            float progress = Mathf.Clamp01(currentOperation.progress / 0.9f); // operation.progress 最大值为 0.9
            progressBar.value = progress;

            // 如果加载完成，允许跳转
            if (currentOperation.progress >= 0.9f)
            {
                currentOperation.allowSceneActivation = true;
                isLoading = false;
                loadingScreen.SetActive(false); // 隐藏加载动画
            }
        }
    }

    public void Begin()
    {
        // 点击按钮后隐藏Home画布并启用玩家
        homeCanvas.SetActive(false);
        EnablePlayer();
    }

    public void ShowMapSelection()
    {
        // 切换到地图选择画布并禁用Home画布
        homeCanvas.SetActive(false);
        mapSelectionCanvas.SetActive(true);
    }

    // 按钮功能：加载不同的地图
    public void LoadMap2()
    {
        LoadSceneWithAnimation("DemoScene");
    }

    public void LoadMap3()
    {
        LoadSceneWithAnimation("Scene_A");
    }

    public void LoadMap4()
    {
        LoadSceneWithAnimation("Demo_Night_City");
    }

    private void ToggleHomeCanvas()
    {
        isHomeCanvasActive = !isHomeCanvasActive;
        homeCanvas.SetActive(isHomeCanvasActive);

        // 启用或禁用玩家
        if (isHomeCanvasActive)
        {
            DisablePlayer(); // 禁用玩家控制
        }
        else
        {
            EnablePlayer(); // 启用玩家控制
        }
    }

    private void DisablePlayer()
    {
        if (player)
        {
            // 如果玩家对象有控制器组件，禁用它
            player.GetComponent<PlayerController>().enabled = false;
        }
    }

    private void EnablePlayer()
    {
        if (player)
        {
            // 重新启用玩家控制
            player.GetComponent<PlayerController>().enabled = true;
        }
    }

    // 加载场景并显示加载动画
    public void LoadSceneWithAnimation(string sceneName)
    {
        
        // 显示加载动画
        loadingScreen.SetActive(true);
   //mapSelectionCanvas.SetActive(false);
        progressBar.value = 0; // 重置进度条

        // 异步加载场景
        currentOperation = SceneManager.LoadSceneAsync(sceneName);
        currentOperation.allowSceneActivation = false; // 禁止自动跳转
        isLoading = true; // 标记为正在加载
    }
 
}