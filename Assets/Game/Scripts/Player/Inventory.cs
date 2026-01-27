using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("アイテム情報")]
    public List<ItemInstance> items = new List<ItemInstance>();
    public List<ItemInstance> isActiveItems = new List<ItemInstance>();
    public List<ItemInstance> isCoolDownItems = new List<ItemInstance>();

    [Header("選択中")]
    public int isSelectItem;
    public int lineHeight; //縦列
    public int lineWidth;  //横列

    [Header("所持可能数")]
    public int lineMaxHeight;
    public int lineMaxWidth;

    [Header("アイテム効果")]
    public int changeActiveTurn;
    public int changeCoolTime;

    [Header("スクリプト参照")]
    public PlayerController player;
    public StageUI stageUI;
    public GameManager gameManager;

    public void Start()
    {
        //スクリプト取得
        player = GetComponent<PlayerController>();
        stageUI = player.stageUI;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        SetUniqueItem(gameManager.uniqueItems[gameManager.mapNum]);
    }

    public void Update()
    {
        UpdateActiveItems();
        UpdateCoolDownItems();
        UpdateItemTurnModifiers();
    }

    void UpdateActiveItems()
    {
        for (int i = isActiveItems.Count - 1; i >= 0; i--)
        {
            ItemInstance item = isActiveItems[i];

            if (item.useActiveTurn + item.duration - 1 < stageUI.currentTurn)
            {
                item.itemBase.OnActiveDelete(player, stageUI);

                item.isUseActive = false;
                item.isCoolDown = true;
                item.coolTimeTurn = stageUI.currentTurn;

                isCoolDownItems.Add(item);
                isActiveItems.RemoveAt(i);
            }
        }
    }

    void UpdateCoolDownItems()
    {
        for (int i = isCoolDownItems.Count - 1; i >= 0; i--)
        {
            ItemInstance item = isCoolDownItems[i];

            if (item.coolTimeTurn + item.coolTime - 1 < stageUI.currentTurn)
            {
                item.isCoolDown = false;
                isCoolDownItems.RemoveAt(i);
            }
        }
    }

    void UpdateItemTurnModifiers()
    {
        foreach (var item in items)
        {
            if (item == null || item.itemBase == null)
                continue;

            item.duration = item.itemBase.originDuration + changeActiveTurn;
            item.coolTime = item.itemBase.originCoolTime + changeCoolTime;
        }
    }
    // アイテムを追加
    public bool AddItem(ItemBase itemBase)
    {
        if (items[(lineMaxWidth * lineMaxHeight - 1)].itemBase != null)  
        {
            Debug.Log("これ以上アイテムを取得できません");
            return false;
        }

        ItemInstance instance = new ItemInstance(itemBase);

        // ターン補正反映
        instance.duration += changeActiveTurn;
        instance.coolTime += changeCoolTime;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemBase == null)
            {
                items[i] = instance;
                break;
            }
        }

        itemBase.OnGet(player, stageUI);
        itemBase.OnHold(player, stageUI);
        Debug.Log(instance.itemBase.ToString());

        return true;
    }

    public void UseItem(ItemInstance item)
    {
        if (item.isCoolDown || item.isUseActive)
            return;

        item.itemBase.OnUse(player, stageUI);

        item.isUseActive = true;
        item.useActiveTurn = stageUI.currentTurn;

        if (item.duration > 0)
        {
            isActiveItems.Add(item);
        }
        else
        {
            // 即時クールダウン
            item.isCoolDown = true;
            item.coolTimeTurn = stageUI.currentTurn;
            isCoolDownItems.Add(item);
        }
    }

    //特殊アイテムを設定
    public void SetUniqueItem(ItemBase uniqueItem)
    {
        if (uniqueItem == null) return;

        ItemInstance instance = new ItemInstance(uniqueItem);
        items[0] = instance;

        uniqueItem.OnGet(player, stageUI);
        uniqueItem.OnHold(player, stageUI);
    }

    // 特定アイテムを保持しているか確認
    public bool HasItem(ItemBase itemBase)
    {
        return items.Exists(i => i.itemBase == itemBase);
    }

    public void ReduceAllItemsActiveTime(int turnReduce)
    {
        foreach (var item in items)
        {
            if (item.itemBase.originDuration != 0)
            {
                item.useActiveTurn += turnReduce;
                if (item.useActiveTurn < 1) item.useActiveTurn = 1;
            }
        }
    }

    public void ReduceOtherItemsActiveTime(int turnReduce, ItemInstance exceptItem)
    {
        foreach (var item in items)
        {
            if (item != exceptItem && item.itemBase.originDuration != 0)  
            {
                item.useActiveTurn += turnReduce; // クールタイムを減らす
                if (item.useActiveTurn < 1) item.useActiveTurn = 1;
            }
        }
    }

    public void ReduceAllItemsCoolTime(int turnReduce)
    {
        foreach (var item in items)
        {
            item.coolTimeTurn += turnReduce;
            if (item.coolTimeTurn < 0) item.coolTimeTurn = 0;
        }
    }

    public void ReduceOtherItemsCoolTime(int turnReduce, ItemInstance exceptItem)
    {
        foreach (var item in items)
        {
            if (item != exceptItem) 
            {
                item.coolTimeTurn += turnReduce; // クールタイムを減らす
                if (item.coolTimeTurn < 0) item.coolTimeTurn = 0;
            }
        }
    }

    public void ResetOtherItemsCoolTime(ItemInstance exceptItem)
    {
        foreach (var item in items)
        {
            if (item != exceptItem)
            {
                item.coolTimeTurn = 0;
                item.isCoolDown = false;
            }
        }
    }
}
