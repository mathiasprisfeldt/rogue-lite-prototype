using System;
using InControl;
using Managers;
using UnityEngine;
using UnityEngine.Events;

public class PositionTween : MonoBehaviour
{
    [Serializable]
    public class InputChange : UnityEvent<BindingSourceType> { }

    private float _timer;

    [SerializeField]
    private Transform _target;

    [SerializeField]
    private AnimationCurve _xCurve;

    [SerializeField]
    private AnimationCurve _yCurve;

    [SerializeField]
    private Vector2 _xInterval;

    [SerializeField]
    private Vector2 _yInterval;

    [SerializeField]
    private float _duration;

    void Update()
    {
        _target.localPosition = new Vector2(
            Mathf.Lerp(_xInterval.x, _xInterval.y, _xCurve.Evaluate(_timer)), 
            Mathf.Lerp(_yInterval.x, _yInterval.y, _yCurve.Evaluate(_timer)));

        //Reset back to 0 if finished with one loop.
        _timer += Time.deltaTime / _duration;
    }
}
