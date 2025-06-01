using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helicoptercameracontroller : MonoBehaviour
{
  [Header("Follow Settings")]
    public Transform target;
    public Vector3 positionOffset = new Vector3(0, 2, -5);
    public float followSmoothness = 5f;
    public float rotationSmoothness = 3f;

    [Header("Look Settings")]
    public float mouseSensitivity = 100f;
    public float maxVerticalAngle = 80f;
    public float minVerticalAngle = -30f;

    private float _currentYRotation;
    private float _currentXRotation;
    private Vector3 _targetPosition;

    void LateUpdate()
    {
        if (!target) return;

        // 鼠标控制视角
        HandleMouseInput();

        // 计算目标位置和旋转
        UpdateCameraTransform();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            _currentYRotation += mouseX;
            _currentXRotation -= mouseY;
            _currentXRotation = Mathf.Clamp(_currentXRotation, minVerticalAngle, maxVerticalAngle);
        }
    }

    private void UpdateCameraTransform()
    {
        // 计算目标旋转
        Quaternion targetRotation = Quaternion.Euler(_currentXRotation, _currentYRotation, 0);

        // 计算目标位置（基于直升机旋转）
        _targetPosition = target.position + 
                         target.TransformDirection(positionOffset);

        // 平滑移动和旋转
        transform.position = Vector3.Lerp(
            transform.position, 
            _targetPosition, 
            followSmoothness * Time.deltaTime
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            targetRotation, 
            rotationSmoothness * Time.deltaTime
        );
    }

    // 用于重置视角（可选）
    public void ResetView()
    {
        _currentYRotation = target.eulerAngles.y;
        _currentXRotation = 25f;
    }
}
