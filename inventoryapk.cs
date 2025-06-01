using System.Collections;  
using System.Collections.Generic;  
using UnityEngine;  
using UnityEngine.UI; // 引入 UI 命名空间  

public class inventoryapk : MonoBehaviour // 注意类名应为首字母大写，符合 C# 命名约定  
{  
    [SerializeField] // 使用 SerializeField 显示在 Inspector 中  
    public List<GameObject> weapons = new List<GameObject>();  
    public int currentWeaponID;  

    // 添加一个按钮引用  
    [SerializeField]  
    private Button nextWeaponButton; // 用于切换到下一个武器的按钮  

    void Start()  
    {  
        currentWeaponID = 0; // 默认武器ID为0  
        UpdateWeaponVisibility(); // 更新武器的可见性  
        if (nextWeaponButton != null) // 确保按钮不为空  
        {  
            nextWeaponButton.onClick.AddListener(OnNextWeaponButtonClicked); // 添加按钮点击事件监听  
        }  
    }  

    void Update()  
    {  
        chargeCurrentID(); // 每帧检查滚轮输入  
    }  

    public void chargeCurrentID()  
    {  
        if (Input.GetAxis("Mouse ScrollWheel") < 0)  
        {  
            // 正滚加一ID  
            ChargeWeapon((currentWeaponID + 1) % weapons.Count); // 保证循环使用  
        }  
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)  
        {  
            // 反滚减一ID  
            ChargeWeapon((currentWeaponID - 1 + weapons.Count) % weapons.Count); // 保证循环使用  
        }  
    }  

    public void ChargeWeapon(int weaponID)  
    {  
        if (weapons.Count == 0) return;  

        currentWeaponID = weaponID; // 更新索引  
        UpdateWeaponVisibility(); // 更新武器的可见性  
    }  

    private void UpdateWeaponVisibility()  
    {  
        // 激活相应的武器，禁用其他武器  
        for (int i = 0; i < weapons.Count; i++)  
        {  
            weapons[i].SetActive(i == currentWeaponID);  
        }  
    }  

    // 按钮点击事件  
    private void OnNextWeaponButtonClicked()  
    {  
        ChargeWeapon((currentWeaponID + 1) % weapons.Count); // 切换到下一个武器  
    }  
}