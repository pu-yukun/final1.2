using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 

public class we : Weaponf{  

private Vector3 originalGunCameraPosition; // 存储原始位置  
private Quaternion originalGunCameraRotation; // 存储原始旋转

  private Camera Scope_Render_Camera_Back;
// 新增的成员变量，表示开镜状态下的准星缩小比例  
private int sdqzds;  
private bool isReloadingLocked = false;
private PlayerController playerControllers;
private Camera mainCamera;
 [Header("武器摄像机")]  
public Camera gunCamera;

public GameObject reloadsound;
private Camera scope;
 [System.Serializable] //这个特性只需要在内部类中声明一次  
 public class SoundClips {  
 public AudioClip shootSound;public AudioClip silencerShootSound; 
 // 消音器
  public AudioClip reloadingSound; // reload人物说话音效 
  public AudioClip reloadSoundOutofAmmo; // 子弹完了，换子弹完整音效 
  public AudioClip aimSound;
public AudioClip reloadSoundAmmotLeft;


   // 瞄准音效 
   }  



 [Header("武器部件位置")]  
 [Tooltip("射擊位置")] public Transform ShootPoint;  
 public Transform BulletShootPoint; // 子弹位置
  [Tooltip("子弹壳位置")] public Transform CasingBulletSpawnPoint;  

public Transform bulletPrefab;
public Transform casingPrefab;

 [Header("近战")] 
[Tooltip("近战攻击按键")]
public KeyCode meleeKey = KeyCode.F; // 近战按键
private bool isMeleeAttacking = false; // 是否正在近战攻击


 [Header("槍械屬性")]  
 [Tooltip("武器射程")] public float range;  
 [Tooltip("武器射速")] public float fireRate;  
 private float originRate; // 原始射速 
 private float SpreadFactor; // 射击误差偏移量 
 private float fireTimer; // 开火时间计时器 
 private float bulletForce; // 后坐力
  [Tooltip("彈夾")] public int bulletMag;  
 [Tooltip("彈夾内子彈數")] public int currentBullets;  
 [Tooltip("备用彈")] public int bulletleft; // public只说明不赋值让前台按照不同枪械备用不同子弹 

public bool isSilencer;//判断消音器
private int shotgunfragemnet=15;

 [Header("特效")]  
 public Light muzzleflashLight; // 设置枪口亮光,light类型的
  private float lightDuration; // 持续时间 
  public ParticleSystem muzzlePatic; // 灯光火焰粒子特效 
  public ParticleSystem sparkPatic; // 火星
   public int minSparkEmission =1;  
 public int maxSparkEmission =7;  

 [Header("开火声音")]  
 private AudioSource gunaudio;  
 public SoundClips soundClips; // 使用内部类

 [Header("UI")]
 public Image[] crossQuarterImgs;//准星
 public float currentExpanedDegree;  //当前准星开合度，急停有关
public float crossExpanedDegree;
   public float maxCrossDegree;
public Text ammoTextUI;
public Text shootModeTextUI;


 public PlayerController.MovementState state;
 private string shootModeName;//区分全自动和半自动
private AudioSource mainAudioSource;
private Animator animator;

private bool isAiming;//判断瞄准
private bool isreloading;
private Vector3 sniperingFifilePosition;//初始腰射位置
public Vector3 sniperingFifileOnPosition;//瞄准位置

public bool zidong;
private string zidongxianshi;



 [Header("键位设置")]
 [Tooltip("自动切换")]private  KeyCode qh = KeyCode.X;
[Tooltip("装弹")]private  KeyCode reloadInputName = KeyCode.R;
[Tooltip("监视动画")]private  KeyCode inspectInputName = KeyCode.I;

[Header("泵动枪械特殊设置")]  
[Tooltip("只能处于半自动状态")]public bool onlybzd;


 [Header("狙击镜片设置")]
 [Tooltip("镜片材质")]public Material scopeRenderMaterial;
 [Tooltip("没有开镜视野颜色")]public Color fadeColor;
 public Color defaultColor;
[Tooltip("启用狙击枪开镜单独模式")]public bool ifjjq;
[Tooltip("启用狙击枪镜片物体")]public GameObject scopecomponet;

