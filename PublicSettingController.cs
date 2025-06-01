using UnityEngine;  
using UnityEngine.UI;  
using System.Collections;  

public class PublicSettingsController : MonoBehaviour  
{  
    [Header("Player Reference")]  
    [SerializeField] private GameObject playerObject;  

    [Header("UI References")]  
    public Slider fovSlider;  
    public Slider sensitivityCoefficientSlider;   
    public Text fovValueText;  
    public Text sensitivityCoefficientText;   
    public Button restoreButton;  
    public Button closeButton;  
public Button shubiao; 
    [Header("FOV Settings")]  
    [SerializeField] private float defaultFOV = 60f;  
    [SerializeField] private float minFOV = 40f;  
    [SerializeField] private float maxFOV = 100f;  

    [Header("Sensitivity Coefficient")]  
    [SerializeField] private float minCoefficient = 0.1f;  
    [SerializeField] private float maxCoefficient = 3.0f;  
    [SerializeField] private float defaultCoefficient = 0.5f;

  public   bool currentCheckset1 ;
       public bool currentCheckset2 ;


    private Camera mainCamera;  
    private MouseLook mouseLook;  
private bool isCursorVisible = true; // 用于跟踪鼠标可见性  


 [Header("Debug Settings")]
    [SerializeField] private float checkInterval = 1f; // 状态检测间隔

    void Awake()  
    {  
        Debug.Log("PublicSettingsController Awake");  
        mainCamera = Camera.main;  
        mouseLook = playerObject.GetComponent<MouseLook>();  
        
        if (mouseLook == null)   
        {  
            Debug.LogError("MouseLook component not found!");  
            return;  
        }  
    }  

    void Start()  
    {  
        Debug.Log("PublicSettingsController Start");  
        StartCoroutine(InitializeUI());  
        StartCoroutine(CheckSettingsState());
    }  

    private IEnumerator InitializeUI()  
    {  
        // 等待一段时间，检查滑动条和按钮的状态  
        yield return new WaitForSeconds(1f); // 等待 1 秒，确保 UI 元素已激活  

        // 检查并初始化 FOV 滑动条  
        if (fovSlider != null)  
        {  
            fovSlider.minValue = minFOV;  
            fovSlider.maxValue = maxFOV;  
            fovSlider.value = defaultFOV; // 设置初始FOV  
            fovValueText.text = defaultFOV.ToString("F0");  
            fovSlider.onValueChanged.AddListener(UpdateFOV);  
        }  
        else  
        {  
            Debug.LogWarning("FOV Slider not found! Setting default FOV to 15.");  
            defaultFOV = 15f; // 使用默认值  
        }  

        // 检查并初始化灵敏度系数滑动条  
        if (sensitivityCoefficientSlider != null)  
        {  
            sensitivityCoefficientSlider.minValue = minCoefficient;  
            sensitivityCoefficientSlider.maxValue = maxCoefficient;  
            sensitivityCoefficientSlider.value = defaultCoefficient; // 设置初始灵敏度  
            sensitivityCoefficientText.text = defaultCoefficient.ToString("F1");  
            sensitivityCoefficientSlider.onValueChanged.AddListener(UpdateSensitivityCoefficient);  
        }  
        else  
        {  
            Debug.LogWarning("Sensitivity Coefficient Slider not found! Setting default coefficient to 1.0.");  
            defaultCoefficient = 1.0f; // 使用默认系数值  
        }  

        // 检查并绑定恢复和关闭按钮  
        if (restoreButton != null)  
        {  
            restoreButton.onClick.AddListener(RestoreDefaults);  
        }  
        else  
        {  
            Debug.LogWarning("Restore Button not found!");  
        }  

        if (closeButton != null)  
        {  
            closeButton.onClick.AddListener(ClosePanel);  
        }  
        else  
        {  
            Debug.LogWarning("Close Button not found!");  
        }  
       //   ToggleCursorVisibility();
    }  

