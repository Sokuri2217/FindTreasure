using UnityEngine;

/// <summary>
/// アイテムの効果タイプを定義
/// </summary>
public enum ItemEffectType
{
    None,
    AddPower1,
    AddDigArea,
    AddLimitTurn,
    AddLimit1Turn3Cool2,
    AddUse1,
    AddSpeed5SubTime10
}

/// <summary>
/// 各アイテムデータをScriptableObjectとして定義
/// </summary>
[CreateAssetMenu(fileName = "ItemBase", menuName = "Scriptable Objects/ItemManager")]
public class ItemManager : ItemBase
{
    [Header("効果設定")]
    public ItemEffectType effectType;

    [Tooltip("数値パラメータ1")]
    public float value1;
    [Tooltip("数値パラメータ2")]
    public float value2;
    [Tooltip("数値パラメータ3")]
    public float activeValue1;
    [Tooltip("数値パラメータ4")]
    public float activeValue2;

    [Tooltip("持続ターン数（0なら常在効果）")]
    public int duration;
    [Tooltip("アイテムを使用したターン")]
    public int activeItemTurn;
    [Tooltip("クールタイム")]
    public int coolTime;

    // ==============================
    // 効果適用処理
    // ==============================

    /// <summary>
    /// アイテム獲得時（即時効果）
    /// </summary>
    public override void OnGet(PlayerController player)
    {
        //switch (effectType)
        //{
            
        //}
    }

    /// <summary>
    /// 常在効果（保持している間に有効）
    /// </summary>
    public override void OnHold(PlayerController player)
    {
        switch (effectType)
        {
            case ItemEffectType.AddPower1:
                player.digPower += (int)value1;
                break;
            case ItemEffectType.AddDigArea:
                player.dig_width += (int)value1 * 2;
                player.dig_height += (int)value2 * 2;
                break;
            case ItemEffectType.AddUse1:
                player.useItem += (int)value1;
                break;
            case ItemEffectType.AddSpeed5SubTime10:
                player.moveSpeed += (int)value1;
                player.stageUI.phaseLimit[(int)Phase.DIG] -= value2;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 常在効果の解除（削除時）
    /// </summary>
    public override void OnHoldDelete(PlayerController player)
    {
        switch (effectType)
        {
            case ItemEffectType.AddPower1:
                player.digPower -= (int)value1;
                break;
            case ItemEffectType.AddDigArea:
                player.dig_width -= (int)value1 * 2;
                player.dig_height -= (int)value2 * 2;
                break;
            case ItemEffectType.AddUse1:
                player.useItem -= (int)value1;
                break;
            case ItemEffectType.AddSpeed5SubTime10:
                player.moveSpeed -= (int)value1;
                player.stageUI.phaseLimit[(int)Phase.DIG] += value2;
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
            case ItemEffectType.AddLimitTurn:
                player.digLimit += (int)activeValue1;
                break;
            case ItemEffectType.AddDigArea:
                player.dig_width += (int)activeValue1 * 2;
                player.dig_height += (int)activeValue2 * 2;
                break;
            default:
                Debug.Log("このアイテムに『あくてぃぶ』効果はありません");
                break;
        }
        if (duration > 0)
        {
            player.isActiveItems.Add(this);
            activeItemTurn = stageUI.currentTurn;
        }
    }

    /// <summary>
    /// ターン経過による自動解除
    /// </summary>
    public override void TurnCount(PlayerController player, StageUI stageUI)
    {
        if (duration > 0 && player.isActiveItems.Contains(this))
        {
            // 持続ターンが過ぎたら効果解除
            if ((activeItemTurn + duration) < stageUI.currentTurn)  
            {
                OnActiveDelete(player);
                player.isActiveItems.Remove(this);
                isUseActive = false;
            }
        }
    }

    /// <summary>
    /// 常在効果の解除（削除時）
    /// </summary>
    public override void OnActiveDelete(PlayerController player)
    {
        switch (effectType)
        {
            case ItemEffectType.AddLimitTurn:
                player.digLimit -= (int)activeValue1;
                break;
            case ItemEffectType.AddDigArea:
                player.dig_width -= (int)activeValue1 * 2;
                player.dig_height -= (int)activeValue2 * 2;
                break;
            default:
                break;
        }
    }
}
