using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using Managers;
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
            GameManager.Instance.Player = Instantiate(_playerPrefab, transform.position, Quaternion.identity).GetComponent<PlayerApplication>();
    }
}
