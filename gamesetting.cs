using UnityEngine;
using UnityEngine.UI;

public class gamesettings : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject publicSettingsPanel;
    [SerializeField] private Button publicButton;
    [SerializeField] private Button exitButton;
    public GameObject healhbar;
    public GameObject moshi;

    private bool isPaused = false;



public bool checkset1 = false; // 标记是否打开公共设置  鼠标光标bool设置！
    public bool checkset2 = false; // 标记是否打开主设置  

    void Start()
    {
        settingsPanel.SetActive(false);
        publicSettingsPanel.SetActive(false);
        
        // 绑定按钮事件
        publicButton.onClick.AddListener(() => OpenPanel(publicSettingsPanel));
        exitButton.onClick.AddListener(QuitGame);
    }

    void Update()
    {    Debug.Log($"公共设置: {checkset1}\n主设置: {checkset2}");
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMainSettings();
        }
    }

    // 通用面板打开方法
public void resetcheck(){
CloseAllPanels();

    checkset1=false;
    checkset2=false;
  //   settingsPanel.SetActive(false);
   //     publicSettingsPanel.SetActive(false);

}

    public void OpenPanel(GameObject targetPanel)
    {
   checkset1 = (targetPanel == publicSettingsPanel);
        checkset2 = (targetPanel == settingsPanel);


        isPaused = true;
        Time.timeScale = 0;
        
        // 关闭所有面板
        settingsPanel.SetActive(false);
        publicSettingsPanel.SetActive(false);
        
        // 打开目标面板
        targetPanel.SetActive(true);
        UpdateCursorState();


  // 检查打开的面板  
  /*
        if (targetPanel == publicSettingsPanel)  
        {  
            checkset1 = true; //打开公共设置  
        }  
        else  
        {  
            checkset1 = false; // 关闭公共设置  
        }  
*/



    }
  public void ClosePanel(GameObject panel)
    {
        if (panel == publicSettingsPanel)
        {
            checkset1 = false;
            publicSettingsPanel.SetActive(false);
        }
        else if (panel == settingsPanel)
        {
            checkset2 = false;
            settingsPanel.SetActive(false);
        }

        // 检查是否所有面板关闭
        if (!checkset1 && !checkset2)
        {
            isPaused = false;
            Time.timeScale = 1;
        }
        
        UpdateCursorState();
    }

    // 专用操作方法
    
    public void operateset()
    {
       // Invoke("operatestart",1f);
        OpenPanel(publicSettingsPanel);

    }
    /*
public operatesetstart(){
    OpenPanel(publicSettingsPanel);
    }
*/
    // 主设置面板切换
    public void ToggleMainSettings()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        settingsPanel.SetActive(isPaused);
         checkset2 = isPaused;

        publicSettingsPanel.SetActive(false);
        UpdateCursorState();
    }

    public void CloseAllPanels()
    {


        isPaused = false;
        Time.timeScale = 1;
        settingsPanel.SetActive(false);
        publicSettingsPanel.SetActive(false);
        UpdateCursorState();
        checkset1 = false;
        checkset2 = false;
          Debug.Log($"gg设置: {checkset1}\n主gg设置: {checkset2}");
    }

public void returnshubiao()
    {
        // 正确切换至公共设置
        OpenPanel(publicSettingsPanel);
    }
    void UpdateCursorState()
    {
        healhbar.SetActive(!isPaused);
        moshi.SetActive(!isPaused);

        
       // Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        //Cursor.visible = isPaused;

  // 直接基于面板激活状态判断（避免依赖变量）
    bool anyPanelActive = settingsPanel.activeSelf || publicSettingsPanel.activeSelf;
    
    Cursor.lockState = anyPanelActive ? CursorLockMode.None : CursorLockMode.Locked;
    Cursor.visible = anyPanelActive;
    
    Debug.Log($"光标状态更新 | 面板1激活: {settingsPanel.activeSelf} | 面板2激活: {publicSettingsPanel.activeSelf}");

    }

    void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}