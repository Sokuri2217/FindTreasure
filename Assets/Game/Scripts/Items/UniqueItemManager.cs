using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum UniqueEffectType
{
    Stage1,
    Stage2,
    Stage3,
    Stage4,
    Stage5,
}

/// <summary>
/// 各アイテムデータをScriptableObjectとして定義
/// </summary>
[CreateAssetMenu(fileName = "ItemBase", menuName = "Scriptable Objects/UniqueItemManager")]

public class UniqueItemManager : ItemBase
{
    [Header("効果設定")]
    public UniqueEffectType type;

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
        switch (type)
        {
            case UniqueEffectType.Stage1:
                break;
            case UniqueEffectType.Stage2:
                break;
            case UniqueEffectType.Stage3:
                break;
            case UniqueEffectType.Stage4:
                break;
            case UniqueEffectType.Stage5:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 常在効果（保持している間に有効）
    /// </summary>
    public override void OnHold(PlayerController player)
    {
        switch (type)
        {
            case UniqueEffectType.Stage1:
                break;
            case UniqueEffectType.Stage2:
                break;
            case UniqueEffectType.Stage3:
                break;
            case UniqueEffectType.Stage4:
                break;
            case UniqueEffectType.Stage5:
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
        switch (type)
        {
            case UniqueEffectType.Stage1:
                break;
            case UniqueEffectType.Stage2:
                break;
            case UniqueEffectType.Stage3:
                break;
            case UniqueEffectType.Stage4:
                break;
            case UniqueEffectType.Stage5:
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
        switch (type)
        {
            case UniqueEffectType.Stage1:
                break;
            case UniqueEffectType.Stage2:
                break;
            case UniqueEffectType.Stage3:
                break;
            case UniqueEffectType.Stage4:
                break;
            case UniqueEffectType.Stage5:
                break;
            default:
                break;
        }

        if (duration > 0)
        {
            player.isActiveItems.Add(this);
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
                OnActiveDelete(player);
                player.isActiveItems.Remove(this);
            }
        }
    }

    /// <summary>
    /// 常在効果の解除（削除時）
    /// </summary>
    public override void OnActiveDelete(PlayerController player)
    {
        switch (type)
        {
            case UniqueEffectType.Stage1:
                break;
            case UniqueEffectType.Stage2:
                break;
            case UniqueEffectType.Stage3:
                break;
            case UniqueEffectType.Stage4:
                break;
            case UniqueEffectType.Stage5:
                break;
            default:
                break;
        }
    }
}
