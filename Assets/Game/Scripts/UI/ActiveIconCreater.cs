using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ActiveIconCreater : MonoBehaviour
{
    [Header("アイコンを表示する親オブジェクト")]
    public Transform iconParent;

    [Header("描画画像")]
    public GameObject iconChild;
    public GameObject activeIcon;

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

        UpdateIcons(inventory.isActiveItems.Count);
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
            // prefab から生成
            GameObject iconObj = Instantiate(iconChild, iconParent);
            GameObject activeIconObj = Instantiate(activeIcon, iconObj.transform);

            Image iconImage = activeIconObj.GetComponent<Image>();
            iconImage.sprite = inventory.isActiveItems[i].icon;
        }
    }
}
