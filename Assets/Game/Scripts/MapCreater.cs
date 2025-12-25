using System.Collections.Generic;
using UnityEngine;

public class MapCreater : MonoBehaviour
{
    public float tileSize;            // 1マスの大きさ
    public GameObject tilePrefab;     // タイルオブジェクト
    public GameObject playerPrefab;   // プレイヤーオブジェクト
    public GameObject treasurePrefab; //タカラモノオブジェクト
    public GameObject itemPrefab;     //ホリダシモノオブジェクト
    public Terrain ground;            //地面(Terrain)
    public int[] minDeep;             //最小深度
    public int[] maxDeep;             //最大深度
    public bool createMap;            //タイルマップ生成許可フラグ

    //タイル情報
    public List<TileManager> allTiles = new List<TileManager>();
    [Header("スクリプト情報")]
    public GameManager gameManager;
    public MapGimmick mapGimmick;

    void Start()
    {
        //スクリプト取得
        mapGimmick = GetComponent<MapGimmick>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        //マップ生成
        PlacePlayerOnStart();                      // プレイヤーをスタート位置に配置
        GenerateGrid();                            // グリッドマップを生成
        mapGimmick.SetGimmick(gameManager.mapNum); // ギミックを適応
        PlaceItemsRandomly();                      // タイルにアイテムをランダムに埋め込む
        ChangeGroundPoint();                       // 地面オブジェクトを移動させる
    }

    void GenerateGrid()
    {
        for (int x = 0; x < (gameManager.setStage * 10); x++) 
        {
            for (int z = 0; z < (gameManager.setStage * 10); z++) 
            {
                // タイルを配置する座標を計算（X-Z平面上）
                Vector3 spawnPos = new Vector3(x * tileSize, 0, z * tileSize);

                // タイルプレハブを生成し、親を GridManager に設定
                GameObject tileObj = Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform);
                tileObj.name = $"Tile {x},{z}";

                // Tile スクリプトを取得して初期化
                TileManager tile = tileObj.GetComponent<TileManager>();
                // 初期状態ではアイテムなし
                tile.SetHasItem(false); 

                // リストに追加して後から参照可能に
                allTiles.Add(tile);
            }
        }

        createMap = true;
    }

    public void PlaceItemsRandomly()
    {
        if (!createMap) return;

        // 設定された個数以内で、ユニークにランダム選択
        int treasureCount = Mathf.Min(gameManager.setTreasure, allTiles.Count);
        int itemCount = Mathf.Min(gameManager.setItem, allTiles.Count);

        List<TileManager> candidates = new List<TileManager>(allTiles);
        List<ItemBase> itemBases = new List<ItemBase>(gameManager.items);

        //タカラモノを設定
        for (int i = 0; i < treasureCount; i++)
        {
            int index = Random.Range(0, candidates.Count);
            TileManager selected = candidates[index];

            // タカラモノをセット
            selected.SetHasTreasure(true);

            

            //オブジェクトを格納
            selected.treasureObj = treasurePrefab;
            //シンドを設定(minDeep～maxDeep-1の値をランダムでシンドとして設定する)
            selected.deep = Random.Range(minDeep[(int)Deep.TREASURE], maxDeep[(int)Deep.TREASURE]);

            // 重複しないように候補から削除
            candidates.RemoveAt(index);
        }
        //ホリダシモノを設定
        for (int j = 0; j < itemCount; j++) 
        {
            int index = Random.Range(0, candidates.Count);
            TileManager selected = candidates[index];
            selected.SetHasItem(true);

            // アイテムを新しく生成
            GameObject newItem = itemPrefab;

            // ランダムなアイテムデータを設定
            int itemIndex = Random.Range(0, itemBases.Count);
            ItemObject itemData = newItem.GetComponent<ItemObject>();
            itemData.itemBase = itemBases[itemIndex];

            //シンドを設定(minDeep～maxDeep-1の値をランダムでシンドとして設定する)
            selected.deep = Random.Range(minDeep[(int)Deep.ITEM], maxDeep[(int)Deep.ITEM]);

            // TileManager に保持
            selected.itemObj = newItem;
            selected.deep = 1;
        }
    }

    void PlacePlayerOnStart()
    {
        Vector3 startPos = new Vector3(Mathf.Round(gameManager.setStage * 10 / 2), playerPrefab.transform.position.y, Mathf.Round(gameManager.setStage * 10 / 2));
        GameObject playerObj = Instantiate(playerPrefab, startPos, Quaternion.identity);
        mapGimmick.player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    //地面の位置を調整
    public void ChangeGroundPoint()
    {
        //地面オブジェクトを取得
        ground = GameObject.FindWithTag("Ground").GetComponent<Terrain>();
        Transform groundPos = ground.transform;
        Vector3 groundSize = ground.terrainData.size;
        //グリッドマップの広さに応じて調整
        groundPos.position -= new Vector3(groundSize.x / 2, 0.0f, groundSize.z / 2);
        groundPos.position += new Vector3((gameManager.setStage * 10 / 2), 0.0f, (gameManager.setStage * 10 / 2));
    }
}

public enum Deep
{
    TREASURE,
    ITEM
}