  private void Awake() // 获取组件
  {  






    mainCamera=Camera.main;
  
animator=GetComponent<Animator>();
mainAudioSource=GetComponent<AudioSource>();
    playerControllers  =  GetComponentInParent <PlayerController>();
 gunaudio = GetComponent<AudioSource>();  


 }  


 void Start()  
 {  





  zidong=false;
sniperingFifilePosition = transform.localPosition;

  muzzleflashLight.enabled=false;
 lightDuration =0.02f;  
 range =300f;  
 bulletForce =150f;  
 bulletleft = bulletMag *10;  
 currentBullets = bulletMag;  
 }  

  void Update()  
 {  

// 近战攻击
    if (Input.GetKeyDown(meleeKey) && !isMeleeAttacking && !isreloading && !isAiming)
    {
        MeleeAttack();
    }





if (!onlybzd&&Input.GetKeyDown(KeyCode.X)) {  
    zidong = !zidong; // 切换 zidong 的值  
     UpdateShootModeText(); // 更新 UI 文本
    Debug.Log("当前射击模式: " + (zidong ? "全自动" : "半自动")); 

}else if(onlybzd){
    //Debug.Log("当前射击模式只能是半自动" );  
}




  HandleAiming();
  if(isAiming){
    Aimin();
  }else{
    AimOut();
  }
state=playerControllers.state;


if(state==PlayerController.MovementState.walking && Vector3.SqrMagnitude(playerControllers.moveDirection)>0 && state!=PlayerController.MovementState.running &&state!=PlayerController.MovementState.crouching){

  ExpanCrossUpdate(crossExpanedDegree);
  
}else if(state!=PlayerController.MovementState.walking&& state==PlayerController.MovementState.running&& state!= PlayerController.MovementState.crouching){


  ExpanCrossUpdate(crossExpanedDegree*2);//奔跑准星扩散度2倍
}else{
  ExpanCrossUpdate(0);//闲置为0
}

if(Input.GetKeyDown(reloadInputName) &&currentBullets<bulletMag &&bulletleft>0){//按下换单执行,且不能满仓换弹,且剩余子弹可以换弹


  ReloadAnimation();
  isreloading=true;
}


if(Input.GetMouseButton(1)&& !isreloading){

  isAiming =true;
  animator.SetBool("Aim",isAiming);
  transform.localPosition = sniperingFifileOnPosition;
}else{
  isAiming = false;
    animator.SetBool("Aim",isAiming);

}


//扩散度后期加装开镜时候增加if语句使得不同
SpreadFactor=(isAiming)?0.0001f : 0.015f; //如果是isaming为0.0001f不是为0.2f
 



 if (fireTimer < fireRate)  
 {  
 fireTimer += Time.deltaTime;  
 }  
AnimatorStateInfo info=animator.GetCurrentAnimatorStateInfo(0);//因为要和散弹枪换弹分开获得当前动画区分动画名字来执行各种换弹规模
if(info.IsName("reload_ammo_left")||info.IsName("reload_out_of_ammo")||info.IsName("reload_open")||info.IsName("reload_close")||info.IsName("reload_insert 1")||info.IsName("reload_insert 2")
||info.IsName("reload_insert 3")||info.IsName("reload_insert 4")||info.IsName("reload_insert 5")||info.IsName("reload_insert 6")){
  isreloading=true;
}else{
  isreloading=false;
}


if(Input.GetKeyDown(inspectInputName)){
animator.SetTrigger("inspect");

}



 if (zidong==true) {  
zidongxianshi="爆炸开火";
  if (Input.GetMouseButton(0) && currentBullets>0) {  

if(onlybzd==true){
  shotgunfragemnet=5;
   GunFire();  
 
}

if(onlybzd==false){
shotgunfragemnet=1;
   GunFire(); 

}

 }    
 }else if(zidong==false){
zidongxianshi="半自动开火";

   if (Input.GetMouseButtonDown(0) && currentBullets>0) {  
 

if(onlybzd==true&&ifjjq==false){
  shotgunfragemnet=8;
   GunFire(); 
   UpdateAmmoUI(); 
}else{


shotgunfragemnet=1;
   GunFire();  
   UpdateAmmoUI(); 



}




 }  
 }
 }
//


void MeleeAttack()
{
    // 设置触发器，播放近战攻击动画
    animator.SetTrigger("knife");
    isMeleeAttacking = true;

    // 播放近战攻击音效
   // if (meleeSound != null)
    //{
    //    gunaudio.PlayOneShot(meleeSound);
   // }

}

