using UnityEngine;  
using UnityEngine.SceneManagement;  

public class SceneController : MonoBehaviour  
{  
    private GameObject player; // 用于存储玩家对象  
    private static bool playerExists = false; // 跟踪是否存在玩家  

    private void Awake()  
    {  
        // 检查当前场景的名称  
        string currentScene = SceneManager.GetActiveScene().name;  

        // 尝试查找现有玩家对象  
        player = GameObject.Find("Player"); // 确保这里的 "Player" 是您的玩家对象的确切名称  

        // 如果已经存在玩家且当前场景不是 "3"  
        if (playerExists && currentScene != "3")  
        {  
            Destroy(gameObject); // 销毁当前玩家实例  
        }  
        else  
        {  
            playerExists = true; // 设置玩家存在状态  
            DontDestroyOnLoad(gameObject); // 不销毁玩家  
        }  
    }  

    public void LoadScene(string sceneName)  
    {  
        // 加载新场景  
        SceneManager.LoadScene(sceneName);  
    }  

    public void DestroyPlayer()  
    {  
        if (playerExists)  
        {  
            // 检查当前场景名称  
            string currentScene = SceneManager.GetActiveScene().name;  

            // 仅在当前场景不是“Demo”时销毁玩家  
            if (currentScene != "Demo")  
            {  
                playerExists = false; // 设置为不存在  
                Destroy(player); // 销毁玩家  
            }  
            else  
            {  
                Debug.Log("玩家不能在 'Demo' 场景中被销毁"); // 输出调试信息  
            }  
        }  
    }  
}