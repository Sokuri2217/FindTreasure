using UnityEngine;

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
[CreateAssetMenu(fileName = "UniqueItem",menuName = "Scriptable Objects/UniqueItemManager")]
public class UniqueItemManager : ItemBase
{
    [Header("効果設定")]
    public UniqueEffectType type;

    [Tooltip("数値パラメータ1")]
    public float value1;
    [Tooltip("数値パラメータ2")]
    public float value2;
    [Tooltip("数値パラメータ3")]
    public float activeValue1;
    [Tooltip("数値パラメータ4")]
    public float activeValue2;

    // ==============================
    // 効果適用処理
    // ==============================

    /// <summary>
    /// アイテム獲得時（即時効果）
    /// </summary>
    public override void OnGet(PlayerController player, StageUI stageUI)
    {

    }

    /// <summary>
    /// 常在効果（保持している間に有効）
    /// </summary>
    public override void OnHold(PlayerController player, StageUI stageUI)
    {
        switch (type)
        {
            case UniqueEffectType.Stage1:
                player.digPower += (int)value1;
                player.dig_width_data += (int)value2 * 2;
                player.dig_height_data += (int)value2 * 2;
                break;
            case UniqueEffectType.Stage2:
                player.digLimit += (int)value1;
                stageUI.phaseLimit[(int)Phase.DIG] += value2;
                break;
            case UniqueEffectType.Stage3:
                player.dig_width_data += (int)value1 * 2;
                player.dig_height_data += (int)value1 * 2;
                player.originSpeed *= value2;
                break;
            case UniqueEffectType.Stage4:
                player.originSpeed *= value1;
                stageUI.phaseLimit[(int)Phase.ITEM] += value2;
                break;
            case UniqueEffectType.Stage5:
                ItemInstance myInstance = player.inventory.items.Find(x => x.itemBase == this);
                if (myInstance != null)
                {
                    player.inventory.ReduceOtherItemsCoolTime((int)value1, myInstance);
                }
                player.digLimit += (int)value2;
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
                stageUI.phaseLimit[(int)Phase.ITEM] += activeValue1;
                stageUI.phaseLimit[(int)Phase.DIG] += activeValue1;
                player.digLimit += (int)activeValue2;
                break;
            case UniqueEffectType.Stage2:
                player.originSpeed *= activeValue1;
                player.ignoredDeep[(int)Get.TREASURE] = true;
                break;
            case UniqueEffectType.Stage3:
                player.digPower += (int)activeValue1;
                player.getObjStopTime[(int)Get.TREASURE] += activeValue2;
                player.getObjStopTime[(int)Get.ITEM] += (activeValue2 / 2);
                break;
            case UniqueEffectType.Stage4:
                player.digLimit += (int)activeValue1;
                player.consumedProbability += activeValue2;
                break;
            case UniqueEffectType.Stage5:
                player.digPower += (int)activeValue1;
                stageUI.currentTurn -= (int)activeValue2;
                player.ignoredDeep[(int)Get.ITEM] = true;
                break;
            default:
                break;
        }
    }

    public override void OnActiveDelete(PlayerController player, StageUI stageUI) 
    {
        switch (type)
        {
            case UniqueEffectType.Stage1:
                stageUI.phaseLimit[(int)Phase.ITEM] -= activeValue1;
                stageUI.phaseLimit[(int)Phase.DIG] -= activeValue1;
                player.digLimit -= (int)activeValue2;
                break;
            case UniqueEffectType.Stage2:
                player.originSpeed /= activeValue1;
                player.ignoredDeep[(int)Get.TREASURE] = false;
                break;
            case UniqueEffectType.Stage3:
                player.digPower += (int)activeValue1;
                player.getObjStopTime[(int)Get.TREASURE] -= activeValue2;
                player.getObjStopTime[(int)Get.ITEM] -= (activeValue2 / 2);
                break;
            case UniqueEffectType.Stage4:
                player.digLimit -= (int)activeValue1;
                player.consumedProbability -= activeValue2;
                break;
            case UniqueEffectType.Stage5:
                player.digPower -= (int)activeValue1;
                stageUI.currentTurn -= (int)activeValue2;
                player.ignoredDeep[(int)Get.ITEM] = false;
                break;
            default:
                break;
        }
    }
}
