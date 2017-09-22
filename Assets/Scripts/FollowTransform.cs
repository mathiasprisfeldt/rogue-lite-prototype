using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour {

    [SerializeField]
    private bool _acive = true;

    [SerializeField]
    private Transform _target;

    [SerializeField]
    private float _smoothX;

    [SerializeField]
    private float _smoothY;

    private Vector2 velocity;

    public Transform Target
    {
        get { return _target; }
        set { _target = value; }
    }


    void FixedUpdate ()
	{
	    if (_acive && Target)
	    {
	        float posX = Mathf.SmoothDamp(transform.position.x, Target.position.x, ref velocity.x, _smoothX);
	        float posY = Mathf.SmoothDamp(transform.position.y, Target.position.y, ref velocity.y, _smoothY);

            transform.position = new Vector3(posX,posY,transform.position.z);

	    }
	}
}
