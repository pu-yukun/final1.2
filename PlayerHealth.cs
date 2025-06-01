using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    private bool isWaitingForRespawn = false;

    [Header("基本设置")]
    public bool isManager = false; // 是否为管理员
    public int maxHealth = 100; // 最大血量
    public int currentHealth; // 当前血量
    public GameObject inventory; // 物品管理
    public GameObject gameOverUI; // 游戏结束UI
    public Button restartButton; // 重新开始按钮
    public Image healthBarFill; // 血量填充 Image
    public Text healthText; // 显示血量的 Text
    public GameObject inputFieldUI; // 输入框 UI
    public InputField inputField; // 输入框
    [Tooltip("每秒扣血的量")]
    public float killBlood = 50f; // 每秒扣血量

    [Header("回血设置")]
    public float healDelay = 5f; // 触发回血的延迟时间
    public float healInterval = 3f; // 每次回血的间隔时间
    public int healAmount = 6; // 每次回血量

    [Header("调试信息")]
    public Text debugText; // 用于显示调试信息的UI Text

    private Vector3 respawnPosition;
    private PlayerController playerController;
    private bool isrebirth = false;
    private float lastDamageTime;
    private int lastRecordedHealth;
    private bool isHealing = false;
    private float healStartTime;

    private void Awake()
    {
        SceneManagerController sceneManager = FindObjectOfType<SceneManagerController>();
        if (sceneManager != null)
        {
            respawnPosition = sceneManager.playerSpawnPosition;
        }
        else
        {
            Debug.LogError("SceneManagerController not found!");
        }
    }

    private void UpdateRespawnPosition()
    {
        SceneManagerController sceneManager = FindObjectOfType<SceneManagerController>();
        if (sceneManager != null)
        {
            respawnPosition = sceneManager.playerSpawnPosition;
        }
    }

    void Start()
    {
        UpdateRespawnPosition();
        playerController = GetComponent<PlayerController>();
        maxHealth = isManager ? 999999 : 100;
        currentHealth = maxHealth;

        gameOverUI.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
        UpdateHealthBar();
        UpdateHealthText();
        inputFieldUI.SetActive(false);

  EasterEggManager eggManager = FindObjectOfType<EasterEggManager>();
    if (eggManager != null)
    {
        eggManager.ReloadGameData();
    }



    }

    public void TakeDamage(int damage)
    {
        if (isManager) return;

        Debug.Log($"受到伤害 {damage}，当前血量：{currentHealth}");
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();
        UpdateHealthText();

        if (currentHealth <= 0)
        {
            Debug.Log("玩家死亡，取消所有回血调用");
            CancelInvoke("CheckAndStartHealing");
            CancelInvoke("HealOverTime");
            Die();
            return;
        }

        lastDamageTime = Time.time;
        lastRecordedHealth = currentHealth;

        Debug.Log($"记录伤害时间：{lastDamageTime}，记录血量：{lastRecordedHealth}");

        CancelInvoke("CheckAndStartHealing");
        CancelInvoke("HealOverTime");
        Invoke("CheckAndStartHealing", healDelay);

        // 显示回血倒计时
        StartCoroutine(ShowHealCountdown());
    }

    private IEnumerator ShowHealCountdown()
    {
        float remainingTime = healDelay;
        while (remainingTime > 0)
        {
            if (debugText != null)
                debugText.text = $"回血倒计时：{remainingTime:F1}秒";
            remainingTime -= Time.deltaTime;
            yield return null;
        }
        if (debugText != null)
            debugText.text = "";
    }

    private void CheckAndStartHealing()
    {
        Debug.Log($"触发回血检测：当前时间 {Time.time}，最后伤害时间 {lastDamageTime}");
        Debug.Log($"当前血量 {currentHealth}，记录血量 {lastRecordedHealth}");

        if (Time.time - lastDamageTime < healDelay)
        {
            Debug.Log("未达到延迟时间，取消回血");
            return;
        }

        if (currentHealth == lastRecordedHealth && currentHealth < maxHealth)
        {
            Debug.Log("满足回血条件，开始回血");
            isHealing = true;
            healStartTime = Time.time;
            InvokeRepeating("HealOverTime", 0f, healInterval);
        }
        else
        {
            Debug.Log($"不满足回血条件：当前血量{(currentHealth == lastRecordedHealth ? "未变化" : "已变化")}，血量{(currentHealth >= maxHealth ? "已满" : "未满")}");
        }
    }

    private void HealOverTime()
    {
        if (currentHealth >= maxHealth || isrebirth)
        {
            Debug.Log("停止回血：血量已满或玩家复活");
            CancelHealing();
            return;
        }

        int newHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        Debug.Log($"回血中：{currentHealth} -> {newHealth} (间隔：{Time.time - healStartTime:F1}秒)");

        currentHealth = newHealth;
        UpdateHealthBar();
        UpdateHealthText();

        if (currentHealth >= maxHealth)
        {
            Debug.Log("回血完成");
            CancelHealing();
        }
    }

    private void CancelHealing()
    {
        Debug.Log("取消回血调用");
        CancelInvoke("HealOverTime");
        isHealing = false;
    }

    private void Die()
    {
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        inventory.SetActive(false);
        gameOverUI.SetActive(true);
        Debug.Log("玩家已死亡！");


 isWaitingForRespawn = true;
    StartCoroutine(WaitForAnyKeyToRespawn());



    }


