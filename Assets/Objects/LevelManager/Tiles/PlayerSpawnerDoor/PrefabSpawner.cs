using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using Managers;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _prefab;

    [SerializeField]
    private bool _spawn = true;

    private void Start()
    {
        if (GameManager.Instance.Player != null)
            GameManager.Instance.Player.transform.position = transform.position;
        else if (_spawn)
            GameManager.Instance.Player = Instantiate(_prefab, transform.position, Quaternion.identity).GetComponent<PlayerApplication>();
    }
}
