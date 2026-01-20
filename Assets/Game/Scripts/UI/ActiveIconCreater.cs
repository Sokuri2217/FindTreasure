using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ActiveIconCreater : MonoBehaviour
{
    [Header("アイコンを表示する親オブジェクト")]
    public Transform iconParent;

    [Header("描画画像")]
    public GameObject iconPrefab;

    [Header("プレイヤー")]
    public GameObject player;
    public PlayerController playerController;
    public Inventory inventory;
    public StageUI stageUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            playerController = player.GetComponent<PlayerController>();
            inventory = player.GetComponent<Inventory>();
            stageUI = player.GetComponent<StageUI>();
        }

        UpdateIcons();
    }

    public void UpdateIcons()
    {
        // 既存アイコンを削除
        foreach (Transform child in iconParent)
        {
            Destroy(child.gameObject);
        }

        // 新しくアイコンを生成
        // 新しいアイコンを生成
        for (int i = 0; i < inventory.isActiveItems.Count; i++)
        {
            GameObject iconObj = Instantiate(iconPrefab, iconParent);
            Image img = iconObj.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = inventory.isActiveItems[i].itemBase.icon;
            }
        }
    }
}
