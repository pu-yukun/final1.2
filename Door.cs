using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunTemple
{
    public class Door : MonoBehaviour
    {
        public bool IsLocked = false;
        public bool DoorClosed = true;
        public float OpenRotationAmount = 90;
        public float RotationSpeed = 1f;
        public float MaxDistance = 3.0f;
        public string playerTag = "Player";
        private Collider DoorCollider;

        private GameObject Player;
        private Camera Cam;
        private CursorManager cursor;

        Vector3 StartRotation;
        float StartAngle = 0;
        float EndAngle = 0;
        float LerpTime = 1f;
        float CurrentLerpTime = 0;
        bool Rotating;

        private bool scriptIsEnabled = true;

        void Start()
        {
            StartRotation = transform.localEulerAngles;
            DoorCollider = GetComponent<BoxCollider>();

            if (!DoorCollider)
            {
                Debug.LogWarning($"{GetType().Name}.cs on {gameObject.name}: Door has no collider!", gameObject);
                scriptIsEnabled = false;
                return;
            }

            Player = GameObject.FindGameObjectWithTag(playerTag);
            if (!Player)
            {
                Debug.LogWarning($"{GetType().Name}.cs on {gameObject.name}: No object tagged with {playerTag} found!", gameObject);
                scriptIsEnabled = false;
                return;
            }

            Cam = Camera.main;
            if (!Cam)
            {
                Debug.LogWarning($"{GetType().Name}.cs on {gameObject.name}: No MainCamera found!", gameObject);
                scriptIsEnabled = false;
                return;
            }

            cursor = CursorManager.instance;
            if (cursor != null)
            {
                cursor.SetCursorToDefault();
            }

            Debug.Log($"{gameObject.name} initialized successfully. DoorClosed: {DoorClosed}, IsLocked: {IsLocked}");
        }

        void Update()
        {
            if (!scriptIsEnabled) return;

            if (Rotating)
            {
                Rotate();
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Debug.Log("Mouse0 pressed. Attempting to open door...");
                TryToOpen();
            }

            if (cursor != null)
            {
                CursorHint();
            }
        }

        void TryToOpen()
        {
            float distance = Vector3.Distance(transform.position, Player.transform.position);
            Debug.Log($"Player distance to door: {distance} (MaxDistance: {MaxDistance})");

            if (distance <= MaxDistance)
            {
                Ray ray = Cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                RaycastHit hit;

                Debug.DrawRay(ray.origin, ray.direction * MaxDistance, Color.green, 2f); // 可视化射线

                if (DoorCollider.Raycast(ray, out hit, MaxDistance))
                {
                    Debug.Log($"Raycast hit door: {hit.collider.name}");
                    if (!IsLocked)
                    {
                        Debug.Log("Door is unlocked. Activating door...");
                        Activate();
                    }
                    else
                    {
                        Debug.Log("Door is locked!");
                    }
                }
                else
                {
                    Debug.Log("Raycast did not hit door collider.");
                }
            }
            else
            {
                Debug.Log("Player is too far from the door.");
            }
        }

        void CursorHint()
        {
            float distance = Vector3.Distance(transform.position, Player.transform.position);
            if (distance <= MaxDistance)
            {
                Ray ray = Cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                RaycastHit hit;

                if (DoorCollider.Raycast(ray, out hit, MaxDistance))
                {
                    if (!IsLocked)
                    {
                        cursor.SetCursorToDoor();
                    }
                    else
                    {
                        cursor.SetCursorToLocked();
                    }
                }
                else
                {
                    cursor.SetCursorToDefault();
                }
            }
        }

        public void Activate()
        {
            if (DoorClosed)
            {
                Debug.Log("Opening door...");
                Open();
            }
            else
            {
                Debug.Log("Closing door...");
                Close();
            }
        }

        void Rotate()
        {
            CurrentLerpTime += Time.deltaTime * RotationSpeed;
            if (CurrentLerpTime > LerpTime)
            {
                CurrentLerpTime = LerpTime;
            }

            float _Perc = CurrentLerpTime / LerpTime;
            float _Angle = CircularLerp.Clerp(StartAngle, EndAngle, _Perc);
            transform.localEulerAngles = new Vector3(transform.eulerAngles.x, _Angle, transform.eulerAngles.z);

            Debug.Log($"Rotating door: Current Angle = {_Angle}, Target Angle = {EndAngle}, Progress = {_Perc * 100}%");

            if (CurrentLerpTime == LerpTime)
            {
                Rotating = false;
                DoorCollider.enabled = true;
                Debug.Log("Door rotation complete.");
            }
        }

        void Open()
        {
            DoorCollider.enabled = false;
            DoorClosed = false;
            StartAngle = transform.localEulerAngles.y;

            // 动态调整开门方向
            Vector3 dirToPlayer = (Player.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, dirToPlayer);
            EndAngle = (dot > 0) ? StartRotation.y - OpenRotationAmount : StartRotation.y + OpenRotationAmount;

            CurrentLerpTime = 0;
            Rotating = true;

            Debug.Log($"Door opening: StartAngle = {StartAngle}, EndAngle = {EndAngle}");
        }

        void Close()
        {
            DoorCollider.enabled = false;
            DoorClosed = true;
            StartAngle = transform.localEulerAngles.y;
            EndAngle = StartRotation.y;
            CurrentLerpTime = 0;
            Rotating = true;

            Debug.Log($"Door closing: StartAngle = {StartAngle}, EndAngle = {EndAngle}");
        }
    }
}