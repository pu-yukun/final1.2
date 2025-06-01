using UnityEngine;  

public class camerafollowapk : MonoBehaviour  
{  
    public enum RotationAxes { MouseXAndY, MouseX, MouseY }  
    public RotationAxes axes = RotationAxes.MouseXAndY;  

    public float sensitivityX = 300F; // 增加灵敏度  
    public float sensitivityY = 300F; // 增加灵敏度  
    public float minimumY = -300F;  
    public float maximumY = 300F;  

    public RectTransform joystickBackground; // 用于表示摇杆背景的 RectTransform  
    public float joystickRadius = 100f; // 摇杆的有效半径  
    private bool isRotating = false; // 标记是否正在进行视角旋转  
    private float rotationY = 0f;  

    private void Update()  
    {  
        HandleTouchInput();  
    }  

    private void HandleTouchInput()  
    {  
        if (Input.touchCount > 0)  
        {  
            Touch touch = Input.GetTouch(0);  
            bool isTouchingJoystick = IsTouchInJoystickArea(touch.position);  

            // Debug 輸出触摸位置和摇杆区域  
            Debug.Log($"Touch Position: {touch.position}, Is Touching Joystick: {isTouchingJoystick}");  

            // 在触摸开始时检测摇杆区域  
            if (touch.phase == TouchPhase.Began)  
            {  
                if (isTouchingJoystick)  
                {  
                    Debug.Log("Touch started within joystick area.");  
                }  
                else  
                {  
                    isRotating = true; // 如果不在摇杆区域，允许旋转  
                    Debug.Log("Touch started outside joystick area, rotation enabled.");  
                }  
            }  

            // 只有在允许旋转时才处理移动  
            if (isRotating && touch.phase == TouchPhase.Moved)  
            {  
                RotateCamera(touch);  
            }  

            // 当触摸结束时，重置旋转状态  
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)  
            {  
                isRotating = false;  
                Debug.Log("Touch ended, rotation disabled.");  
            }  
        }  
    }  

    private void RotateCamera(Touch touch)  
    {  
        float rotationX = transform.localEulerAngles.y + touch.deltaPosition.x * sensitivityX * Time.deltaTime*8;  
        rotationY -= touch.deltaPosition.y * sensitivityY * Time.deltaTime*8;  
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);  

        // 列出旋转限制的调试信息  
        Debug.Log($"Rotation X: {rotationX}, Rotation Y: {rotationY}");  

        // 更新摄像机的旋转  
        if (axes == RotationAxes.MouseXAndY)  
        {  
            transform.localEulerAngles = new Vector3(rotationY, rotationX, 0);  
        }  
        else if (axes == RotationAxes.MouseX)  
        {  
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationX, 0);  
        }  
        else if (axes == RotationAxes.MouseY)  
        {  
            transform.localEulerAngles = new Vector3(rotationY, transform.localEulerAngles.y, 0);  
        }  
    }  

    private bool IsTouchInJoystickArea(Vector2 touchPosition)  
    {  
        Vector2 joystickCenter = joystickBackground.anchoredPosition; // 摇杆的中心位置  
        float distance = Vector2.Distance(touchPosition, joystickCenter); // 计算触摸位置与摇杆中心的距离  
        // Debug 输出划定的区域  
        Debug.Log($"Joystick Center: {joystickCenter}, Distance: {distance}");  
        return distance < joystickRadius; // 判断触摸是否在摇杆范围内  
    }  
}