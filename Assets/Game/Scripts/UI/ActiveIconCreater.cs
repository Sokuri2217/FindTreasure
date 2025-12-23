using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ActiveIconCreater : MonoBehaviour
{
    [Header("アイコンを表示する親オブジェクト")]
    public Transform iconParent;

    [Header("描画画像")]
    public GameObject activeIcon;
    public List<ItemBase> createrList = new List<ItemBase>();

    [Header("プレイヤー")]
    public PlayerController player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        }

        AddActiveItemData();
        UpdateIcons(createrList.Count);
    }

    public void AddActiveItemData()
    {
        
    }

    public void UpdateIcons(int value)
    {
        // 既存アイコンを削除
        foreach (Transform child in iconParent)
        {
            Destroy(child.gameObject);
        }

        // 新しくアイコンを生成
        for (int i = 0; i < value; i++)
        {
            Instantiate(activeIcon, iconParent);
            Image iconImage = activeIcon.GetComponent<Image>();
            iconImage.sprite = player.isActiveItems[i].icon;
        }
    }
}
