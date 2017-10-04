using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CoinTypes
{
    Gold, Silver, Bronze
}

public class CoinType : MonoBehaviour
{
    [SerializeField]
    private CoinTypes _coinType;

    [SerializeField]
    private Animator _animator;

    private CoinTypes _oldCoinType;
	
    private 

	// Update is called once per frame
	void Update ()
    {
        if (_coinType != _oldCoinType)
        {
            _oldCoinType = _coinType;
            UpdateAnimator(_coinType);
        }	
	}

    private void UpdateAnimator(CoinTypes coinType)
    {
        _animator.SetInteger("CoinType",(int)coinType);
        _animator.SetTrigger("UpdateCoin");
    }
}
