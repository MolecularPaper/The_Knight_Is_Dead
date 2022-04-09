using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemObserver
{
    public void ItemUpdate(Item item);
}

public interface IItemObservable
{
    public void Subscribe(IItemObserver observer);

    public void Unsubscribe(IItemObserver observer);

    public void ItemUpdate();
}

[System.Serializable]
public class ItemInfo
{
    public string itemName = "";
    public ulong count = 0;

    public ItemInfo() { }

    public ItemInfo(ItemInfo itemInfo)
    {
        this.itemName = itemInfo.itemName;
        this.count = itemInfo.count;
    }
}

[System.Serializable]
public class Item : ItemInfo, IItemObservable
{
    private delegate void ItemUpdateDel(Item item);
    private ItemUpdateDel itemUpdateDel;
    public ulong Count {
        get => count;
        set {
            count = value;
            ItemUpdate();
        }
    }

    public Item(string itemName, ulong count)
    {
        this.itemName = itemName;
        Count = count;
    }

    public Item(ItemInfo itemInfo)
    {
        this.itemName = itemInfo.itemName;
        Count = itemInfo.count;
    }

    public void ItemUpdate()
    {
        if(itemUpdateDel != null) itemUpdateDel.Invoke(this);
    }

    public void Subscribe(IItemObserver observer)
    {
        if (observer == null)
            throw new System.NullReferenceException();

        itemUpdateDel += observer.ItemUpdate;
    }

    public void Unsubscribe(IItemObserver observer)
    {
        if (observer == null)
            throw new System.NullReferenceException();

        itemUpdateDel -= observer.ItemUpdate;
    }
}
