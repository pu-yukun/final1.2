using UnityEngine;  

public class LightManager : MonoBehaviour  
{  
    public string playerName = "Player"; // 假设玩家对象的名称是 "Player"  
    public float detectionRadius = 100f;  
    private Light[] allLights;  
    private Transform player;  

    void Start()  
    {  
        // 通过名称查找玩家对象  
        player = GameObject.Find(playerName)?.transform;  
        if (player == null)  
        {  
            Debug.LogError("未找到玩家对象，请检查名称是否正确。");  
            return;  
        }  

        allLights = FindObjectsOfType<Light>();  
    }  

    void Update()  
    {  
        if (player == null) return; // 如果未找到玩家则跳过  

        foreach (Light light in allLights)  
        {  
            if (Vector3.Distance(player.position, light.transform.position) > detectionRadius)  
            {  
                light.enabled = false; // 禁用远离玩家的光源  
            }  
            else  
            {  
                light.enabled = true; // 启用在范围内的光源  
            }  
        }  
    }  
}