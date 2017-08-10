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

    private float _xPosition;
    private float _startYPosition;
    private float _peekTimer;

	// Use this for initialization
	void Start ()
	{
	    _xPosition = transform.localPosition.x;
	    _peekTimer = _durationUntilPeek;
	}
	
	// Update is called once per frame
	void Update ()
	{
        if(_target == null)
            return;
	    var targetX = _target.transform.localScale.x > 0 ? _xPosition : -_xPosition;
	    if (targetX != transform.localPosition.x)
	        transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, targetX, .02f), transform.localPosition.y);

	    if ((_playerMovement.OnGround ||_playerMovement.Hanging) && (_playerMovement.App.C.PlayerActions.Up.IsPressed ||
	        _playerMovement.App.C.PlayerActions.Down.IsPressed))
	    {
            if(_peekTimer > 0)
	            _peekTimer -= Time.deltaTime;
	        else
	        {
	            var dir = _playerMovement.App.C.PlayerActions.Up.IsPressed ? 1 : -1;
                if (transform.localPosition.y != _startYPosition + dir * _peekAmount)
                    transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, _startYPosition + dir * _peekAmount, .1f));

            }
	    }
	    else
	    {
            _peekTimer = _durationUntilPeek;
            if(transform.localPosition.y != _startYPosition)
                transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, _startYPosition, .1f));
        }
	        
	}
}
