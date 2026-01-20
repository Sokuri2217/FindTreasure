using UnityEngine;

[CreateAssetMenu(menuName = "Item/ItemBase")]
public class ItemBase : ScriptableObject
{
    public string itemName;
    [TextArea(2, 5)] public string[] description;
    public Sprite icon;

    public int originDuration;
    public int originCoolTime;

    //Šl“¾Œø‰Ê
    public virtual void OnGet(PlayerController player, StageUI stageUI) { }
    //íİŒø‰Ê
    public virtual void OnHold(PlayerController player, StageUI stageUI) { }
    //”CˆÓŒø‰Ê
    public virtual void OnUse(PlayerController player, StageUI stageUI) { }
    //”jŠü
    public virtual void OnHoldDelete(PlayerController player, StageUI stageUI) { }
    public virtual void OnActiveDelete(PlayerController player, StageUI stageUI) { }
}

public enum Item
{
    GET,
    HOLD,
    ACTIVE,
}
