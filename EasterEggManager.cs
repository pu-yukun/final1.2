using UnityEngine;
using System.Collections;
using System.IO;

[System.Serializable]
public class SaveData
{
    public bool isCaidanActive;
    public bool isManagerActive;
}

public class EasterEggManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject caidanUI;
    public GameObject managerUI;

    [Header("Player Reference")]
    public PlayerHealth playerHealth;
    public string triggerTag = "caidan";

    [Header("Settings")]
    public float checkInterval = 1f;
    private string savePath;
    private SaveData gameData;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "easteregg.sav");
        LoadData();
        UpdateUIState();
        
        if (gameData.isCaidanActive)
        {
            StartCoroutine(ManagerCheckRoutine());
            Debug.Log("彩蛋已激活，启动管理员检测");
        }
          if(caidanUI != null) 
        caidanUI.SetActive(false);
    
    }
public void ReloadGameData()
{
    LoadData();
    ApplyManagerPrivileges(); // 同步权限状态到PlayerHealth
}
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            ActivateCaidan();
        }
    }

    void ActivateCaidan()
    {
        if (gameData.isCaidanActive) return;

        gameData.isCaidanActive = true;
        SaveData();
        UpdateUIState();
        
        StartCoroutine(ManagerCheckRoutine());
        Debug.Log("彩蛋激活！开始检测管理员权限");

 if(caidanUI != null)
        caidanUI.SetActive(true);


    }

    IEnumerator ManagerCheckRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            if (Input.GetKeyDown(KeyCode.KeypadMultiply))
            {
                GrantManagerPrivileges();
                yield break;
            }
        }
    }

    void GrantManagerPrivileges()
    {
        if (playerHealth != null)
        {
            playerHealth.isManager = true;
            playerHealth.maxHealth = 999999;
            playerHealth.currentHealth = playerHealth.maxHealth;
            
            gameData.isManagerActive = true;
            SaveData();
            UpdateUIState();
            
            Debug.Log("管理员权限已授予");
        }
    }

    void UpdateUIState()
    {
        if (caidanUI != null)
            caidanUI.SetActive(gameData.isCaidanActive);
        
        if (managerUI != null)
            managerUI.SetActive(gameData.isManagerActive);
    }
private void ApplyManagerPrivileges()
{
    if (playerHealth != null && gameData.isManagerActive)
    {
        playerHealth.isManager = true;
        playerHealth.maxHealth = 999999;
        playerHealth.currentHealth = playerHealth.maxHealth;
    }
}

// 修改存档加载逻辑
void LoadData()
{
    if (File.Exists(savePath))
    {
        gameData = JsonUtility.FromJson<SaveData>(File.ReadAllText(savePath));
        Debug.Log($"加载存档成功：{JsonUtility.ToJson(gameData)}");
    }
    else
    {
        gameData = new SaveData();
        Debug.Log("无存档文件，创建新存档");
    }
    
    ApplyManagerPrivileges(); // 加载后立即应用权限
}

// 修改存档保存逻辑
void SaveData()
{
    try
    {
        File.WriteAllText(savePath, JsonUtility.ToJson(gameData));
        Debug.Log($"存档成功：{JsonUtility.ToJson(gameData)}");
    }
    catch (System.Exception e)
    {
        Debug.LogError($"存档失败：{e.Message}");
    }
}
    void OnApplicationQuit()
    {
        SaveData();
    }
}