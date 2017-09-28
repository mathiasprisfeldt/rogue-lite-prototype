using System;
using CharacterController;
using ItemSystem;
using Managers;
using UnityEngine;

[RequireComponent(typeof(ItemHandler))]
public class ItemStealer : MonoBehaviour
{
    private ItemHandler _itemHandler;

    void Start()
    {
        _itemHandler = GetComponent<ItemHandler>();
        _itemHandler.Owner.Hitbox.OnTriggerEnter += HitboxOnOnTriggerEnter;
    }

    private void HitboxOnOnTriggerEnter(Collider2D collider2D)
    {
        var collisionCheck = collider2D.GetComponent<CollisionCheck>();

        if (collisionCheck && 
            collisionCheck.Character.ItemHandler &&
            collisionCheck.Character.ItemHandler.CanStealFrom())
        {
            Debug.Log("ASD");
        }
    }

    void Update()
    {
        if (GameManager.Instance.Player.C.PlayerActions.Attack && _itemHandler.Owner is ActionsController)
        {
            foreach (Collider2D targetCollider in _itemHandler.Owner.Hitbox.Sides.TargetColliders)
            {
                ItemHandler targetItemHandler = targetCollider.GetComponent<CollisionCheck>().Character.ItemHandler;

                if (targetItemHandler.CanStealFrom())
                    _itemHandler.Steal(targetItemHandler);
            }
        }
    }
}
