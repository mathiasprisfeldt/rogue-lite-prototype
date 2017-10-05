using System;
using Archon.SwissArmyLib.Utils;
using CharacterController;
using Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraPoint : MonoBehaviour
{
    [SerializeField]
    private ActionsController _actionsController;

    [SerializeField]
    private Transform _target;

    [SerializeField]
    private float _peekAmount;

    [SerializeField]
    private float _durationUntilPeek;

    [SerializeField]
    private float _peekDeadZone;

    private float _xPosition;
    private float _peekTImer;

    // Use this for initialization
    private void Start()
    {
        _xPosition = transform.localPosition.x;
        GetCamera();
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode loadSceneMode)
    {
        GetCamera();
    }

    private void GetCamera()
    {
        GameObject mainCam = Camera.main.gameObject;
        if (mainCam)
        {
            FollowTransform ft = mainCam.GetComponent<FollowTransform>();
            ft.Target = gameObject.transform;
            ft.transform.position = transform.position;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (_target == null)
            return;
        var targetX = _target.transform.localScale.x > 0 ? _xPosition : -_xPosition;
        var targetY = 0f;
        var inRightState = _actionsController.State == CharacterState.Idle ||
                           _actionsController.State == CharacterState.Moving ||
                           _actionsController.LastUsedVerticalMoveAbility ==
                           CharacterController.MoveAbility.LedgeHanging;

        if (_actionsController.App.C.PlayerActions != null && inRightState
            &&
            (_actionsController.App.C.PlayerActions.DeadZoneUp(_peekDeadZone) ||
             _actionsController.App.C.PlayerActions.DeadZoneDown(_peekDeadZone)))
        {
            if (_peekTImer > 0)
                _peekTImer -= BetterTime.FixedDeltaTime;
            else
            {
                var dir = _actionsController.App.C.PlayerActions.DeadZoneUp(_peekDeadZone) ? 1 : -1;
                targetY = dir*_peekAmount;
            }

        }
        else
            _peekTImer = _durationUntilPeek;

        if (transform.localPosition != new Vector3(targetX, targetY, transform.localPosition.z))
            transform.localPosition = new Vector3(targetX, targetY, transform.localPosition.z);
    }

    public void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
