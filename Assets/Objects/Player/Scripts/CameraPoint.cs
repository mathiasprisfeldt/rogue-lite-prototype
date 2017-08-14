using Controllers;
using UnityEngine;
using Action = CharacterController.Action;

public class CameraPoint : MonoBehaviour
{
    [SerializeField]
    private Action _action;

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
	    }
        
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (_target == null)
            return;
	    var targetX = _target.transform.localScale.x > 0 ? _xPosition : -_xPosition;
	    var targetY = 0f;
	    var inRightState = _action.State == CharacterState.Idle || _action.State == CharacterState.Moving || _action.LastUsedVerticalAbility == CharacterController.Ability.LedgeHanging;

	    if (_action.App.C.PlayerActions != null && inRightState
            && (_action.App.C.PlayerActions.DeadZoneUp(_peekDeadZone) || _action.App.C.PlayerActions.DeadZoneDown(_peekDeadZone)))
	    {
	        if (_peekTImer > 0)
	            _peekTImer -= Time.fixedDeltaTime;
	        else
	        {
	            var dir = _action.App.C.PlayerActions.DeadZoneUp(_peekDeadZone) ? 1 :-1;
                targetY = dir * _peekAmount;
            }
	        
	    }
	    else
	        _peekTImer = _durationUntilPeek;

        if (transform.localPosition != new Vector3(targetX,targetY,transform.localPosition.z))
            transform.localPosition = new Vector3(targetX, targetY, transform.localPosition.z);
	}
}
