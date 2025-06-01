using UnityEngine;
using UnityEngine.AI;

public class hostagebehavior : MonoBehaviour
{
    public enum HostageState { Sit, Wait, Walk }
    
    [Header("跟随设置")]
    public float followDistance = 3f; // 开始跟随的距离
    public float stoppingDistance = 2f; // 停止移动的距离
    public float speed = 3.5f; // 移动速度

    [Header("状态显示")]
    [SerializeField] private HostageState _currentState;
    public HostageState CurrentState => _currentState;

    private Transform _player;
    private NavMeshAgent _agent;
    private Animator _animator;

    void Start()
    {
        // 初始化组件
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        
        // 配置NavMeshAgent
        _agent.speed = speed;
        _agent.stoppingDistance = stoppingDistance;
        _agent.autoBraking = true;




        // 初始状态为 Sit
        SetState(HostageState.Sit);
        
        // 订阅对话结束事件
        hostageui.OnDialogueEnd += OnDialogueEndHandler;

        Debug.Log("hostagebehavior 初始化完成");
    }

    void OnDestroy()
    {
        // 取消事件订阅
        hostageui.OnDialogueEnd -= OnDialogueEndHandler;
    }

    void Update()
    {
        // 动态查找玩家
        if (_player == null)
        {
            _player = gamemanager.Instance?.player?.transform;
            if (_player == null)
            {
                Debug.LogWarning("未找到玩家，请确保玩家已生成且 gamemanager 已正确初始化");
                return;
            }
            else
            {
                Debug.Log("成功找到玩家: " + _player.name);
            }
        }

        // 如果当前状态是 Sit，不执行任何逻辑
        if (_currentState == HostageState.Sit) return;

        // 计算与玩家的距离
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        Debug.Log($"与玩家的距离: {distanceToPlayer}");

        // 根据距离切换状态
        if (_currentState == HostageState.Walk && distanceToPlayer <= stoppingDistance)
        {
            Debug.Log("距离玩家过近，切换到 Wait 状态");
            SetState(HostageState.Wait);
        }
        else if (_currentState == HostageState.Wait && distanceToPlayer > followDistance)
        {
            Debug.Log("距离玩家过远，切换到 Walk 状态");
            SetState(HostageState.Walk);
        }

        // 如果状态是 Walk，持续跟随玩家
        if (_currentState == HostageState.Walk)
        {
            Debug.Log("正在跟随玩家");
            _agent.SetDestination(_player.position);
        }
    }

    private void SetState(HostageState newState)
    {
        if (_currentState == newState) return;

        Debug.Log($"状态切换: {_currentState} -> {newState}");
        _currentState = newState;
        UpdateAnimation();
        HandleStateBehavior(newState);
    }

    private void UpdateAnimation()
    {
        // 设置动画参数
        _animator.SetBool("IsWalking", _currentState == HostageState.Walk);
        _animator.SetBool("IsWaiting", _currentState == HostageState.Wait);
    }

    private void HandleStateBehavior(HostageState state)
    {
        switch (state)
        {
            case HostageState.Sit:
                _agent.isStopped = true; // 停止移动
                break;
            case HostageState.Wait:
                _agent.isStopped = true; // 停止移动
                break;
            case HostageState.Walk:
                _agent.isStopped = false; // 开始移动
                break;
        }
    }

    private void OnDialogueEndHandler()
    {
        Debug.Log("对话结束，开始检测玩家距离");

        // 动态查找玩家
        if (_player == null)
        {
            _player = gamemanager.Instance?.player?.transform;
            if (_player == null)
            {
                Debug.LogWarning("未找到玩家，无法切换状态");
                return;
            }
        }

        // 计算与玩家的距离
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        Debug.Log($"对话结束时与玩家的距离: {distanceToPlayer}");

        // 根据距离切换状态
        if (distanceToPlayer <= stoppingDistance)
        {
            Debug.Log("距离玩家过近，切换到 Wait 状态");
            SetState(HostageState.Wait);
        }
        else if (distanceToPlayer > followDistance)
        {
            Debug.Log("距离玩家过远，切换到 Walk 状态");
            SetState(HostageState.Walk);
        }
    }



    private void FaceToPlayer()  
    {  
        // 计算方向并旋转人质以面向玩家  
        Vector3 direction = (_player.position - transform.position).normalized;  
        Quaternion lookRotation = Quaternion.LookRotation(direction);  
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // 平滑旋转  
    }  
}