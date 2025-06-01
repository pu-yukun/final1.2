using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{

    public Button startButton;
    public Button exitButton;
    
    [Header("动画设置")]
    public float scaleFactor = 1.2f;
    public float animationDuration = 2f;

 
    private Text startText;
    private Text exitText;
    private Vector3 startOriginalScale;
    private Vector3 exitOriginalScale;

    void Start()
    {
        
        startText = startButton.GetComponentInChildren<Text>();
        exitText = exitButton.GetComponentInChildren<Text>();
        
        startOriginalScale = startText.transform.localScale;
        exitOriginalScale = exitText.transform.localScale;

      
        startButton.onClick.AddListener(() => StartCoroutine(StartGameRoutine()));
        exitButton.onClick.AddListener(() => StartCoroutine(ExitGameRoutine()));
    }

    IEnumerator StartGameRoutine()
    {
        startButton.interactable = false;
        
        // 同时修改缩放和字体
        startText.transform.localScale = startOriginalScale * scaleFactor;
        startText.fontStyle = FontStyle.Bold;

        yield return new WaitForSeconds(animationDuration);
        
     
        SceneManager.LoadScene("officialgame1"); 
    }

    IEnumerator ExitGameRoutine()
    {
        exitButton.interactable = false;
        
        exitText.transform.localScale = exitOriginalScale * scaleFactor;
        exitText.fontStyle = FontStyle.Bold;

        yield return new WaitForSeconds(animationDuration);
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}