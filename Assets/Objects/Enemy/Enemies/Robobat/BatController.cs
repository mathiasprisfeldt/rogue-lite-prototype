using Controllers;
using ItemSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonoBehaviour
{
    [SerializeField]
    private List<Item> _randomItems = new List<Item>();

    private void Awake()
    {
        GetComponent<Character>().ItemHandler.ItemsAtStart.Add(
            _randomItems[Random.Range(0, _randomItems.Count)]);
    }
}
