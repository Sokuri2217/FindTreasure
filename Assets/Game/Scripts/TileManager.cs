using UnityEngine;

public class TileManager : MonoBehaviour
{
    [Header("タイルの基本情報")]
    public bool hasTreasure;       //タカラモノがあるかどうか
    public bool hasItem;           //ホリダシモノがあるかどうか
    public GameObject treasureObj; //タカラモノオブジェクト
    public GameObject itemObj;     //ホリダシモノオブジェクト
    public int deep;               //深度←これが0になるとタカラモノを地表に出す
    public bool dig;               //このグリッドを掘れるかどうか

    [Header("オブジェクト参照")]
    private GameObject player; //プレイヤーオブジェクト

    void Start()
    {

    }

    private void Update()
    {
        //プレイヤーを保存出来ていないとき
        if (player == null) 
        {
            player = GameObject.FindWithTag("Player");
        }

        DigFieldGrid();
        DigGrid();
        ChangeGridColor();
    }

    //採掘範囲
    public void DigFieldGrid()
    {
        PlayerController playerController = player.GetComponent<PlayerController>();

        dig = false;

        //採掘範囲
        int digWidth = (playerController.dig_width / 2);
        int digHeight = (playerController.dig_height / 2);

        // プレイヤーの回転（Y軸だけ考慮）
        Quaternion rotation = Quaternion.Euler(0, player.transform.eulerAngles.y, 0);

        for (int i = -digWidth; i <= digWidth; i++)  
        {
            for (int j = -digHeight; j <= digHeight; j++) 
            {
                // プレイヤーの向きに合わせて、(i,j) を回転させる
                Vector3 offset = new Vector3(j, 0, i); // XZ平面 (jがX方向、iがZ方向)
                Vector3 rotatedOffset = rotation * offset;

                // 対象タイルのワールド座標
                Vector3 targetPos = player.transform.position + rotatedOffset;

                if (Mathf.Round(transform.position.x) == Mathf.Round(targetPos.x) &&
                    Mathf.Round(transform.position.z) == Mathf.Round(targetPos.z)) 
                {
                    //十字範囲のみ採掘出来る
                    dig = !((i > 0 || i < 0) && (j > 0 || j < 0));
                    return;
                }
            }
        }
    }

    //採掘実行
    public void DigGrid()
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController.isDig)  
        {
            if (hasTreasure && dig)
            {
                //深度を下げ、深度が0以下になると、タカラモノを地表に出す
                deep -= playerController.digPower;
                if (deep <= 0)
                {
                    hasTreasure = false;
                    //オブジェクト生成
                    Transform treasurePos = treasureObj.transform;
                    treasurePos.position = new Vector3(
                        transform.position.x,
                        treasurePos.position.y,
                        transform.position.z
                        );
                    Instantiate(treasureObj, treasurePos.position, Quaternion.identity);
                }
            }
            else if (hasItem && dig) 
            {
                //深度を下げ、深度が0以下になると、ホリダシモノを地表に出す
                deep -= playerController.digPower;
                if (deep <= 0)
                {
                    hasItem = false;
                    Transform itemPos = itemObj.transform;
                    itemPos.position = new Vector3(
                        transform.position.x,
                        itemPos.position.y,
                        transform.position.z
                        );
                    //オブジェクト生成
                    Instantiate(itemObj, itemPos.position, Quaternion.identity);
                    ItemObject itemObject = itemObj.GetComponent<ItemObject>();
                    GameManager gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
                    if (Random.Range(0, 1000) <= 4)
                    {
                        itemObject.itemBase = gameManager.uniqueItems[Random.Range(0, gameManager.uniqueItems.Count)];
                    }
                    else
                    {
                        itemObject.itemBase = gameManager.items[Random.Range(0, gameManager.items.Count)];
                    }
                }
                
            }
        }
    }

    // アイテム有無を設定
    public void SetHasTreasure(bool value)
    {
        hasTreasure = value;
    }

    // アイテム有無を設定
    public void SetHasItem(bool value)
    {
        hasItem = value;
    }

    //グリッドの色変更
    public void ChangeGridColor()
    {
        Renderer renderer = GetComponent<Renderer>();
        //採掘範囲内のグリッドを赤くする
        if (dig)
        {
            renderer.material.color = Color.red;
        }
        //else if (hasTreasure)
        //{
        //    renderer.material.color = Color.green;
        //}
        //else if (hasItem)
        //{
        //    renderer.material.color = Color.blue;
        //}
        else
        {
            renderer.material.color = Color.white;
        }
    }
}
