using UnityEngine;
using UnityEngine.UI;

public class FullscreenImage : MonoBehaviour {
    private RectTransform rectTransform;

    void Start() {
        rectTransform = GetComponent<RectTransform>();
        UpdateFullscreen();
    }

    void UpdateFullscreen() {
        // 设置锚点和边距
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        // 保持宽高比（可选）
        AspectRatioFitter fitter = GetComponent<AspectRatioFitter>();
        if (fitter != null) {
            fitter.aspectRatio = (float)Screen.width / Screen.height;
        }
    }

    // 监听屏幕尺寸变化（如设备旋转）
    void Update() {
        if (Screen.width != rectTransform.rect.width || Screen.height != rectTransform.rect.height) {
            UpdateFullscreen();
        }
    }
}