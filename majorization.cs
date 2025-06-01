using UnityEngine;  

public class majorization : MonoBehaviour  
{  
    void Start()  
    {  
        // 将所有子物体设置为静态  
        SetStaticRecursively(transform, true);  
    }  

    private void SetStaticRecursively(Transform parent, bool isStatic)  
    {  
        // 将当前物体的静态状态设置为所需状态  
        GameObject go = parent.gameObject;  
        go.isStatic = isStatic;  

        // 遍历并递归所有子物体  
        foreach (Transform child in parent)  
        {  
            SetStaticRecursively(child, isStatic);  
        }  
    }  
}