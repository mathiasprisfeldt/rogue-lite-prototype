using System;
using InControl;
using ItemSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI handler for each Item holded by <see cref="ItemUIHandler"/>
/// </summary>
public class PlayerUIItem : MonoBehaviour
{

    [SerializeField]
    private Image _cooldownImage;

    [SerializeField]
    private TextMeshProUGUI _cooldownText;

    [SerializeField]
    private Image _inputImage;

    public Item Item { get; set; }

    void Start()
    {
        if (Item.Type == ItemType.Passive)
        {
            _inputImage.enabled = false;
        }
    }

    void Update()
    {
        if (!Item.IsActivationReady)
        {
            _cooldownImage.fillAmount = 1 - (float) Item.CooldownTimer.Normalized;
            _cooldownText.text = Item.CooldownTimer.ReversedClock.TotalSeconds.ToString("N0");
        }
        else
        {
            _cooldownImage.fillAmount = 0;
            _cooldownText.text = String.Empty;
        }
    }

}