    public void UpdateFOV(float value)  
    {  
        mainCamera.fieldOfView = value;  
        fovValueText.text = value.ToString("F0");  
    }  

    public void UpdateSensitivityCoefficient(float coefficient)  
    {  
        Debug.Log("Executing Update Sensitivity Coefficient");  
        // 计算实际灵敏度 = 基础灵敏度 × 系数  
        float actualSensitivity = MouseLook.BASE_SENSITIVITY * coefficient;  
        
        mouseLook.sensitivityX = actualSensitivity;  
        mouseLook.sensitivityY = actualSensitivity;  
        
        // 更新系数显示  
        sensitivityCoefficientText.text = coefficient.ToString("F1");  
        
        Debug.Log($"Coefficient: {coefficient}, Actual Sensitivity: {actualSensitivity}");  
    }  

  public  void RestoreDefaults()  
    {  
        // 重置FOV  
    
        // 重置灵敏度系数  
        
        sensitivityCoefficientSlider.value = defaultCoefficient;  
        UpdateSensitivityCoefficient(defaultCoefficient);  
        isCursorVisible =true;
        ToggleCursorVisibility();
    }  

 public   void ClosePanel()  
    {  
         //  
         Invoke("closeready",1f);
      
   Debug.Log("开始关闭");
   }  
public void closeready(){

  FindObjectOfType<gamesettings>()?.resetcheck();
          //FindObjectOfType<gamesettings>()?.ClosePanel(); 
 ToggleCursorVisibility(); 

}
/*
public void ToggleCursorVisibility()  
{  
    isCursorVisible = !isCursorVisible; // 切换鼠标可见属性  

    // 根据 isCursorVisible 状态设置鼠标指针的可见性  
    Cursor.visible = isCursorVisible;   
    if (isCursorVisible)  
    {  
        Cursor.lockState = CursorLockMode.None; // 解除锁定  
    }  
    else  
    {  
        Cursor.lockState = CursorLockMode.Locked; // 锁定光标  
    }  

    Debug.Log($"Cursor visibility toggled. Current state: {isCursorVisible}");  
}  
*/


/*
   void OnDestroy()
    {
       
            StopCoroutine(checkCoroutine);
        
    }
*/
IEnumerator CheckSettingsState()  
{  
    // 初始化上一次记录的状态
    bool lastCheck1 = currentCheckset1;
    bool lastCheck2 = currentCheckset2;

    while (true)  
    {  
        gamesettings settings = FindObjectOfType<gamesettings>();  
        if (settings == null)  
        {  
            Debug.LogWarning("Gamesettings instance not found!");  
            yield return new WaitForSeconds(checkInterval);  
            continue;
        }  

        // 获取最新状态（不立即覆盖当前值）
        bool newCheck1 = settings.checkset1;  
        bool newCheck2 = settings.checkset2;  

        // 仅当状态变化时才更新
        if (newCheck1 != lastCheck1 || newCheck2 != lastCheck2)  
        {  
            Debug.Log($"状态变化: 旧状态({lastCheck1},{lastCheck2}) → 新状态({newCheck1},{newCheck2})");
            
            // 更新当前状态
            currentCheckset1 = newCheck1;
            currentCheckset2 = newCheck2;
            lastCheck1 = newCheck1;
            lastCheck2 = newCheck2;
            
            ToggleCursorVisibility();
        }  

        yield return new WaitForSeconds(checkInterval);  
    }  
}

  public  void ToggleCursorVisibility()
    {
        // 核心逻辑：当两个设置面板都关闭时操作光标
        if (!currentCheckset1 && !currentCheckset2)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("游戏设置关闭，隐藏鼠标光标");
            
            // 恢复游戏状态
            Time.timeScale = 1f;
          //  healhbar.SetActive(true);
          //  moshi.SetActive(true);
        }
        else
        {
            // 确保至少一个面板打开时的状态
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("设置面板打开，显示鼠标光标");
        }
    }






}