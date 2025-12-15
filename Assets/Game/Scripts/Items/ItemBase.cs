using UnityEngine;

[CreateAssetMenu(fileName = "ItemBase", menuName = "Scriptable Objects/ItemBase")]
public class ItemBase : ScriptableObject
{
    public string itemName;
    [TextArea(2, 5)] public string[] description;
    public Sprite icon;
    public bool isUseActive;

    //Šl“¾Œø‰Ê
    public virtual void OnGet(PlayerController player) { }
    //íİŒø‰Ê
    public virtual void OnHold(PlayerController player) { }
    //”CˆÓŒø‰Ê
    public virtual void OnUse(PlayerController player, StageUI stageUI) { }
    //”jŠü
    public virtual void OnHoldDelete(PlayerController player) { }
    public virtual void OnActiveDelete(PlayerController player) { }
    //ƒ^[ƒ“Œo‰ß
    public virtual void TurnCount(PlayerController player, StageUI stageUI) { }
}

public enum Item
{
    GET,
    HOLD,
    ACTIVE,
}
