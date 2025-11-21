using UnityEngine;

/// <summary>
/// アイテムの効果タイプを定義
/// </summary>
public enum ItemEffectType
{
    /// <summary>
    /// 命名規則
    /// Add + 変更点 + VX(変数のvalue1など) + Y(変更値→プラスなら"P"、マイナスなら"M"をつける)
    /// ターン制限があるものはTX(何ターンか) + CX(クールタイム)
    /// </summary>
    None,
    AddDigPowerV1P1,
    AddDigWidthAreaV1P1HeightAreaV2P1,
    AddDigWidthAreaV1P2HeightAreaV2M1,
    AddDigWidthAreaV1M1HeightAreaV2P2,
    AddDigLimitV1P1T3C2,
    AddUseItemV1P1,
    AddSpeedV1P5AddDigTimeV2M10
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
    public float value3;
    [Tooltip("数値パラメータ4")]
    public float value4;

    [Tooltip("持続ターン数（0なら常在効果）")]
    public int duration;
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
            case ItemEffectType.AddDigPowerV1P1:
                player.digPower += (int)value1;
                break;
            case ItemEffectType.AddDigWidthAreaV1P1HeightAreaV2P1:
                player.dig_width += (int)value1 * 2;
                player.dig_height += (int)value2 * 2;
                break;
            case ItemEffectType.AddDigWidthAreaV1P2HeightAreaV2M1:
                player.dig_width += (int)value1 * 2;
                player.dig_height += (int)value2 * 2;
                break;
            case ItemEffectType.AddDigWidthAreaV1M1HeightAreaV2P2:
                player.dig_width += (int)value1 * 2;
                player.dig_height += (int)value2 * 2;
                break;
            case ItemEffectType.AddUseItemV1P1:
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
            case ItemEffectType.AddDigPowerV1P1:
                player.digPower -= (int)value1;
                break;
            case ItemEffectType.AddDigWidthAreaV1P1HeightAreaV2P1:
                player.dig_width -= (int)value1 * 2;
                player.dig_height -= (int)value2 * 2;
                break;
            case ItemEffectType.AddDigWidthAreaV1P2HeightAreaV2M1:
                player.dig_width -= (int)value1 * 2;
                player.dig_height -= (int)value2 * 2;
                break;
            case ItemEffectType.AddDigWidthAreaV1M1HeightAreaV2P2:
                player.dig_width -= (int)value1 * 2;
                player.dig_height -= (int)value2 * 2;
                break;
            case ItemEffectType.AddUseItemV1P1:
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
