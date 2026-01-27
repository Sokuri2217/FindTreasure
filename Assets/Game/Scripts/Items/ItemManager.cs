using UnityEngine;

/// <summary>
/// アイテムの効果タイプを定義
/// </summary>
public enum ItemEffectType
{
    None,
    AddDigArea,
    AddDigNum,
    AddDigLimit,
    AddDigPower,
    AddMoveSpeed,
    ChangePhaseTime,
    ItemUpgrade,
    BackTurn,
    ClearCoolTime,

}

/// <summary>
/// 各アイテムデータをScriptableObjectとして定義
/// </summary>
[CreateAssetMenu(menuName = "Item/ItemManager")]
public class ItemManager : ItemBase
{
    [Header("効果設定")]
    public ItemEffectType effectType;

    [Tooltip("数値パラメータ1")]
    public float value1;
    [Tooltip("数値パラメータ2")]
    public float value2;
    [Tooltip("アクティブ数値パラメータ1")]
    public float activeValue1;
    [Tooltip("アクティブ数値パラメータ2")]
    public float activeValue2;
    [Tooltip("フェーズ")]
    public bool item;
    public bool dig;



    // ==============================
    // 効果適用処理
    // ==============================

    /// <summary>
    /// アイテム獲得時（即時効果）
    /// </summary>
    public override void OnGet(PlayerController player, StageUI stageUI)
    {
        switch (effectType)
        {
            case ItemEffectType.BackTurn:
                stageUI.currentTurn -= (int)value1;
                break;
            case ItemEffectType.AddDigNum:
                player.digCurrent += (int)value1;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 常在効果（保持している間に有効）
    /// </summary>
    public override void OnHold(PlayerController player, StageUI stageUI)
    {
        switch (effectType)
        {
            case ItemEffectType.AddDigArea:
                player.dig_width_data += (int)value1 * 2;
                player.dig_height_data += (int)value2 * 2;
                break;
            case ItemEffectType.ChangePhaseTime:
                stageUI.phaseLimit[(int)Phase.ITEM] += value1;
                stageUI.phaseLimit[(int)Phase.DIG] += value1;
                break;
            case ItemEffectType.AddDigLimit:
                player.digLimit += (int)value1;
                break;
            case ItemEffectType.AddDigPower:
                player.digPower += (int)value1;
                break;
            case ItemEffectType.AddMoveSpeed:
                player.originSpeed += (int)value1;
                break;
            case ItemEffectType.ItemUpgrade:
                ItemInstance myInstance = player.inventory.items.Find(x => x.itemBase == this);
                if(myInstance != null)
                {
                    player.inventory.ReduceAllItemsActiveTime((int)value1);
                    player.inventory.ReduceAllItemsCoolTime(-((int)value1));
                }
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 任意発動時（ボタンなどで使用）
    /// </summary>
    public override void OnUse(PlayerController player, StageUI stageUI)
    {
        switch (effectType)
        {
            case ItemEffectType.AddDigArea:
                player.dig_width_data += (int)activeValue1 * 2;
                player.dig_height_data += (int)activeValue2 * 2;
                break;
            case ItemEffectType.BackTurn:
                stageUI.currentTurn -= (int)activeValue1;
                break;
            case ItemEffectType.ChangePhaseTime:
                stageUI.phaseLimit[(int)Phase.ITEM] += activeValue1;
                stageUI.phaseLimit[(int)Phase.DIG] += activeValue1;
                break;
            case ItemEffectType.ClearCoolTime:
                ItemInstance myInstance = player.inventory.items.Find(x => x.itemBase == this);
                if (myInstance != null)
                    player.inventory.ResetOtherItemsCoolTime(myInstance);
                break;
            case ItemEffectType.AddDigNum:
                player.consumedProbability += activeValue1;
                break;
            case ItemEffectType.AddDigLimit:
                player.digLimit += (int)activeValue1;
                break;
            case ItemEffectType.AddDigPower:
                player.digPower += (int)activeValue1;
                break;
            default:
                Debug.Log("このアイテムに『あくてぃぶ』効果はありません");
                break;
        }
    }

    public override void OnActiveDelete(PlayerController player, StageUI stageUI)
    {
        switch (effectType)
        {
            case ItemEffectType.AddDigArea:
                player.dig_width_data -= (int)activeValue1 * 2;
                player.dig_height_data -= (int)activeValue2 * 2;
                break;
            case ItemEffectType.ChangePhaseTime:
                stageUI.phaseLimit[(int)Phase.ITEM] -= activeValue1;
                stageUI.phaseLimit[(int)Phase.DIG] -= activeValue1;
                break;
            case ItemEffectType.AddDigNum:
                player.consumedProbability -= activeValue1;
                break;
            case ItemEffectType.AddDigLimit:
                player.digLimit -= (int)activeValue1;
                break;
            case ItemEffectType.AddDigPower:
                player.digPower -= (int)activeValue1;
                break;
            default:
                Debug.Log("このアイテムに『あくてぃぶ』効果はありません");
                break;
        }
    }
}
