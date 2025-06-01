using UnityEngine;  
using UnityEngine.UI; // 使用UI组件  
using UnityEngine.SceneManagement; // 使用场景管理  

public class UIController : MonoBehaviour  
{  
    public GameObject homeCanvas; // 引用HomePage Canvas  
    public GameObject mapSelectionCanvas; // 引用地图选择Canvas  
    public GameObject player; // 引用玩家对象  
    private bool isHomeCanvasActive = true; // Home画布开始时激活  

    void Start()  
    {  
        // 初始状态是显示Home画布  
        homeCanvas.SetActive(isHomeCanvasActive);  
        mapSelectionCanvas.SetActive(false); // 避免地图选择画布初始时显示  
    }  

    void Update()  
    {  
        // 检测ESC键  
        if (Input.GetKeyDown(KeyCode.Escape))  
        {  
            ToggleHomeCanvas();  
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
        SceneManager.LoadScene("DemoScene"); // 确保这是您的地图2的场景名称  
    }  

    public void LoadMap3()  
    {  
        SceneManager.LoadScene("Scene_A"); // 确保这是您的地图3的场景名称  
    }  

    public void LoadMap4()  
    {  
        SceneManager.LoadScene("Map4Scene"); // 确保这是您的地图4的场景名称  
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
}