using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EndGame : MonoBehaviour
{
    [Header("滚动设置")]
    public float scrollSpeed = 100f; // 文本滚动速度（单位：像素/秒）
    public float targetY = 500f;    // 目标 Y 轴位置

    private RectTransform textRectTransform; // 文本的 RectTransform
    private bool isScrolling = true;         // 是否正在滚动

    void Start()
    {
        // 获取 Text 组件的 RectTransform
        textRectTransform = GetComponent<RectTransform>();
        if (textRectTransform == null)
        {
            Debug.LogError("未找到 RectTransform 组件！");
            enabled = false; // 禁用脚本
        }
    }

    void Update()
    {
        if (isScrolling)
        {
            // 计算新的 Y 轴位置
            float newY = textRectTransform.anchoredPosition.y + scrollSpeed * Time.deltaTime;

            // 更新文本位置
            textRectTransform.anchoredPosition = new Vector2(
                textRectTransform.anchoredPosition.x,
                newY
            );

            // 检查是否到达目标位置
            if (newY >= targetY)
            {
                isScrolling = false; // 停止滚动
                Debug.Log("文本已到达目标位置，退出游戏 Demo");

                // 退出游戏
                ExitGame();
            }
        }
    }

    void ExitGame()
    {
#if UNITY_EDITOR
        // 在编辑器中停止播放
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 在发布版本中退出应用
        Application.Quit();
#endif
    }
}
