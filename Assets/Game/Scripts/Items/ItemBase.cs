using UnityEngine;

[CreateAssetMenu(menuName = "Item/ItemBase")]
public class ItemBase : ScriptableObject
{
    public string itemName;
    [TextArea(2, 5)] public string[] description;
    public Sprite icon;

    [Header("レアリティー")]
    public Rarity rarity;
    [Tooltip("レアになる確率")]
    public float randomRarity;
    [Tooltip("レアリティー補正")]
    public float rarityEffect;
    [Tooltip("レア補正")]
    public float rareEffect;
    [Tooltip("レアリティーごとにアイコンを設定")]
    public Sprite[] rarityIcon = new Sprite[2];

    public int originDuration;
    public int originCoolTime;

    //レアリティーを設定
    public virtual void SetRarity(PlayerController player, StageUI stageUI) { }
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
