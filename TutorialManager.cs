using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialUI; // 教程UI面板
    public Text tutorialText; // 教程文本（用句号分隔句子）
    public AudioSource audioSource; // 音频源
    public AudioClip[] tutorialClips; // 每句话的音频剪辑
    public Button nextButton; // 下一句按钮
    public Button skipButton; // 跳过教程按钮
    public GameObject player; // 玩家对象
    public GameObject teacher; // 教官对象
    public float interactionDistance = 8f; // 互动距离
    public Button hintButton; // 新增：提示按钮（按Ctrl开始教程）

    private int currentStep = 0; // 当前教程步骤
    private bool isTutorialActive = false; // 教程是否激活
    private string[] sentences; // 存储按句号分割后的句子
    private bool isInRange = false; // 是否在互动范围内

    void Start()
    {
        tutorialUI.SetActive(false);
        hintButton.gameObject.SetActive(false); // 初始隐藏提示按钮

        // 初始化句子数组
        sentences = tutorialText.text.Split(new[] { '。' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 绑定按钮事件
        nextButton.onClick.AddListener(NextStep);
        skipButton.onClick.AddListener(SkipTutorial);
    }

    void Update()
    {
        // 检测玩家是否在互动范围内
        float distance = Vector3.Distance(player.transform.position, teacher.transform.position);
        bool wasInRange = isInRange;
        isInRange = distance < interactionDistance;

        // 状态变化时更新提示按钮
        if (isInRange != wasInRange && !isTutorialActive)
        {
            hintButton.gameObject.SetActive(isInRange);
        }

        // 互动范围内且未激活教程时检测按键
        if (isInRange && !isTutorialActive)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                StartTutorial();
                hintButton.gameObject.SetActive(false); // 开始教程后隐藏提示按钮
            }
        }

        // 教程激活时的操作
        if (isTutorialActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                NextStep();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                ReviewTutorial();
            }

            // 如果教程进行中玩家离开互动范围，强制结束教程
            if (!isInRange)
            {
                EndTutorial();
            }
        }
    }

   public void StartTutorial()
    {
        isTutorialActive = true;
        tutorialUI.SetActive(true);
        currentStep = 0;
        UpdateTutorialStep();
    }

   public void NextStep()
    {
        if (currentStep < sentences.Length - 1)
        {
            currentStep++;
            UpdateTutorialStep();
        }
        else
        {
            EndTutorial();
        }
    }

   public void UpdateTutorialStep()
    {
        tutorialText.text = sentences[currentStep];
        if (currentStep < tutorialClips.Length)
        {
            audioSource.clip = tutorialClips[currentStep];
            audioSource.Play();
        }
    }

public    void SkipTutorial()
    {
        EndTutorial();
    }

 public   void ReviewTutorial()
    {
        currentStep = 0;
        UpdateTutorialStep();
    }

 public   void EndTutorial()
    {
        isTutorialActive = false;
        tutorialUI.SetActive(false);

        // 结束后如果仍在互动范围内，重新显示提示按钮
        if (isInRange)
        {
            hintButton.gameObject.SetActive(true);
        }
    }
  
}