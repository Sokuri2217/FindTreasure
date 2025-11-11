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
    }

    // アイテムを追加
    public bool AddItem(ItemBase item)
    {
        //上限に達していた場合
        if (items.Count >= getMaxItem)
        {
            return false;
        }

        items.Add(item);
        item.OnGet(player);    // 即時効果を発動
        item.OnHold(player);   // 所持時効果を適用
        Debug.Log($"{item.name} を追加しました");

        return true;
    }

    // アイテムを削除
    public void RemoveItem(ItemBase item)
    {
        if (items.Contains(item))
        {
            item.OnDelete(player); // 効果解除
            items.Remove(item);
            Debug.Log($"{item.name} を削除しました");
        }
    }

    // 特定アイテムを保持しているか確認
    public bool HasItem(ItemBase item)
    {
        return items.Contains(item);
    }
}
