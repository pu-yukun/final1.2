using UnityEngine;

namespace SunTemple
{
    public class PlayerDoorInteraction : MonoBehaviour
    {
        public KeyCode interactionKey = KeyCode.LeftControl;
        public float interactionDistance = 3f;
        private Door nearestDoor;

        void Update()
        {
            // 持续寻找最近的门
            FindNearestDoor();

            if (Input.GetKeyDown(interactionKey))
            {
                TryInteractWithDoor();
            }
        }

        void FindNearestDoor()
        {
            Door[] allDoors = FindObjectsOfType<Door>();
            float closestDistance = Mathf.Infinity;
            nearestDoor = null;

            foreach (Door door in allDoors)
            {
                float distance = Vector3.Distance(transform.position, door.transform.position);
                if (distance <= interactionDistance && distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestDoor = door;
                }
            }
        }

        void TryInteractWithDoor()
        {
            if (nearestDoor != null && !nearestDoor.IsLocked)
            {
                nearestDoor.Activate();
                Debug.Log($"已{(nearestDoor.DoorClosed ? "关闭" : "打开")}门：{nearestDoor.name}");
            }
        }
    }
}