 public override void GunFire()  
 {  
  isMeleeAttacking=false;
 if (fireTimer < fireRate || currentBullets <=0  || animator.GetCurrentAnimatorStateInfo(0).IsName("take_out") || isreloading || animator.GetCurrentAnimatorStateInfo(0).IsName("inspect")) return; //获取动画层级以及类名  
 

 StartCoroutine(MuzzleFlashLight()); // 封装方法存放开火特效
  muzzlePatic.Emit(1); // emit方法立刻发送多少例子仅限particle system 
  sparkPatic.Emit(UnityEngine.Random.Range(minSparkEmission, maxSparkEmission)); // 随机发射调用下面方法，在min于max之间 // 开火时播放声音 
  PlaySound(soundClips.shootSound);  
StartCoroutine(Shootcs());
 

 if(!isAiming){
animator.CrossFadeInFixedTime("fire",0.1f);//淡入淡出方法动画
 }else{
animator.Play("aim_fire",0,0);

 }



for(int i=0;i<shotgunfragemnet;i++){

 RaycastHit hit;  
 Vector3 shootDirection = ShootPoint.forward; //仅仅向枪管前方为射击点
  shootDirection += ShootPoint.TransformDirection(new Vector3(UnityEngine.Random.Range(-SpreadFactor, SpreadFactor),UnityEngine.Random.Range(-SpreadFactor, SpreadFactor))); // 将射击方向变为世界空间 
  if (Physics.Raycast(ShootPoint.position, shootDirection, out hit, range))  
 {  
Transform bullet=null;

if(onlybzd==false){
bullet=(Transform)Instantiate(bulletPrefab,BulletShootPoint.transform.position,BulletShootPoint.transform.rotation);
   Debug.Log("生成一粒子弹");  
}else if(onlybzd==true&&ifjjq==false){

 bullet=Instantiate(bulletPrefab,hit.point,Quaternion.FromToRotation(Vector3.up,hit.normal)); 
    Debug.Log("生成散弹子弹");  
}else if(onlybzd==true&&ifjjq==true){

 bullet=(Transform)Instantiate(bulletPrefab,BulletShootPoint.transform.position,BulletShootPoint.transform.rotation);
    Debug.Log("生成狙击枪子弹");  
}else{
bullet=(Transform)Instantiate(bulletPrefab,BulletShootPoint.transform.position,BulletShootPoint.transform.rotation);
    Debug.Log("生成 普通子弹");  
}


bullet.GetComponent<Rigidbody>().velocity=(bullet.transform.forward+shootDirection)*bulletForce;//给子弹朝向加随机偏移量*子弹的力
 Debug.Log("Hit something");  
 }  
//抛蛋壳
}






//抛蛋壳
 Instantiate(casingPrefab,CasingBulletSpawnPoint.transform.position,CasingBulletSpawnPoint.transform.rotation);
gunaudio.clip=isSilencer?soundClips.silencerShootSound: soundClips.shootSound;
 fireTimer =0f; // 重置 
 currentBullets--; // 射击子弹减少 
 UpdateAmmoUI();
 }
//

 
 public IEnumerator MuzzleFlashLight()  
 {  
 muzzleflashLight.enabled = true;  
 yield return new WaitForSeconds(lightDuration);  
 muzzleflashLight.enabled = false;  
 }  

 private void PlaySound(AudioClip clip)  
 {  
 if (clip != null && gunaudio != null)  
 {  
 gunaudio.PlayOneShot(clip);  
 }  
 }  

