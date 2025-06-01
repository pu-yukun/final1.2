using UnityEngine;  
using UnityEngine.SceneManagement;  
using UnityEngine.UI;  
using System.Collections;  

public class SceneLoader : MonoBehaviour  
{  
    public GameObject loadingScreen; // 加载界面  
    public Slider loadingBar;         // 加载条（如果使用Slider）  
    public float minLoadingTime = 2f; // 最小加载时间，避免加载过快看不到效果  

    // 调用此方法来加载新场景  
    public void LoadScene(string sceneName)  
    {  
        StartCoroutine(LoadSceneAsync(sceneName));  
    }  

    private IEnumerator LoadSceneAsync(string sceneName)  
    {  
        // 显示加载界面  
        loadingScreen.SetActive(true);  

        // 开始异步加载场景  
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);  

        // 禁用场景激活，等待加载完成  
        operation.allowSceneActivation = false;  

        float startTime = Time.time;  

        while (!operation.isDone)  
        {  
            // 更新加载条的进度  
            float progress = Mathf.Clamp01(operation.progress / 0.9f);  
            loadingBar.value = progress;  

            // 检查场景是否加载完成  
            if (operation.progress >= 0.9f)  
            {  
                // 等待一段时间，以确保加载条可见  
                if (Time.time - startTime >= minLoadingTime)  
                {  
                    operation.allowSceneActivation = true; // 激活场景  
                }  
            }  

            yield return null; // 等待下一帧  
        }  

        // 隐藏加载界面  
        loadingScreen.SetActive(false);  
    }  
}