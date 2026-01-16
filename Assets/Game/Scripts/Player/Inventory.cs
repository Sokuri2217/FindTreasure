using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("アイテム情報")]
    public List<ItemBase> items = new List<ItemBase>();
    public List<ItemBase> isActiveItems = new List<ItemBase>();
    public List<ItemBase> isCoolDownItems = new List<ItemBase>();

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
        for (int i = 0; i < isActiveItems.Count; i++) 
        {
            //ターン経過で効果を解除
            int useActiveTurn = isActiveItems[i].useActiveTurn;
            int duration = isActiveItems[i].duration;
            if (isActiveItems[i].isUseActive && (useActiveTurn + duration) <= stageUI.currentTurn) 
            {
                isActiveItems[i].OnActiveDelete(player, stageUI);
                isActiveItems.Remove(isActiveItems[i]);
                isActiveItems[i].isCoolDown = true;
                isActiveItems[i].coolTimeTurn = stageUI.currentTurn;
                isCoolDownItems.Add(isActiveItems[i]);
            }
        }
        for (int i = 0; i < isCoolDownItems.Count; i++) 
        {
            //ターン経過で再使用可能にする
            int coolTimeTurn = isCoolDownItems[i].coolTimeTurn;
            int coolTime = isCoolDownItems[i].coolTime;
            if (isCoolDownItems[i].isCoolDown && (coolTimeTurn + coolTime) <= stageUI.currentTurn)
            {
                isCoolDownItems[i].isCoolDown = false;
                isCoolDownItems.Remove(isCoolDownItems[i]);
            }
        }

        //各ターンの変更処理
        for (int i = 0; i < items.Count; i++) 
        {
            if (items[i] != null) 
            {
                items[i].duration = items[i].originDuration + changeActiveTurn;
                items[i].coolTime = items[i].originCoolTime + changeCoolTime;
            }
        }
    }

    // アイテムを追加
    public bool AddItem(ItemBase item)
    {
        for (int i = 0; i < items.Count; i++) 
        {
            //インベントリが全て埋まっているとき
            if (items[(items.Count - 1)] != null)
            {
                Debug.Log($"これ以上アイテムを取得できません");
                return false;
            }
            //インベントリに空白があるとき
            if (items[i] == null)
            {
                items[i] = item;
                item.OnGet(player,player.stageUI);       //即時効果を発動
                item.OnHold(player, player.stageUI);      //所持時効果を適用
                item.isUseActive = false; //アクティブ効果を未使用状態にする
                item.isCoolDown = false;  //アクティブ効果をクールダウン状態にする
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
        uniqueItem.OnGet(player, player.stageUI);    // 即時効果を発動
        uniqueItem.OnHold(player, player.stageUI);   // 所持時効果を適用
        uniqueItem.isUseActive = false;
        uniqueItem.isCoolDown = false;
    }

    // アイテムを削除
    public void RemoveItem(ItemBase item)
    {
        if (items.Contains(item))
        {
            item.OnHoldDelete(player, player.stageUI); // 効果解除
            item.OnActiveDelete(player, player.stageUI); // 効果解除
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
