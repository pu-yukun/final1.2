using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class knifetwoattackrange : MonoBehaviour
{
    // Start is called before the first frame update
 public GameObject knife;

    // Start is called before the first frame update
    void Start()
    {
        knife.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

public void twoknifeactive(){
knife.SetActive(true);
Debug.Log("刀2帧攻击已经激活");


}

public void twoknifedeactive(){
knife.SetActive(false);
Debug.Log("结束攻击2刀");


}

}
