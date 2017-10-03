﻿using System;
using System.Linq;
using InControl;
using ItemSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI handler for each Item holded by <see cref="ItemUIHandler"/>
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class PlayerUIItem : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private Image _icon;

    [SerializeField]
    private Image _cooldownImage;

    [SerializeField]
    private TextMeshProUGUI _cooldownText;

    [SerializeField]
    private Image _inputImage;

    public Item Item { get; set; }
    public ItemType ItemType { get; set; }
    public ItemUIHandler Owner { get; set; }

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        SetItem(null);
    }

    void Update()
    {
        if (Item && !Item.IsActivationReady)
        {
            _cooldownImage.fillAmount = 1 - (float) Item.CooldownTimer.Normalized;
            _cooldownText.text = Math.Ceiling(Item.CooldownTimer.ReversedClock.TotalSeconds).ToString("N0");
        }
        else
        {
            _cooldownImage.fillAmount = 0;
            _cooldownText.text = String.Empty;
        }
    }

    /// <summary>
    /// Sets the current item of this item icon.
    /// </summary>
    public void SetItem(Item item)
    {
        _inputImage.enabled = item && item.Type != ItemType.Passive;

        if (item)
        {
            _icon.enabled = true;
            _icon.sprite = item.Icon;
        }
        else
            _icon.enabled = false;

        if (item && item.ActivationAction != null)
        {
            ItemUIHandler.ItemInputImage inputImage = Owner.InputImages
                .FirstOrDefault(image => image.Name == item.ActivationAction.Action.Name);

            if (inputImage != null)
                _inputImage.sprite = inputImage.Sprite;
        }

        Item = item;
    }
}