private IEnumerator WaitForAnyKeyToRespawn()
{
    while (!Input.anyKeyDown )
    {
        yield return null;
    }

    isWaitingForRespawn = false;
    RestartGame();
}



    private void RestartGame()
    {
        Debug.Log("玩家复活，重置状态");
        isrebirth = true;
        CancelHealing();

        currentHealth = maxHealth;

        SceneManagerController sceneManager = FindObjectOfType<SceneManagerController>();
        if (sceneManager != null)
        {
            respawnPosition = sceneManager.GetPlayerSpawnPosition();
            Debug.Log("Respawn Position from SceneManager: " + respawnPosition);
        }
        else
        {
            Debug.LogError("SceneManagerController not found! Defaulting to (0, 0, 0).");
            respawnPosition = Vector3.zero;
        }
        transform.position = respawnPosition;

        Debug.Log("Player revived at position: " + transform.position);

        inventory.SetActive(true);
        gameOverUI.SetActive(false);
        UpdateHealthBar();
        UpdateHealthText();

        if (playerController != null)
        {
            playerController.enabled = true;
        }
        isrebirth = false;
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
            healthText.text = $"玩家的血量：{currentHealth}";
    }

    public void EnableManagerMode()
    {
        inputField.text = "";
        inputField.Select();
        inputField.ActivateInputField();
        inputFieldUI.SetActive(true);
    }

    public void CheckInput()
    {
        if (isrebirth)
        {
            Debug.Log("玩家已死亡，无法继续操作。");
            inputField.text = "";
            inputFieldUI.SetActive(false);
            return;
        }

        Debug.Log("Input Field Text: " + inputField.text);

        if (inputField.text == "Lbwnb")
        {
            isManager = true;
            maxHealth = 999999;
            currentHealth = maxHealth;
            UpdateHealthText($"欢迎管理员，你的血量为：{maxHealth}");
        }
        else if (inputField.text == "kill")
        {
            StartCoroutine(ApplyDamageOverTime(killBlood));
            Debug.Log("Started taking damage!");
        }
        else
        {
            Debug.Log("Incorrect input!");
        }

        inputField.text = "";
        inputFieldUI.SetActive(false);
        inventory.SetActive(true);
        Debug.Log("Input field hidden. Inventory activated.");
    }


// 在PlayerHealth.cs中添加以下方法
public void SetManagerMode(bool isManager)
{
    this.isManager = isManager;
    maxHealth = isManager ? 999999 : 100;
    currentHealth = maxHealth;
    UpdateHealthBar();
    UpdateHealthText();
    
    Debug.Log(isManager ? "管理员模式激活" : "管理员模式关闭");
}


    private void UpdateHealthText(string message)
    {
        if (healthText != null)
            healthText.text = message;
    }

    private IEnumerator ApplyDamageOverTime(float damagePerSecond)
    {
        while (currentHealth > 0)
        {
            TakeDamage((int)damagePerSecond);
            yield return new WaitForSeconds(1f);
        }
    }

    public void RequestInputFieldToggle()
    {
        if (!inputFieldUI.activeSelf)
        {
            EnableManagerMode();
        }
        else
        {
            inputFieldUI.SetActive(false);
            inventory.SetActive(true);
            isManager = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tilde))
        {
            RequestInputFieldToggle();
        }

        if (inputFieldUI.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            CheckInput();
        }
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateHealthBar();
        UpdateHealthText();
    }
}