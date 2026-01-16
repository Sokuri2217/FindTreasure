using UnityEngine;

[CreateAssetMenu(fileName = "ItemBase", menuName = "Scriptable Objects/ItemBase")]
public class ItemBase : ScriptableObject
{
    public string itemName;
    [TextArea(2, 5)] public string[] description;
    public Sprite icon;
    public bool isUseActive;
    public bool isCoolDown;
    [Tooltip("持続ターン数（0なら常在効果）")]
    public int originDuration;
    public int duration;
    public int useActiveTurn;
    [Tooltip("クールタイム")]
    public int originCoolTime;
    public int coolTime;
    public int coolTimeTurn;

    //獲得時効果
    public virtual void OnGet(PlayerController player, StageUI stageUI) { }
    //常在効果
    public virtual void OnHold(PlayerController player, StageUI stageUI) { }
    //任意効果
    public virtual void OnUse(PlayerController player, StageUI stageUI) { }
    //破棄
    public virtual void OnHoldDelete(PlayerController player, StageUI stageUI) { }
    public virtual void OnActiveDelete(PlayerController player, StageUI stageUI) { }
}

public enum Item
{
    GET,
    HOLD,
    ACTIVE,
}
