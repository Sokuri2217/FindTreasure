using UnityEngine;

public enum Rarity
{
    Normal,
    Rare
}

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
        if (itemBase == null) return;

        this.itemBase = itemBase;
        //ÉåÉAÇ…ê›íË
        if (Random.Range(0, 100.0f) <= itemBase.randomRarity)
        {
            itemBase.rarity = Rarity.Rare;
            itemBase.rarityEffect = itemBase.rareEffect;
            itemBase.icon = itemBase.rarityIcon[(int)Rarity.Rare];
        }
        //ÉmÅ[É}ÉãÇ…ê›íË
        else
        {
            itemBase.rarity = Rarity.Normal;
            itemBase.rarityEffect = 0.0f;
            itemBase.icon = itemBase.rarityIcon[(int)Rarity.Normal];
        }
        duration = itemBase.originDuration;
        coolTime = itemBase.originCoolTime;
        isUseActive = false;
        isCoolDown = false;
    }
}