 public override void Aimin() {
  
  Debug.Log("正确运行啊混蛋");
  
  float currentVelocity= 0f;
for(int i=0; i<crossQuarterImgs.Length;i++){

  crossQuarterImgs[i].gameObject.SetActive(false);
}
if((gameObject.name=="5")){

 
  scopeRenderMaterial.color=defaultColor;

  gunCamera.fieldOfView=20;
        Debug.Log("成功改变");





Debug.Log("成功改变");
}else{
  Debug.Log("不是瞄准镜武器");
}
if(ifjjq){
mainCamera.fieldOfView=Mathf.SmoothDamp(30,60,ref currentVelocity,0.1f);//平滑摄像头让次逼近视野
mainAudioSource.clip=soundClips.aimSound;
mainAudioSource.Play();
Invoke("openscope",0.1f);
//瞄准改变视野
}
  }  
  void openscope(){
    scopecomponet.SetActive(true);
  }
 public override void AimOut() { 


 // 隐藏 gunCamera 
float currentVelocity= 0f;
for(int i=0; i<crossQuarterImgs.Length;i++){

  crossQuarterImgs[i].gameObject.SetActive(true);
}
if((gameObject.name=="5")){
  mainCamera.fieldOfView=Mathf.SmoothDamp(60,30,ref currentVelocity,0.2f);//平滑摄像头让次逼近视野
  scopeRenderMaterial.color=fadeColor;
  gunCamera.fieldOfView=30;
Debug.Log("成功改变");
   scopecomponet.SetActive(false);

}else{
  Debug.Log("不是瞄准镜武器");
}


 }  


public override void ReloadAnimation() {  
//设计装弹动画
  if (onlybzd && currentBullets < bulletMag && bulletleft > 0&&!isReloadingLocked) {  
        // 计算需要装填的子弹数量  
        isReloadingLocked=true;
        int bulletsToLoad = Mathf.Min(bulletMag - currentBullets, bulletleft);  
         
if(reloadsound!=null){
  reloadsound.SetActive(true);
}


        // 播放装弹主动画  
        if(!isReloadingLocked){animator.SetTrigger("shotgun_reload");}
         // 播放主动画  
        Debug.Log("开始装弹动画。");  

        // 等待主动画播放完成  
        //yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // 等待主动画播放完成  

        // 根据需要装填的子弹数量进行装填  
        
            // 计算当前状态名称  
          

            // 播放对应的装填动画  

     
     switch (bulletsToLoad) {  
    case 1:  
        animator.Play("reload_insert6");  
        sdqzds=1;
        isreloading = true;
           // mainAudioSource.PlayOneShot(soundClips.reloadingSound); // 播放装弹音效
        break;  
    case 2:  
        animator.Play("reload_insert5");
        sdqzds=2;  
        isreloading = true;
        break;  
    case 3:  
        animator.Play("reload_insert4");
        sdqzds=3;  
        isreloading = true;
        break;  
    case 4:  
        animator.Play("reload_insert3");  
        sdqzds=4;
        isreloading = true;
        break;  
    case 5:  
        animator.Play("reload_insert2");  
        sdqzds=5;
        isreloading = true;
        break;  
    case 6:  
        animator.Play("reload_insert1"); 
        sdqzds=6;
        isreloading = true; 
        break;  
    default:  
        Debug.LogWarning("无效的子弹数量。");
        sdqzds=0; // 处理不在1到6范围内的情况  
        break;  
} 
 

  for(int i=1;i<=sdqzds;i++){
            //Debug.Log($"装入子弹。");  
            
            // 更新子弹数量  
            currentBullets++;  // 当前子弹+1  
            bulletleft--;      // 剩余子弹-1  
            UpdateAmmoUI();    // 更新 UI 显示  
   
            // 等待装填动画播放完成  
           // yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // 等待当前动画播放完成  
        

        // 装弹完成后的状态处理  
       
  }
 isreloading = false; // 重置装填状态  
        Debug.Log("装弹完成。");  
isReloadingLocked=false;

    }
if(currentBullets>0 && bulletleft>0&&onlybzd==false){
animator.Play("reload_ammo_left",0,0);
Reload();
mainAudioSource.clip=soundClips.reloadSoundAmmotLeft;
mainAudioSource.clip=soundClips.reloadingSound;
 mainAudioSource.PlayOneShot(soundClips.reloadSoundAmmotLeft,6f);
mainAudioSource.Play();
 StartCoroutine(ResetReloadStateAfterAnimation("reload_ammo_left"));  
}else if(currentBullets==0 &&bulletleft>0&&onlybzd==false){
animator.Play("reload_out_of_ammo",0,0);
Reload();
mainAudioSource.clip=soundClips.reloadSoundAmmotLeft;;
mainAudioSource.clip=soundClips.reloadingSound;
mainAudioSource.Play();
mainAudioSource.PlayOneShot(soundClips.reloadSoundOutofAmmo,8f); 


       StartCoroutine(ResetReloadStateAfterAnimation("reload_out_of_ammo")); 

}
}



 
 public override void Reload() { 

if(bulletleft<=0)
return;
 int bulletToload= bulletMag - currentBullets;//要装填子弹等于弹夹数减去剩余弹夹里面的子弹
int bulletToreduce=bulletleft>=bulletToload? bulletToload : bulletleft;
bulletleft-=bulletToreduce;//备弹减少
currentBullets +=bulletToreduce;
UpdateAmmoUI();

   scopecomponet.SetActive(false);

 }  
public void  UpdateAmmoUI(){

   //Debug.Log($"当前子弹: {currentBullets}, 剩余备用弹: {bulletleft}");   
    ammoTextUI.text = $"{currentBullets}/{bulletleft}";  

ammoTextUI.text=currentBullets+"/"+bulletleft;
shootModeTextUI.text=shootModeName;
}




