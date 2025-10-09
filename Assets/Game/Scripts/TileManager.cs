using UnityEngine;

public class TileManager : MonoBehaviour
{
    public bool hasItem = false; // このタイルにアイテムがあるかどうか

    void Start()
    {

    }

    // アイテム有無を外部から設定する関数
    public void SetHasItem(bool value)
    {
        hasItem = value;
    }
}
