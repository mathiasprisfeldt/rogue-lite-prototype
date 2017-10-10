using System.Collections;
using System.Collections.Generic;
using Managers;
using PrefabDrops;
using TMPro;
using UnityEngine;

public class SimpleShop : MonoBehaviour
{
    [SerializeField]
    private int _cost;

    [SerializeField]
    private PrefabDropper _prefabDropper;

    [SerializeField]
    private TextMeshProUGUI _text;

    private bool _playerIsNear;
    private int _oldCost;

    public void Update()
    {
        if (GameManager.Instance && GameManager.Instance.Player && _prefabDropper)
        {
            if (GameManager.Instance.Player.C.PlayerActions.Attack.WasPressed
                && GameManager.Instance.Gold >= _cost && _playerIsNear)
            {
                _prefabDropper.Drop();
                GameManager.Instance.Gold -= _cost;
            }
        }

        if (_text && _cost != _oldCost)
        {
            _oldCost = _cost;
            _text.text = _cost + "$";
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            _playerIsNear = true;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            _playerIsNear = false;
    }
}
