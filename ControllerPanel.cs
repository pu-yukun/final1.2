using System;
using System.Collections.Generic;
using UnityEngine;







public class ControllerPanel : MonoBehaviour 
{
    [Header("Audio Settings")]
    public AudioSource MusicSound;

    [Header("Key Bindings")]
    [SerializeField] KeyCode SpeedUp = KeyCode.Space;
    [SerializeField] KeyCode SpeedDown = KeyCode.C;
    [SerializeField] KeyCode Forward = KeyCode.W;
    [SerializeField] KeyCode Back = KeyCode.S;
    [SerializeField] KeyCode Left = KeyCode.A;
    [SerializeField] KeyCode Right = KeyCode.D;
    [SerializeField] KeyCode TurnLeft = KeyCode.Q;
    [SerializeField] KeyCode TurnRight = KeyCode.E;
    [SerializeField] KeyCode EnterHelicopter = KeyCode.L; // 新增关键键
    [SerializeField] KeyCode MusicToggle = KeyCode.M;

    // 显式定义按键与枚举的映射关系
    private Dictionary<KeyCode, PressedKeyCode> keyMapping;

    public event Action<PressedKeyCode[]> KeyPressed;

    private void Awake()
    {
        InitializeKeyMapping();
    }

    private void InitializeKeyMapping()
    {
        keyMapping = new Dictionary<KeyCode, PressedKeyCode>
        {
            {SpeedUp, PressedKeyCode.SpeedUpPressed},
            {SpeedDown, PressedKeyCode.SpeedDownPressed},
            {Forward, PressedKeyCode.ForwardPressed},
            {Back, PressedKeyCode.BackPressed},
            {Left, PressedKeyCode.LeftPressed},
            {Right, PressedKeyCode.RightPressed},
            {TurnLeft, PressedKeyCode.TurnLeftPressed},
            {TurnRight, PressedKeyCode.TurnRightPressed},
            {EnterHelicopter, PressedKeyCode.EnterHelicopter} // 添加映射
        };
    }

    void Update()
    {
        HandleInput();
        HandleMusicControl();
    }

    private void HandleInput()
    {
        var pressedKeys = new List<PressedKeyCode>();
        
        foreach(var kvp in keyMapping)
        {
            if(Input.GetKey(kvp.Key))
                pressedKeys.Add(kvp.Value);
        }

        if(pressedKeys.Count > 0)
            KeyPressed?.Invoke(pressedKeys.ToArray());
    }

    private void HandleMusicControl()
    {
        if(Input.GetKeyDown(MusicToggle))
        {
            if(MusicSound.isPlaying)
            {
                MusicSound.Stop();
            }
            else
            {
                MusicSound.volume = 1f;
                MusicSound.Play();
            }
        }
    }






}