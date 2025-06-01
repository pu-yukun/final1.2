using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TextDropEffect : MonoBehaviour
{
    [Header("UI Settings")]
    public Text uiText;
    public float charDisplaySpeed = 0.08f;  // 修改：加快显示速度
    public float fullTextDuration = 2.5f;

    [Header("Player Settings")]
    public Transform player;
    public float spawnDistance = 3.5f;      // 修改：增加生成距离
    public float minCharSpacing = 0.3f;     // 修改：减小间距

    [Header("Physics Settings")]
    public Font boldFont;
    public int fontSize = 36;
    public Color textColor = Color.white;
    public float dropForce = 1.8f;          // 修改：减小掉落力度
    public float airResistance = 1.5f;
    public float destroyDelay = 3f;

    [Header("Layout Settings")]             // 新增：布局参数组
    public int maxCharsPerLine = 12;        // 新增：每行最大字符数
    public float lineSpacing = 0.8f;        // 新增：行间距
    public float viewAngle = 25f;           // 新增：生成视角

    private List<GameObject> activeChars = new List<GameObject>();
    private string[] sentences;

    void Start()
    {
        string originalText = "嗯虽然不知道你是谁。但是恭喜你发现了这个彩蛋。你是一个很有毅力亦或者有决心的人。这是我的第一部作品。我从小有很多梦想。从当老师当作家，到现在的做游戏。都有一个共同点。即使在几十或几百年后。我早已不在但我的作品或思想仍然流传。这很酷不是吗。从前我父亲一句话激起我的兴趣。你会打游戏怎么不会去做呢？。而这个作品便是我的回应。以后生活所迫我可能不干这个。但我的作品也会提醒这我。原来我大学的时候也有一腔热血。不都说了恭喜你找到彩蛋。你将解锁管理员权限。按*可以获取你的权限。";
        sentences = originalText.Split(new[] { '。' }, System.StringSplitOptions.RemoveEmptyEntries);
        StartCoroutine(DisplaySequence());
    }

    IEnumerator DisplaySequence()
    {
        foreach (string sentence in sentences)
        {
            // 显示UI文字
            yield return StartCoroutine(ShowTextOneByOne(sentence + "。"));
            
            // 动态调整显示时间
            float dynamicDuration = Mathf.Clamp(fullTextDuration, 
                                              sentence.Length * 0.1f, 
                                              fullTextDuration * 2);
            yield return new WaitForSeconds(dynamicDuration);
            
            // 物理掉落效果
            StartCoroutine(DropCharacters(sentence));
            
            // 优化等待时间
            float dropDuration = Mathf.Max(
                sentence.Length * charDisplaySpeed * 0.5f,
                destroyDelay * 1.5f
            );
            yield return new WaitForSeconds(dropDuration);
            
            uiText.text = "";
        }
        
        // 终局处理
        uiText.text = "彩蛋结束";
        yield return new WaitForSeconds(2f);
        uiText.text = "";
    }

    IEnumerator ShowTextOneByOne(string text)
    {
        uiText.text = "";
        foreach (char c in text)
        {
            uiText.text += c;
            yield return new WaitForSeconds(charDisplaySpeed);
        }
    }

    IEnumerator DropCharacters(string sentence)
    {
        // 基于视角计算生成点
        Vector3 spawnDirection = Quaternion.Euler(-viewAngle, 0, 0) * player.forward;
       // Vector3 basePos = player.position + spawnDirection.normalized * spawnDistance  Vector3.up * 2f;
// 修改为：
Vector3 basePos = player.position + 
                 spawnDirection.normalized * spawnDistance +
                 Vector3.up * 7f; // 新增：在原有基础上+2米
        // 分割多行
        List<string> lines = new List<string>();
        for (int i=0; i<sentence.Length; i+=maxCharsPerLine)
        {
            int length = Mathf.Min(maxCharsPerLine, sentence.Length - i);
            lines.Add(sentence.Substring(i, length));
        }

        // 整体居中布局
        float totalHeight = (lines.Count - 1) * lineSpacing;
        Vector3 startPos = basePos + Vector3.up * (totalHeight / 2);

        for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
        {
            string line = lines[lineIndex];
            Vector3 linePos = startPos - Vector3.up * lineIndex * lineSpacing;
            
            // 动态字符间距
            float screenWidth = Camera.main.orthographic? 
                Camera.main.orthographicSize * 2 * Screen.width/Screen.height :
                Mathf.Tan(Mathf.Deg2Rad*Camera.main.fieldOfView/2) * spawnDistance * 2;
            
            float charSpacing = Mathf.Min(
                minCharSpacing,
                screenWidth / (line.Length + 1)
            );

            Vector3 lineStart = linePos - player.right * charSpacing * (line.Length - 1) / 2;

            for (int i = 0; i < line.Length; i++)
            {
                Vector3 spawnPos = lineStart + player.right * i * charSpacing;
                spawnPos += new Vector3(
                    Random.Range(-0.1f, 0.1f),
                    Random.Range(-0.1f, 0.1f),
                    Random.Range(-0.1f, 0.1f)
                );

                GameObject charObj = CreateCharacter(line[i], spawnPos);
                activeChars.Add(charObj);

                Rigidbody rb = charObj.AddComponent<Rigidbody>();
                rb.drag = airResistance;
                rb.AddForce(new Vector3(
                    Random.Range(-dropForce, dropForce),
                    Random.Range(-dropForce * 0.5f, 0),
                    0
                ), ForceMode.Impulse);

                rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                                RigidbodyConstraints.FreezeRotationZ;

                FaceToPlayer(charObj.transform);
                yield return new WaitForSeconds(charDisplaySpeed * 0.7f); // 修改：加快生成间隔
            }
        }

        StartCoroutine(CleanupCharacters());
    }

    GameObject CreateCharacter(char c, Vector3 pos)
    {
        GameObject charObj = new GameObject("Char_" + c);
        charObj.transform.position = pos;

        TextMesh textMesh = charObj.AddComponent<TextMesh>();
        textMesh.text = c.ToString();
        textMesh.font = boldFont;
        textMesh.fontSize = fontSize;
        textMesh.color = textColor;
        textMesh.anchor = TextAnchor.MiddleCenter;

        charObj.transform.Rotate(90, 0, 0);
        charObj.AddComponent<BoxCollider>().size = new Vector3(0.1f, 0.1f, 0.1f);

        return charObj;
    }

    void FaceToPlayer(Transform charTransform)
    {
        Vector3 lookDir = charTransform.position - Camera.main.transform.position;
        charTransform.rotation = Quaternion.LookRotation(lookDir) * 
                               Quaternion.Euler(0, 0, 0); // 修改：微调视角
    }

    IEnumerator CleanupCharacters()
    {
        yield return new WaitForSeconds(destroyDelay);
        foreach (var charObj in activeChars)
        {
            if (charObj != null) Destroy(charObj);
        }
        activeChars.Clear();
    }

    // 新增：调试可视化
    void OnDrawGizmosSelected()
    {
        if(player == null || !Application.isPlaying) return;
        
        Gizmos.color = Color.cyan;
        Vector3 dir = Quaternion.Euler(-viewAngle,0,0) * player.forward;
        Vector3 center = player.position + dir.normalized * spawnDistance;
        Gizmos.DrawWireSphere(center, 0.3f);
        Gizmos.DrawLine(player.position, center);
    }
}