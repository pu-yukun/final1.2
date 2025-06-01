using UnityEngine;  

public class throwflash : MonoBehaviour  
{  
    [Header("References")]  
    public Transform flashbangPrefab; // 闪光弹预制体  
    public Transform throwOrigin;  

    [Header("Throw Settings")]  
    public float throwForce = 10f;  
    public float arcHeight = 2f;  
    public int lineSegmentCount = 20; // 轨迹点的数量  
public int flash=5;
    [Header("Audio")]  
    public AudioClip throwSound; // 投掷音效  
    private AudioSource audioSource;  

    private Animator animator;  
    private bool isAiming;  

    [Header("Line Renderer Settings")]  
    public LineRenderer lineRenderer; // 将 LineRenderer 作为公共变量  

    void Start()  
    {  
        audioSource = GetComponent<AudioSource>();  
        animator = GetComponent<Animator>();  

        if (lineRenderer != null)  
        {  
            lineRenderer.positionCount = lineSegmentCount;  
            // 你可以在这里设置 LineRenderer 的初始宽度  
            lineRenderer.startWidth = 0.1f; // 设定起始宽度  
            lineRenderer.endWidth = 0.1f;   // 设定结束宽度  
        }  
        else  
        {  
            Debug.LogError("LineRenderer is not assigned!");  
        }  
    }  

    void Update()  
    {  
        if (Input.GetKeyDown(KeyCode.G))  
        {  
            StartAiming();  
        }  

        if (Input.GetKeyUp(KeyCode.G))  
        {  
            ThrowFlashbang();  
        }  

        // 仅在瞄准时显示轨迹  
        if (isAiming)  
        {  
            ShowTrajectory();  
        }  
        else  
        {  
            if (lineRenderer != null)  
            {  
                lineRenderer.enabled = false; // 不显示轨迹  
            }  
        }  
    }  

    void StartAiming()  
    {  
        isAiming = true;  
        if (lineRenderer != null)  
        {  
            lineRenderer.enabled = true; // 显示轨迹  
        }  
    }  

    void ShowTrajectory()  
    {  
        Vector3 launchVelocity = CalculateThrowVelocity();  
        Vector3[] positions = new Vector3[lineSegmentCount];  

        for (int i = 0; i < lineSegmentCount; i++)  
        {  
            float t = i / (float)(lineSegmentCount - 1); // 计算进度  
            // 使用抛物线方程计算位置  
            positions[i] = throwOrigin.position + launchVelocity * t + Physics.gravity * t * t / 2f;  
        }  

        if (lineRenderer != null)  
        {  
            lineRenderer.SetPositions(positions);  
        }  
    }  

    void ThrowFlashbang()  
    {  

 if (flash <= 0)  
        {  
            Debug.Log("No flashbangs left to throw!");  
            return; // 如果没有可用的闪光弹，则返回，不执行后续代码  


             lineRenderer.enabled = false; // 显示轨迹  
        }  


        if (throwSound != null && audioSource != null)  
        {  
            audioSource.PlayOneShot(throwSound);  
        }  

        isAiming = false;  

        if (animator != null)  
            animator.SetTrigger("throw");  

        // 生成闪光弹实例  
        if (flashbangPrefab != null)  
        {  
            Transform flashbang = Instantiate(flashbangPrefab, throwOrigin.position, Quaternion.identity);  
            Rigidbody rb = flashbang.GetComponent<Rigidbody>();  
            if (rb != null)  
            {  
                rb.velocity = CalculateThrowVelocity();  
            }  

            flash--;
        }  
        else  
        {  
            Debug.LogError("Flashbang prefab is missing!");  
        }  

        if (lineRenderer != null)  
        {  
            lineRenderer.enabled = false; // 隐藏轨迹  
        }  
    }  

    Vector3 CalculateThrowVelocity()  
    {  
        Vector3 targetPos = Camera.main.transform.position + Camera.main.transform.forward * 10f;  
        Vector3 direction = (targetPos - throwOrigin.position).normalized;  
        return direction * throwForce + Vector3.up * arcHeight;  
    }  

   
}