using UnityEngine;
using System.Collections.Generic;

public class DigIconCreater : MonoBehaviour
{
    [Header("アイコンを表示する親オブジェクト")]
    public Transform iconParent;

    [Header("描画画像")]
    public GameObject digCountIcon;
    public int currentDigCount;

    [Header("スクリプト参照")]
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
            currentDigCount = player.digLimit;
            UpdateIcons(currentDigCount);
        }

        currentDigCount = player.digCurrent;
        UpdateIcons(currentDigCount);
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
            Instantiate(digCountIcon, iconParent);
        }
    }
}
