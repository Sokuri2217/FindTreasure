using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("アイテム情報")]
    public List<ItemBase> items = new List<ItemBase>();

    [Header("選択中")]
    public int isSelectItem;
    public int lineHeight; //縦列
    public int lineWidth;  //横列

    [Header("所持可能数")]
    public int getMaxItem;
    public int lineMaxHeight;
    public int lineMaxWidth;

    [Header("スクリプト参照")]
    public PlayerController player;

    public void Start()
    {
        player = GetComponent<PlayerController>();
        SetInventory(getMaxItem);
    }

    //インベントリの初期化
    public void SetInventory(int slotNum)
    {
        //最大所持数に応じて、インベントリの容量を決まる
        for (int i = 0; i < slotNum; i++) 
        {
            items.Add(null);
        }
    }

    // アイテムを追加
    public bool AddItem(ItemBase item)
    {
        for (int i = 0; i < getMaxItem; i++) 
        {
            //インベントリが全て埋まっているとき
            if (items[(getMaxItem - 1)] != null)
            {
                Debug.Log($"これ以上アイテムを取得できません");
                return false;
            }
            //インベントリに空白があるとき
            if (items[i] == null)
            {
                items[i] = item;
                item.OnGet(player);    // 即時効果を発動
                item.OnHold(player);   // 所持時効果を適用
                Debug.Log($"{item.name} を追加しました");
                break;
            }
        }
        return true;
    }

    //特殊アイテムを設定
    public void SetUniqueItem(ItemBase uniqueItem)
    {
        items[0] = uniqueItem;
        uniqueItem.OnGet(player);    // 即時効果を発動
        uniqueItem.OnHold(player);   // 所持時効果を適用
    }

    // アイテムを削除
    public void RemoveItem(ItemBase item)
    {
        if (items.Contains(item))
        {
            item.OnHoldDelete(player); // 効果解除
            item.OnActiveDelete(player); // 効果解除
            items.Remove(item);
            items.Add(null);
            Debug.Log($"{item.name} を削除しました");
        }
    }

    // 特定アイテムを保持しているか確認
    public bool HasItem(ItemBase item)
    {
        return items.Contains(item);
    }
}
