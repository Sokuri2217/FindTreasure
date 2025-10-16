using UnityEngine;

public class TileManager : MonoBehaviour
{
    [Header("タイルの基本情報")]
    public bool hasTreasure = false; //タカラモノがあるかどうか
    public GameObject treasureObj;   //所持しているタカラモノ
    public int deep; //深度←これが0になるとタカラモノを地表に出す

    private GameObject player; //プレイヤーオブジェクト
    public bool dig; //このグリッドを掘れるかどうか

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
                    if ((i > 0 || i < 0) && (j > 0 || j < 0)) 
                    {
                        dig = false;
                    }
                    else
                    {
                        dig = true;
                    }
                    return;
                }
            }
        }
    }

    //採掘実行
    public void DigGrid()
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController.isDig && hasTreasure && dig)  
        {
            //深度を下げ、深度が0以下になると、タカラモノを地表に出す
            deep -= playerController.digPower;
            if(deep <= 0)
            {
                hasTreasure = false;
                //オブジェクト生成
                Instantiate(treasureObj, transform.position, Quaternion.identity);
            }
        }
    }

    // アイテム有無を設定
    public void SetHasItem(bool value)
    {
        hasTreasure = value;
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
        else if(hasTreasure)
        {
            renderer.material.color = Color.green;
        }
        else
        {
            renderer.material.color = Color.white;
        }
    }
}
