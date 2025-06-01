using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]  
public class SoundClips {
public AudioClip shootSound;//普通开火
public AudioClip SliencerShootSound;//消音器
public AudioClip reloadingSound;//reload音效
public AudioClip reloadSoundOutofAmmo;//子弹完了，换子弹完整音效
public AudioClip aimSound;//瞄准音效

}



public class Weapon_autoGun : Weaponf
{





[Header("武器部件位置")]

[Tooltip("射擊位置")]public Transform ShootPoint;
public Transform BulletShootPoint;//子彈位置 
[Tooltip("子彈殼位置")]public Transform CasingBulletSpawnPoint;

[Header("槍械屬性")]

[Tooltip("武器射程")]public float range;
[Tooltip("武器射速")]public float fireRate;
private float originRate;//原始射速
private float SpreadFactor;//射擊誤差偏移量
private float fireTimer;//開火時間計時器
private float bulletForce;//後坐力
[Tooltip("彈夾")]public int bulletMag;
[Tooltip("彈夾内子彈數")]public int currentBullets;
[Tooltip("备用彈")]public int bulletleft;//public只说明不赋值让前台按照不同枪械备用不同子弹


[Header("特效")]
public Light muzzleflashLight;//设置枪口亮光,light类型的
private float lightDuration;//持续时间
public ParticleSystem muzzlePatic;//灯光火焰粒子特效
public ParticleSystem sparkPatic;//火星 
public int minSparkEmission =1;
public int maxSparkEmission =7;

[Header("开火声音 ")]
private AudioSource gunaudio;
public SoundClips soundClips;


private void Awake()//获取音效组件w
{
gunaudio=GetComponent<AudioSource>();

}

private void Start(){
lightDuration=0.02f;

range = 300f;
bulletForce = 100f;
bulletleft=bulletMag*10;
currentBullets=bulletMag;


}
private void Update(){

if(fireTimer<fireRate){
  fireTimer+=Time.deltaTime;
}



  if(Input.GetMouseButton(0)){  
GunFire();
}


    
}

    public override void GunFire(){



if(fireTimer<fireRate ||currentBullets<=0){

  return;
  
}
StartCoroutine(MuzzleFlashLight());//封装方法存放开火特效
muzzlePatic.Emit(1);//emit方法立刻发送多少例子仅限particle system
sparkPatic.Emit(Random.Range(minSparkEmission,maxSparkEmission));//随机发射调用下面方法，在min于max之间
        RaycastHit hit;
Vector3 shootDirection =ShootPoint.forward;//僅僅向槍管前方為射擊點
shootDirection =shootDirection +ShootPoint.TransformDirection(new Vector3(Random.Range(SpreadFactor,SpreadFactor),Random.Range(SpreadFactor,SpreadFactor)));//將射擊方向變爲世界空間
if(Physics.Raycast(ShootPoint.position,shootDirection,out hit,range))
   {Debug.Log("a克塞");} 

fireTimer=0f;//重置
currentBullets--;//射击子弹减少
   }

public IEnumerator MuzzleFlashLight(){
muzzleflashLight.enabled =true;
yield return new WaitForSeconds(lightDuration);
muzzleflashLight.enabled =false;
}





   public override void Aimin(){



   }

 public override void AimOut(){


    
   }

 public override void ReloadAnimation(){


    
   }


    public override void Reload(){


    
   }

    public override void ExpanCrossUpdate(float expande){


    
   }

    // Update is called once per frame

}
