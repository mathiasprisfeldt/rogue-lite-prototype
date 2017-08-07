using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour {

    [SerializeField]
    private bool _acive = true;

    [SerializeField]
    private Transform _target;
    	
	void Update ()
    {
        if (_acive && _target)
            transform.position = new Vector3(_target.position.x, _target.position.y,transform.position.z);
	}
}
