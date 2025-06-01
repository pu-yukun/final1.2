using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class hostageui : MonoBehaviour
{
    public GameObject hostageUI; // 人质UI面板  
    public Text hostageText; // 人质文本（用句号分隔句子）  
    public AudioSource audioSource; // 音频源  
    public AudioClip[] hostageClips; // 每句话的音频剪辑  
    public Button nextButton; // 下一句按钮  
    public Button skipButton; // 跳过人质对话按钮  
    private GameObject player; // 玩家对象  
    private GameObject hostage; // 人质对象  
    public float interactionDistance = 8f; // 互动距离  
    public GameObject hintCanvas; // 提示按钮的父物体 Canvas  

    private int currentStep = 0; // 当前对话步骤  
    private bool isHostageActive = false; // 人质对话是否激活  
    private string[] sentences; // 存储按句号分割后的句子  
    private bool isInRange = false; // 是否在互动范围内  
public static event Action OnDialogueEnd;
    void Start()
    {
        player = gamemanager.Instance.player;

        hostageUI.SetActive(false);
        if (hintCanvas != null)
        {
            hintCanvas.SetActive(false); // 初始隐藏提示按钮的父物体 Canvas
        }

        // 初始化句子数组  
        sentences = hostageText.text.Split(new[] { '。' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 绑定按钮事件  
        nextButton.onClick.AddListener(NextStep);
        skipButton.onClick.AddListener(SkipHostageDialogue);

        Debug.Log("hostageui 初始化完成");
    }

    void Update()
    {
        // 动态获取 hostage
        if (hostage == null)
        {
            hostage = gamemanager.Instance.GetCurrentHostage(); // 尝试重新获取 hostage
            if (hostage != null)
            {
                Debug.Log("成功获取 hostage: " + hostage.name);
            }
            else
            {
                Debug.LogWarning("未找到 hostage，请检查 hostage 的 Tag 或场景中是否存在 hostage 对象");
                return; // 如果没有 hostage，跳过后续逻辑
            }
        }

        // 检测玩家是否在互动范围内  
        float distance = Vector3.Distance(player.transform.position, hostage.transform.position);
        bool wasInRange = isInRange;
        isInRange = distance < interactionDistance;

        Debug.Log($"玩家与 hostage 的距离: {distance}, 是否在互动范围内: {isInRange}");

        // 状态变化时更新提示按钮的父物体 Canvas  
        if (distance <= interactionDistance && !isHostageActive)
        {
            Debug.Log($"提示按钮的父物体 Canvas 状态更新: {isInRange}");
            if (hintCanvas != null)
            {
                hintCanvas.SetActive(true); // 显示提示按钮的父物体 Canvas
            }
        }

        // 互动范围内且未激活人质对话时检测按键  
        if (isInRange && !isHostageActive)
        {
            Debug.Log("玩家在互动范围内，等待按键输入");
            if (Input.GetKey(KeyCode.LeftControl))
            {
                Debug.Log("按下 LeftControl，开始人质对话");
                StartHostageDialogue();
                if (hintCanvas != null)
                {
                    hintCanvas.SetActive(false); // 开始对话后隐藏提示按钮的父物体 Canvas
                }
            }
        }

        // 人质对话激活时的操作  
        if (isHostageActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("按下鼠标左键，进入下一步");
                NextStep();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("按下 P 键，重新开始对话");
                ReviewHostageDialogue();
            }

            // 如果对话进行中玩家离开互动范围，强制结束对话  
            if (!isInRange)
            {
                Debug.Log("玩家离开互动范围，强制结束对话");
                EndHostageDialogue();
            }
        }
    }

    public void StartHostageDialogue()
    {
        Debug.Log("开始人质对话");
        isHostageActive = true;
        hostageUI.SetActive(true);
        currentStep = 0;
        UpdateHostageStep();
    }

    public void NextStep()
    {
        if (currentStep < sentences.Length - 1)
        {
            currentStep++;
            Debug.Log($"进入下一步，当前步骤: {currentStep}");
            UpdateHostageStep();
        }
        else
        {
            Debug.Log("对话结束");
            EndHostageDialogue();
        }
    }

    public void UpdateHostageStep()
    {
        Debug.Log($"更新对话内容: {sentences[currentStep]}");
        hostageText.text = sentences[currentStep];
        if (currentStep < hostageClips.Length)
        {
            Debug.Log($"播放音频剪辑: {hostageClips[currentStep].name}");
            audioSource.clip = hostageClips[currentStep];
            audioSource.Play();
        }
    }

    public void SkipHostageDialogue()
    {
        Debug.Log("跳过对话");
        EndHostageDialogue();
    }

    public void ReviewHostageDialogue()
    {
        Debug.Log("重新开始对话");
        currentStep = 0;
        UpdateHostageStep();
    }

    public void EndHostageDialogue()
    {
        Debug.Log("结束对话");
        isHostageActive = false;
        hostageUI.SetActive(false);

        // 结束后如果仍在互动范围内，重新显示提示按钮的父物体 Canvas  
        if (isInRange && hintCanvas != null)
        {
            Debug.Log("重新显示提示按钮的父物体 Canvas");
            hintCanvas.SetActive(true);
        }
   OnDialogueEnd?.Invoke();

 MissionManager2.Instance.CompleteMission(0);

  }



}