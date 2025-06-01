using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMedic : MonoBehaviour
{public int increasehealth;
    public float interactionDistance = 8f;
    public Text interactPromptText; // 拖拽你的UI Text到这里

    private Transform player;
    private PlayerHealth playerHealth;
    private bool isInRange;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        if (interactPromptText != null) interactPromptText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        isInRange = distance <= interactionDistance;

        // 控制提示显示
        if (interactPromptText != null)
            interactPromptText.gameObject.SetActive(isInRange);

        // 检测交互输入
        if ( isInRange && (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)))
        {
            playerHealth.IncreaseMaxHealth(increasehealth);
            Destroy(gameObject); // 使用后销毁药瓶
        }
    }

    // 可选：使用触发器代替距离检测（更高效）
    /*
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            if (interactPromptText != null) interactPromptText.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            if (interactPromptText != null) interactPromptText.gameObject.SetActive(false);
        }
    }
    */
}