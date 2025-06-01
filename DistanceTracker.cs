using UnityEngine;
using UnityEngine.UI;

public class DistanceTracker : MonoBehaviour
{
    [Header("UI Components")]
    public Text distanceText; // UI 文本
    public Image targetMarker; // UI 倒三角形图像

    [Header("Tracking Settings")]
    public float maxAngle = 45.0f; // 最大仰角
    public float minHeight = 0.5f; // 最小高度防止被遮挡
    public float minScale = 0.01f; // 最小缩放因子
    public float maxScale = 1.15f; // 最大缩放因子
    public float maxDistance = 100.0f; // 最大距离

    [Header("Debug")]
    public bool showDebugLogs = true; // 是否显示调试日志

    private GameObject player; // 玩家的游戏对象
    private GameObject target; // 目标的游戏对象
    private string currentTargetTag = "track"; // 当前跟踪目标的 Tag


   public string targetTag;
    void Start()
    {
        // 初始化时隐藏 UI 元素
        if (distanceText != null) distanceText.gameObject.SetActive(false);
        if (targetMarker != null) targetMarker.gameObject.SetActive(false);

        DebugLog("DistanceTracker 初始化完成");
    }

    void Update()
    {
        // 动态查找玩家和目标
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");
        if (target == null) target = GameObject.FindGameObjectWithTag(currentTargetTag);

        // 按下 Tab 键时切换距离显示
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleUI();
        }

        // 如果 UI 是激活的，跟踪距离
        if (distanceText != null && distanceText.gameObject.activeSelf && player != null && target != null)
        {
            UpdateDistance();
            PositionTargetMarker();
        }
    }

    /// <summary>
    /// 切换 UI 显示状态
    /// </summary>
    private void ToggleUI()
    {
        if (distanceText != null && targetMarker != null)
        {
            bool isActive = !distanceText.gameObject.activeSelf;
            distanceText.gameObject.SetActive(isActive);
            targetMarker.gameObject.SetActive(isActive);
            DebugLog($"UI 显示状态切换为: {isActive}");
        }
    }

    /// <summary>
    /// 更新目标标记的位置
    /// </summary>
    private void PositionTargetMarker()
    {
        Vector3 targetPosition = target.transform.position;
        Vector3 playerPosition = player.transform.position;

        // 计算水平距离
        float horizontalDistance = Vector3.Distance(
            new Vector3(playerPosition.x, 0, playerPosition.z),
            new Vector3(targetPosition.x, 0, targetPosition.z)
        );

        // 计算目标高度
        float requiredHeight = Mathf.Tan(maxAngle * Mathf.Deg2Rad) * horizontalDistance;
        float adjustedHeight = Mathf.Max(requiredHeight, minHeight);

        // 更新目标标记位置
        targetMarker.transform.position = targetPosition + new Vector3(0, adjustedHeight, 0);

        // 更新图标缩放
        AdjustMarkerScale(horizontalDistance);

        // 旋转目标标记朝向玩家
        Vector3 directionToPlayer = playerPosition - targetPosition;
        targetMarker.transform.rotation = Quaternion.LookRotation(directionToPlayer);
    }

    /// <summary>
    /// 调整目标标记的缩放
    /// </summary>
    private void AdjustMarkerScale(float horizontalDistance)
    {
        float scaleFactor = Mathf.Lerp(minScale, maxScale, horizontalDistance / maxDistance);
        scaleFactor = Mathf.Clamp(scaleFactor, minScale, maxScale);

        DebugLog($"水平距离: {horizontalDistance}, 缩放因子: {scaleFactor}");

        targetMarker.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }

    /// <summary>
    /// 更新距离显示
    /// </summary>
    private void UpdateDistance()
    {
        float distance = Vector3.Distance(player.transform.position, target.transform.position);
        distanceText.text = $"距离目标: {distance.ToString("F2")} 米";
    }

    /// <summary>
    /// 切换跟踪目标
    /// </summary>
    public void ChangeTargetTag(string newTag)
    {
        if (string.IsNullOrEmpty(newTag))
        {
            DebugLogError("新目标 Tag 不能为空");
            return;
        }

        currentTargetTag = newTag;
        target = GameObject.FindGameObjectWithTag(newTag);

        if (target == null)
        {
            DebugLogError($"未找到 Tag 为 {newTag} 的目标");
        }
        else
        {
            DebugLog($"跟踪目标已切换至: {newTag} ({target.name})");
        }
    }

    /// <summary>
    /// 输出调试日志
    /// </summary>
    private void DebugLog(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[DistanceTracker] {message}");
        }
    }

    /// <summary>
    /// 输出调试错误日志
    /// </summary>
    private void DebugLogError(string message)
    {
        if (showDebugLogs)
        {
            Debug.LogError($"[DistanceTracker] {message}");
        }
    }
    /*
public void SetTargetTag(string newTag)
    {
        targetTag = newTag;
        FindTarget();
    }
*/
}