using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPoint : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    private float _xPosition;

	// Use this for initialization
	void Start ()
	{
	    _xPosition = transform.localPosition.x;
	}
	
	// Update is called once per frame
	void Update ()
	{
        if(_target == null)
            return;
	    var targetX = _target.transform.localScale.x > 0 ? _xPosition : -_xPosition;
        if(targetX != transform.localPosition.x)
            transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x,targetX,.1f),transform.localPosition.y);
	}
}
