// SceneATrigger.cs
using UnityEngine;
using System.Collections;

public class SceneATrigger : MonoBehaviour
{
    [Header("UI Control")]
    public float checkInterval = 0.5f;

    private GameObject _player;
    private PlayerMissionController _missionController;

    void Start()
    {
        StartCoroutine(FindPlayerRoutine());
    }

    private IEnumerator FindPlayerRoutine()
    {
        while (true)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            if (_player != null)
            {
                _missionController = _player.GetComponent<PlayerMissionController>();
                if (_missionController != null)
                {
                    ExecuteSceneLogic();
                    yield break;
                }
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void ExecuteSceneLogic()
    {
        // UI控制
        _missionController.hostageAppearCanvas.SetActive(false);
        _missionController.mission2Canvas.SetActive(false);
        _missionController.mission3Canvas.SetActive(true);

        // 任务控制
        _missionController.SetMissionState(false, true);
        
        Debug.Log("<color=green>[Scene Trigger]</color> 场景三UI和任务已调整");
    }
}