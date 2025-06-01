using UnityEngine;  

public class ParentAllObjects : MonoBehaviour  
{  
    void Start()  
    {  
        GameObject newParent = new GameObject("DemoParent"); // 创建新的父物体  
        
        // 遍历当前物体下的所有子物体  
        foreach (Transform child in transform)  
        {  
            // 检查物体的标签是否为"Player"，如果是，则跳过  
            if (child.CompareTag("Player"))  
            {  
                continue; // 跳过，不设置为新父物体的子物体  
            }  
            child.SetParent(newParent.transform); // 将其他子物体设置为新父物体的子物体  
        }  
    }  
}