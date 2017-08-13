using System;
using System.Collections;
using System.Collections.Generic;
using CharacterController;
using UnityEngine;

public class CameraPoint : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement _playerMovement;

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
	void Start ()
	{
	    _xPosition = transform.localPosition.x;
	    GameObject mainCam = Camera.main.gameObject;
	    if (mainCam)
	    {
	        FollowTransform ft = mainCam.GetComponent<FollowTransform>();
	        ft.Target = gameObject.transform;
	        ft.transform.position = transform.position;
	    }
        
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (_target == null)
            return;
	    var targetX = _target.transform.localScale.x > 0 ? _xPosition : -_xPosition;
	    var targetY = 0f;

	    if (_playerMovement.App.C.PlayerActions != null && (_playerMovement.App.C.PlayerActions.DeadZoneUp(_peekDeadZone) || _playerMovement.App.C.PlayerActions.DeadZoneDown(_peekDeadZone)))
	    {
	        if (_peekTImer > 0)
	            _peekTImer -= Time.fixedDeltaTime;
	        else
	        {
	            var dir = _playerMovement.App.C.PlayerActions.DeadZoneUp(_peekDeadZone) ? 1 :-1;
                targetY = dir * _peekAmount;
            }
	        
	    }
	    else
	        _peekTImer = _durationUntilPeek;

        if (transform.localPosition != new Vector3(targetX,targetY,transform.localPosition.z))
            transform.localPosition = new Vector3(targetX, targetY, transform.localPosition.z);
	        
	}
}
