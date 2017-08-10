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

   	
	void FixedUpdate ()
	{
	    if (_acive && _target)
	    {
	        float posX = Mathf.SmoothDamp(transform.position.x, _target.position.x, ref velocity.x, _smoothX);
	        float posY = Mathf.SmoothDamp(transform.position.y, _target.position.y, ref velocity.y, _smoothY);

            transform.position = new Vector3(posX,posY,transform.position.z);

	    }
	}
}
