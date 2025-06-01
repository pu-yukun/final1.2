using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventory : MonoBehaviour  
{
    [SerializeField] 
    public List<GameObject> weapons = new List<GameObject>();
    [SerializeField] 
    public List<int> weaponDamages = new List<int>();
    public int currentWeaponID;

    // Element编号到预制体索引的映射表（根据你的实际配置填写）
    [SerializeField] 
    private int[] elementToWeaponIndex = new int[] { 0, 1, 2, 4, 5 ,6 };

    void Start()  
    {  
        currentWeaponID = 0;
        ChargeWeapon(currentWeaponID);
    }  

    void Update()  
    {  
        HandleScrollWheel();
        HandleNumberInput();
    }  

    void HandleScrollWheel()  
    {  
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll < 0)  
        {  
            int nextID = (currentWeaponID + 1) % weapons.Count;
            ChargeWeapon(nextID);
        }  
        else if (scroll > 0)  
        {  
            int prevID = (currentWeaponID - 1 + weapons.Count) % weapons.Count;
            ChargeWeapon(prevID);
        }  
    }

    void HandleNumberInput()
    {
        // 数字键1-5对应Element0-4（根据需求调整）
        for (int elementID = 0; elementID < 6; elementID++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + elementID))
            {
                if (elementID < elementToWeaponIndex.Length)
                {
                    int weaponIndex = elementToWeaponIndex[elementID];
                    ChargeWeapon(weaponIndex);
                }
                break;
            }
        }
    }

    public void ChargeWeapon(int weaponIndex)
    {
        if (weapons.Count == 0 || weaponIndex < 0 || weaponIndex >= weapons.Count)
        {
            Debug.LogWarning($"无效的武器索引: {weaponIndex}");
            return;
        }

        currentWeaponID = weaponIndex;
        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].SetActive(i == weaponIndex);
        }
    }

    public int GetCurrentWeaponDamage()
    {
        if (currentWeaponID >= 0 && currentWeaponID < weaponDamages.Count)
        {
            return weaponDamages[currentWeaponID];
        }
        return 0;
    }
}