using System.Collections;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;

public class MoneyPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    private int _moneyValue;

	// Update is called once per frame
	void Update ()
    {
        if (GameManager.Instance && GameManager.Instance.Gold != _moneyValue)
        {
            _moneyValue = GameManager.Instance.Gold;
            _text.text = _moneyValue + "$";
        }
	}
}
