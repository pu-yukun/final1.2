using UnityEngine;  
using SunTemple;  

public class SceneManagerController : MonoBehaviour  
{  
    private Camera playerCamera; // 玩家专用摄像机  
    private Camera mapCamera;  

    // 公共变量，允许在Inspector中调节每个地图上玩家的位置  
    public Vector3 playerSpawnPosition;  

    private void Start()  
    {  
        // 查找场景中的玩家对象（确保玩家有一个唯一的标识，比如 Tag）  
        GameObject player = GameObject.FindGameObjectWithTag("Player");  

        playerCamera = player.GetComponentInChildren<Camera>(); // 假设玩家摄像机是玩家子物体上的摄像机  

        // 查找地图摄像机  
        mapCamera = GameObject.FindGameObjectWithTag("mapCamera")?.GetComponent<Camera>(); // 确保设置了对应的 tag  

        if (mapCamera != null)  
        {  
            mapCamera.enabled = false; // 禁用地图摄像机  
        }  

        // 确保玩家摄像机启用  
        if (playerCamera != null)  
        {  
            playerCamera.enabled = true; // 启用玩家摄像机  
        }  

        if (player != null)  
        {  
            // 使用公共变量设置玩家位置  
            player.transform.position = playerSpawnPosition;  

            // 检查玩家是否进入了暴露区域  
            Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, 1f); // 1f是检查范围  

            foreach (var hitCollider in hitColliders)  
            {  
                ExposureArea exposureArea = hitCollider.GetComponent<ExposureArea>();  
                if (exposureArea != null)  
                {  
                    // 如果检测到暴露区，调用 EnterTrigger 方法  
                    exposureArea.EnterTrigger(player.GetComponent<Collider>());  
                }  
            }  
 Debug.Log("Player found."); 

        }  
        else  
        {  
            Debug.LogError("Player object not found in the scene!");  
        }  
    }  
public Vector3 GetPlayerSpawnPosition()  
{  
    return playerSpawnPosition;  
}


}