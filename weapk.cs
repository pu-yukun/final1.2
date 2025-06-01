using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 

public class weapk : weaponfapk{  
// 新增的成员变量，表示开镜状态下的准星缩小比例  
  
private PlayerController playerControllers;
private Camera mainCamera;

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



    [Header("UI 按钮设置")]  
    public Button fireButton; // 开火按钮  
    public Button aimButton; // 瞄准按钮  
    public Button reloadButton; // 换弹按钮  
    public Button inspectButton; // 检视按钮  


 [Header("武器部件位置")]  
 [Tooltip("射擊位置")] public Transform ShootPoint;  
 public Transform BulletShootPoint; // 子弹位置
  [Tooltip("子弹壳位置")] public Transform CasingBulletSpawnPoint;  

public Transform bulletPrefab;
public Transform casingPrefab;



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





 [Header("键位设置")]
[Tooltip("装弹")]private  KeyCode reloadInputName = KeyCode.R;
[Tooltip("监视动画")]private  KeyCode inspectInputName = KeyCode.I;
  private void Awake() // 获取组件
  {  

    mainCamera=Camera.main;
  
animator=GetComponent<Animator>();
mainAudioSource=GetComponent<AudioSource>();
    playerControllers  =  GetComponentInParent <PlayerController>();
 gunaudio = GetComponent<AudioSource>();  
 //按钮点击
     fireButton.onClick.AddListener(OnFireButtonPressed);  
        aimButton.onClick.AddListener(OnAimButtonPressed);  
        reloadButton.onClick.AddListener(OnReloadButtonPressed);  
        inspectButton.onClick.AddListener(OnInspectButtonPressed);  

 }  


 void Start()  
 {  

 fireButton.onClick.AddListener(OnFireButtonClicked);  
 aimButton.onClick.AddListener(aimButtonClicked);  

sniperingFifilePosition = transform.localPosition;

  muzzleflashLight.enabled=false;
 lightDuration =0.02f;  
 range =300f;  
 bulletForce =100f;  
 bulletleft = bulletMag *10;  
 currentBullets = bulletMag;  
 }  

  void Update()  
 {  
  HandleAiming();
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
SpreadFactor=(isAiming)?0.0001f : 0.03f; //如果是isaming为0.0001f不是为0.2f
 



 if (fireTimer < fireRate)  
 {  
 fireTimer += Time.deltaTime;  
 }  
//分辨行走跑步生成相应的动作
animator.SetBool("Run",playerControllers.isRun);
animator.SetBool("Walk",playerControllers.isWalk);

if(Input.GetKeyDown(inspectInputName)){
animator.SetTrigger("inspect");

}




 }  
//



 public override void GunFire()  
 {  
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

 RaycastHit hit;  
 Vector3 shootDirection = ShootPoint.forward; //仅仅向枪管前方为射击点
  shootDirection += ShootPoint.TransformDirection(new Vector3(UnityEngine.Random.Range(-SpreadFactor, SpreadFactor),UnityEngine.Random.Range(-SpreadFactor, SpreadFactor))); // 将射击方向变为世界空间 
  if (Physics.Raycast(ShootPoint.position, shootDirection, out hit, range))  
 {  
Transform bullet;

bullet=(Transform)Instantiate(bulletPrefab,BulletShootPoint.transform.position,BulletShootPoint.transform.rotation);
bullet.GetComponent<Rigidbody>().velocity=(bullet.transform.forward+shootDirection)*bulletForce;//给子弹朝向加随机偏移量*子弹的力
 Debug.Log("Hit something");  
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

 public override void Aimin() {float currentVelocity= 0f;
for(int i=0; i<crossQuarterImgs.Length;i++){

  crossQuarterImgs[i].gameObject.SetActive(false);
}
mainCamera.fieldOfView=Mathf.SmoothDamp(30,60,ref currentVelocity,0.1f);//平滑摄像头让次逼近视野
mainAudioSource.clip=soundClips.aimSound;
mainAudioSource.Play();
//瞄准改变视野

  }  
 public override void AimOut() { }  
 public override void ReloadAnimation() { 
//设计装弹动画


if(currentBullets>0 && bulletleft>0){
animator.Play("reload_ammo_left",0,0);
Reload();
mainAudioSource.clip=soundClips.reloadSoundAmmotLeft;
mainAudioSource.clip=soundClips.reloadingSound;
 mainAudioSource.PlayOneShot(soundClips.reloadSoundAmmotLeft,6f);
mainAudioSource.Play();
 StartCoroutine(ResetReloadStateAfterAnimation("reload_ammo_left"));  
}else if(currentBullets==0 &&bulletleft>0){
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



 }  
public void  UpdateAmmoUI(){

   Debug.Log($"当前子弹: {currentBullets}, 剩余备用弹: {bulletleft}");   
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

private void OnFireButtonPressed()  
{  
    if (currentBullets > 0)  
    {  
        GunFire();  
    }  
}  

private void OnAimButtonPressed()  
{  
    if (!isreloading)  
    {  
        isAiming = !isAiming; // 切换瞄准状态  
        animator.SetBool("Aim", isAiming);  
        transform.localPosition = isAiming ? sniperingFifileOnPosition : sniperingFifilePosition; // 根据状态调整位置  
    }  
}  

private void OnReloadButtonPressed()  
{  
    if (currentBullets < bulletMag && bulletleft > 0)  
    {  
        ReloadAnimation();  
        isreloading = true; // 设置为正在换弹状态  
    }  
}  

private void OnInspectButtonPressed()  
{  
    animator.SetTrigger("inspect");  
}

 // 按钮点击事件处理方法  
    private void OnFireButtonClicked()  
    {  
GunFire();

        Debug.Log("Fire button clicked!"); // 打印调试信息  
    } 


private void aimButtonClicked(){
    Aimin();
    HandleAiming()  ;
}

 }
