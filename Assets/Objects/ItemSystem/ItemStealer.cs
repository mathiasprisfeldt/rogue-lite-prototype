using System;
using System.Linq;
using Archon.SwissArmyLib.Events;
using CharacterController;
using Controllers;
using ItemSystem;
using Managers;
using UnityEngine;

[RequireComponent(typeof(ItemHandler))]
public class ItemStealer : MonoBehaviour, IEventListener<Collider2D>
{
    private GameObject _currPopup;
    private Collider2D _victimCol;
    private Character _currentVictim;
    private ItemHandler _itemHandler;

    [SerializeField]
#pragma warning disable 649
    private GameObject _popupPrefab;
#pragma warning restore 649

    void Start()
    {
        _itemHandler = GetComponent<ItemHandler>();
        _itemHandler.Owner.Hitbox.TriggerExit.AddListener(this);
        _itemHandler.Owner.Hitbox.TriggerStay.AddListener(this);
    }

    void Update()
    {
        if (_currentVictim && !_currentVictim.ItemHandler.Items.Any())
            ForgetVictim();

        //If we're the player, we pressed attack and we got a victim.
        if (_itemHandler.Owner is ActionsController &&
            GameManager.Instance.Player.C.PlayerActions.Interact.WasPressed &&
            _currentVictim)
        {
            if (_itemHandler.Steal(_currentVictim.ItemHandler))
                ForgetVictim();
        }
    }

    public void OnEvent(int eventId, Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Hitbox") || !col.gameObject.CompareTag("Enemy"))
            return;

        switch (eventId)
        {
            case CollisionCheck.ON_TRIGGER_EXIT:
                if (_victimCol == col)
                    ForgetVictim();
                break;
            case CollisionCheck.ON_TRIGGER_STAY:
                var closests = _itemHandler.Owner.Hitbox.Sides.TargetColliders.OrderBy(d => Vector2.Distance(d.transform.position, transform.position));

                Collider2D bestCandidate = closests.FirstOrDefault(d =>
                {
                    ItemHandler handler = d.GetComponent<CollisionCheck>().Character.ItemHandler;
                    if (handler)
                        return handler.CanStealFrom && _itemHandler.CanCarry(handler);

                    return false;
                });

                if (closests.Any() && (bestCandidate && bestCandidate != _victimCol))
                {
                    NewVictim(col);
                    _victimCol = col;
                }

                break;
        }
    }

    private void NewVictim(Collider2D victim)
    {
        var collisionCheck = victim.GetComponent<CollisionCheck>();

        if (collisionCheck &&
            collisionCheck.Character.ItemHandler &&
            collisionCheck.Character.ItemHandler.CanStealFrom)
        {
            ForgetVictim();

            _currentVictim = collisionCheck.Character;
            _currPopup = Instantiate(_popupPrefab, _currentVictim.Origin + _currentVictim.ItemHandler.PopupOffset, Quaternion.identity);
        }
    }

    private void ForgetVictim()
    {
        _victimCol = null;
        _currentVictim = null;
        Destroy(_currPopup);
    }

    void OnDestroy()
    {
        _itemHandler.Owner.Hitbox.TriggerExit.RemoveListener(this);
        _itemHandler.Owner.Hitbox.TriggerStay.RemoveListener(this);
    }
}
