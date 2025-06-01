using System.Collections;  
using System.Collections.Generic;  
using UnityEngine;  
using UnityEngine.UI;  

public class gameStart : MonoBehaviour  
{  
    public GameObject uiCanvas; // 拖拽你的UI画布到这个变量  
    public Button startButton; // 拖拽你的按钮到这个变量  

    void Start()  
    {  
        if (uiCanvas == null)  
        {  
            Debug.LogError("uiCanvas is not assigned!");  
        }  
        
        if (startButton == null)  
        {  
            Debug.LogError("startButton is not assigned!");  
        }  
        
        // 为按钮添加点击事件  
        startButton.onClick.AddListener(OnStartButtonClick);  
    }  

    void OnStartButtonClick()  
    {  
        // 隐藏UI画布  
        if (uiCanvas.activeSelf) // 检查画布是否可见  
        {  
            uiCanvas.SetActive(false);  
            Debug.Log("UI Canvas has been hidden."); // 调试信息  
        }  
        else  
        {  
            Debug.Log("UI Canvas is already hidden."); // 调试信息  
        }  
    }  
}