//using UnityEngine;
//using System.Collections.Generic;

//public class ActiveIconCreater : MonoBehaviour
//{
//    [Header("アイコンを表示する親オブジェクト")]
//    public Transform iconParent;

//    [Header("描画画像")]
//    public GameObject activeIcon;
//    public List<ItemBase> createrList = new List<ItemBase>();

//    [Header("プレイヤー")]
//    public GameObject player;
//    public PlayerController playerController;

//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (player == null)
//        {
//            player = GameObject.FindWithTag("Player");
//            playerController = player.GetComponent<PlayerController>();
//        }

//        AddActiveItemData();
//        UpdateIcons(createrList.Count);
//    }

//    public void AddActiveItemData()
//    {
//        //for (int i = 0; i < playerController.isActiveItems.Count; i++) 
//        //{
//        //    ItemBase activeItem = playerController.isActiveItems[i];
//        //    activeIcon
//        //    Instantiate(activeItem.icon, iconParent);
//        //}
//    }

//    public void UpdateIcons(int value)
//    {
//        // 既存アイコンを削除
//        foreach (Transform child in iconParent)
//        {
//            Destroy(child.gameObject);
//        }

//        // 新しくアイコンを生成
//        for (int i = 0; i < value; i++)
//        {
//            Instantiate(activeIcon, iconParent);
//        }
//    }
//}
