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

    //[Header("各ターンの変更状態")]
    //public int changeActive;
    //public int changeCoolTime;

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

    }

    public void UpdateActiveItems()
    {
        for (int i = isActiveItems.Count - 1; i >= 0; i--)
        {
            ItemInstance item = isActiveItems[i];
            item.duration--;
            if (item.duration <= 0) 
            {
                item.itemBase.OnActiveDelete(player, stageUI);

                item.isUseActive = false;
                item.isCoolDown = true;
                item.duration = item.itemBase.originDuration;
                isCoolDownItems.Add(item);
                isActiveItems.RemoveAt(i);
            }
        }
    }

    public void UpdateCoolDownItems()
    {
        for (int i = isCoolDownItems.Count - 1; i >= 0; i--)
        {
            ItemInstance item = isCoolDownItems[i];
            item.coolTime--;
            if (item.coolTime <= 0)
            {
                item.isCoolDown = false;
                item.coolTime = item.itemBase.originCoolTime;
                isCoolDownItems.RemoveAt(i);
            }
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

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemBase == null)
            {
                items[i] = instance;
                items[i].slotID = i;
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

        if (item.duration > 0)
        {
            isActiveItems.Add(item);
        }
        else
        {
            // 即時クールダウン
            item.isCoolDown = true;
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
            if (item.itemBase != null && item.itemBase.originDuration != 0) 
            {
                item.duration += turnReduce;
                if (item.duration < 1) item.duration = 1;
            }
        }
    }

    public void ReduceOtherItemsActiveTime(int turnReduce, ItemInstance exceptItem)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].slotID != exceptItem.slotID && items[i].itemBase.originDuration != 0) 
            {
                items[i].duration += turnReduce; // クールタイムを減らす
                if (items[i].duration < 1) items[i].duration = 1;
            }
        }
    }

    public void ReduceAllItemsCoolTime(int turnReduce)
    {
        foreach (var item in items)
        {
            item.coolTime += turnReduce;
            if (item.coolTime < 0) item.coolTime = 0;
        }
    }

    public void ReduceOtherItemsCoolTime(int turnReduce, ItemInstance exceptItem)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].slotID != exceptItem.slotID)
            {
                items[i].coolTime += turnReduce; // クールタイムを減らす
                if (items[i].coolTime < 0) items[i].coolTime = 0;
            }
        }
    }

    public void ResetOtherItemsCoolTime(ItemInstance exceptItem)
    {
        for (int i = 0; i < items.Count; i++) 
        {
            if (items[i].slotID != exceptItem.slotID)   
            {
                items[i].isCoolDown = false;
            }
        }
    }
}
