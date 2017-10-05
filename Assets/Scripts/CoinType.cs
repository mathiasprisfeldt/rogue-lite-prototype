using System.Collections;
using System.Collections.Generic;
using AcrylecSkeleton.Extensions;
using Archon.SwissArmyLib.Events;
using Archon.SwissArmyLib.Utils;
using Managers;
using UnityEngine;

public enum CoinTypes
{
    Gold, Silver, Bronze
}

public class CoinType : MonoBehaviour, TellMeWhen.ITimerCallback
{
    [SerializeField]
    private CoinTypes _coinType;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private float _distanceToMove;

    [SerializeField]
    private float _moveSpeed;

    [SerializeField]
    private float _timeUntilMove;

    [SerializeField]
    private Rigidbody2D _rigigbody;

    [SerializeField]
    private Collider2D _physicCollider;

    private CoinTypes _oldCoinType;
    private bool _movetoPlayer;

    public void Awake()
    {
        TellMeWhen.Seconds(_timeUntilMove,this);
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        if (_coinType != _oldCoinType)
        {
            _oldCoinType = _coinType;
            UpdateAnimator(_coinType);
        }

        if (_movetoPlayer && GameManager.Instance && GameManager.Instance.Player)
        {
                Vector2 direction = transform.position.DirectionTo2D(GameManager.Instance.Player.transform
                                        .position) * _moveSpeed * BetterTime.DeltaTime;
                transform.position += new Vector3(direction.x,direction.y,0);
        }
	}

    private void UpdateAnimator(CoinTypes coinType)
    {
        _animator.SetInteger("CoinType",(int)coinType);
        _animator.SetTrigger("UpdateCoin");
    }

    public void OnTimesUp(int id, object args)
    {
        _movetoPlayer = true;
        if (_rigigbody)
            _rigigbody.gravityScale = 0;
        if (_physicCollider)
            _physicCollider.enabled = false;
    }
}
