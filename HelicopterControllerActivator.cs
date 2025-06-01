using UnityEngine;

public class HelicopterControlActivator : MonoBehaviour
{
    private HelicopterController _helicopterController;
    private ControllerPanel _controllerPanel;
    public bool canctroller=false;

    void Update()
    {
        // 获取组件引用
        _helicopterController = GetComponent<HelicopterController>();
        _controllerPanel = GetComponent<ControllerPanel>();

        // 初始禁用
        if(!canctroller){ SetControlState(false);
         Debug.Log("直升机控制禁用中");
        
        }else{
            SetControlState(true);
             Debug.Log("直升机控制已激活");
            
        }
       


        // 订阅事件
        MissionManager2.OnHelicopterControlReady += OnControlReady;
    }

    void OnDestroy()
    {
        // 取消订阅
        MissionManager2.OnHelicopterControlReady -= OnControlReady;
    }

    private void OnControlReady()
    {
        // 事件触发时激活控制
       // SetControlState(true);
       canctroller=true;
        Debug.Log("直升机控制已激活");
    }

    private void SetControlState(bool isEnabled)
    {
Debug.Log("成功执行");

        if (_helicopterController != null)
        {
            _helicopterController.enabled = isEnabled;
            Debug.Log("成功禁用");
        }

        if (_controllerPanel != null)
        {
            _controllerPanel.enabled = isEnabled;
        }
    }
}