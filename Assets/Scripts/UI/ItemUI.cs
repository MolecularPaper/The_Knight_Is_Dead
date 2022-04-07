using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemUI : MonoBehaviour, IItemObserver
{
    [SerializeField] private string itemName;
    [SerializeField] private TextMeshProUGUI count;

    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerCTRL playerCTRL = player.GetComponent<PlayerCTRL>();
        playerCTRL.GetItem(itemName).Subscribe(this);
    }

    public void ItemUpdate(Item item)
    {
        count.text = item.Count.ToString();
    }
}