 public override void ExpanCrossUpdate(float expande) { 

   float adjustmentFactor = isAiming ? 0.15f : 0.8f;
if(currentExpanedDegree<adjustmentFactor*expande-5)
{
  ExpanCross(60*Time.deltaTime);
}else if(currentExpanedDegree>adjustmentFactor*expande + 5){
  ExpanCross(-300*Time.deltaTime);


}

 }  

 public void ExpanCross(float add){
  float scaleFactor = isAiming ? 0.15f : 1f; // 在开镜状态下缩小范围 
  crossQuarterImgs[0].transform.localPosition+=new Vector3(-add*scaleFactor,0,0);//左准星 
 crossQuarterImgs[1].transform.localPosition+=new Vector3(add*scaleFactor,0,0);//右准星 
  crossQuarterImgs[2].transform.localPosition+=new Vector3(0,add*scaleFactor,0);//上准星 
   crossQuarterImgs[3].transform.localPosition+=new Vector3(0,-add*scaleFactor,0);//下准星
   currentExpanedDegree+=add*scaleFactor;
   currentExpanedDegree =Mathf.Clamp(currentExpanedDegree,0,maxCrossDegree);//限制准星开合度大小 

 }

public IEnumerator Shootcs(){
yield return null;
for(int i=0; i<10; i++){
ExpanCross(Time.deltaTime*150);

}

}

private void HandleAiming()   
{  
    if (isreloading)  
    {  
        isAiming = false; // 如果正在换弹，不允许开镜  
    }  
    else  
    {  
        if (Input.GetMouseButton(1)) // 右键进行瞄准  
        {  
            isAiming = true;  
        }  
        else  
        {  
            isAiming = false;  
        }  
    }  
    animator.SetBool("Aim", isAiming); // 更新 Animator 状态  
}  

private IEnumerator ResetReloadStateAfterAnimation(string animationName)  
{  
    // 等待动画播放结束，具体时间需要根据动画时长确定  
    yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);  
    isreloading = false; // 结束换弹  
}  
 private void UpdateShootModeText()  
    {  
        if (shootModeTextUI != null) // 确保 UI 元素未为 null  
        {  
            shootModeTextUI.text = zidong ? "全自动" : "半自动"; // 更新 UI 文本  
        }  
    }  

 }
