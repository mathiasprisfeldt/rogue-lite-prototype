using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGoalDoor : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            LevelManager.Instance.LoadNextLevel();
        }
    }
}