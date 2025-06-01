using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zombieatackrange : MonoBehaviour

{
public int damage=5;
     private GameObject player; // 玩家对象  
    private PlayerHealth playerHealth;
    public GameObject zombieattackrange;

    // Start is called before the first frame update
    void Start()
    {
  // 查找玩家对象  
        player = GameObject.Find("Player");  
        if (player == null)  
        {  
            Debug.LogError("玩家未找到！请确保玩家对象存在并命名为 'Player'。");  
            return;  
        }  

        // 获取 PlayerHealth 组件  
        playerHealth = player.GetComponent<PlayerHealth>();  
        if (playerHealth == null)  
        {  
            Debug.LogError("PlayerHealth 组件未找到！请确保玩家对象上添加了 PlayerHealth。");  
        } 

      
initialzombieattackrange();


       // activezombieattackrange();
        //deactivezombieattackrange();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void  initialzombieattackrange(){
zombieattackrange.SetActive(false);
Debug.Log("初始化攻击范围");
        
    }
public void activezombieattackrange(){
zombieattackrange.SetActive(true);
Debug.Log("激活攻击范围");
}
public void deactivezombieattackrange(){
    zombieattackrange.SetActive(false);
    Debug.Log("ban攻击范围");
}




      public  void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("攻击命中玩家！攻击范围: " + gameObject.name); // 打印攻击范围对象的名字
playerHealth.TakeDamage(damage);
        }
    }
}
