using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    public ItemBase itemBase;

    public int duration;
    public int coolTime;
    public int coolTimeTurn;
    public int useActiveTurn;

    public bool isUseActive;
    public bool isCoolDown;

    public ItemInstance(ItemBase itemBase)
    {
        this.itemBase = itemBase;
        duration = itemBase.originDuration;
        duration = itemBase.originCoolTime;
        isUseActive = false;
        isCoolDown = false;
    }
}