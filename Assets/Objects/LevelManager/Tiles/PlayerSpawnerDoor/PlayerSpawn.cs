using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerPrefab;

    [SerializeField]
    private bool _spawnPlayer = true;

    private void Start()
    {
        if(_spawnPlayer)
            Instantiate(_playerPrefab, transform.position, Quaternion.identity);
    }
}
