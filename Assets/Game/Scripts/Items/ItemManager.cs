using UnityEngine;

/// <summary>
/// アイテムの効果タイプを定義
/// </summary>
public enum ItemEffectType
{
    None,
    AddDigPower,
    AddDigArea,
    AddDigLimit,
    AddUseItemCount,
}

/// <summary>
/// 各アイテムデータをScriptableObjectとして定義
/// </summary>
[CreateAssetMenu(fileName = "ItemBase", menuName = "Scriptable Objects/ItemManager")]
public class ItemManager : ItemBase
{
    [Header("効果設定")]
    public ItemEffectType effectType = ItemEffectType.None;

    [Tooltip("数値パラメータ1（例：追加値、幅、強化量など）")]
    public int value1 = 0;

    [Tooltip("数値パラメータ2（例：高さ、縮小値など）")]
    public int value2 = 0;

    [Tooltip("持続ターン数（0なら常在効果）")]
    public int duration = 0;

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
            case ItemEffectType.AddDigPower:
                player.digPower += value1;
                break;

            case ItemEffectType.AddDigArea:
                player.dig_width += value1 * 2;
                player.dig_height += value2 * 2;
                break;

            case ItemEffectType.AddDigLimit:
                player.digLimit += value1;
                break;
            case ItemEffectType.AddUseItemCount:
                //
                break;
        }
    }

    /// <summary>
    /// 常在効果の解除（削除時）
    /// </summary>
    public override void OnDelete(PlayerController player)
    {
        switch (effectType)
        {
            case ItemEffectType.AddDigPower:
                player.digPower -= value1;
                break;
            case ItemEffectType.AddDigArea:
                player.dig_width -= value1 * 2;
                player.dig_height -= value2 * 2;
                break;
            case ItemEffectType.AddDigLimit:
                player.digLimit -= value1;
                break;
            case ItemEffectType.AddUseItemCount:
                //
                break;
        }
    }

    /// <summary>
    /// 任意発動時（ボタンなどで使用）
    /// </summary>
    public override void OnUse(PlayerController player)
    {
        if (duration > 0)
        {
            player.isActiveItems.Add(this);
            OnHold(player); // 使用と同時に効果発動
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
            if (stageUI.currentTurn >= duration)
            {
                OnDelete(player);
                player.isActiveItems.Remove(this);
            }
        }
    }
}
