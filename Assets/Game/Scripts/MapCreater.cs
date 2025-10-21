using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCreater : MonoBehaviour
{
    public int mapSize;              //マップのサイズ(mapSize * mapSize)
    public float tileSize;           // 1マスの大きさ
    public GameObject tilePrefab;    // タイルオブジェクト
    public GameObject playerPrefab;  // プレイヤーオブジェクト
    public GameObject treasurePrefab;//タカラモノオブジェクト
    public GameObject itemPrefab;    //ホリダシモノオブジェクト

    //マップ上に生成する数
    public int treasure; //タカラモノ
    public int item;     //ホリダシモノ

    private List<TileManager> allTiles = new List<TileManager>();

    void Start()
    {
        GenerateGrid();          // グリッドマップを生成
        PlaceItemsRandomly();    // タイルにアイテムをランダムに埋め込む
        PlacePlayerOnStart();    // プレイヤーをスタート位置に配置
    }

    void GenerateGrid()
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int z = 0; z < mapSize; z++)
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
    }

    void PlaceItemsRandomly()
    {
        // 設定された個数以内で、ユニークにランダム選択
        int treasureCount = Mathf.Min(treasure, allTiles.Count);
        int itemCount = Mathf.Min(item, allTiles.Count);

        List<TileManager> candidates = new List<TileManager>(allTiles);

        //タカラモノを設定
        for (int i = 0; i < treasureCount; i++)
        {
            int index = Random.Range(0, candidates.Count);
            TileManager selected = candidates[index];
            selected.SetHasTreasure(true); // タカラモノをセット
            selected.treasureObj = treasurePrefab; //オブジェクトを格納
            selected.deep = 1;
            candidates.RemoveAt(index); // 重複しないように候補から削除
        }
        //ホリダシモノを設定
        for(int j=0;j<itemCount;j++)
        {
            int index = Random.Range(0, candidates.Count);
            TileManager selected = candidates[index];
            selected.SetHasItem(true);  // ホリダシモノをセット
            candidates.RemoveAt(index); // 重複しないように候補から削除
        }
    }

    void PlacePlayerOnStart()
    {
        Vector3 startPos = new Vector3(Mathf.Round(mapSize / 2), 1.25f, Mathf.Round(mapSize / 2));
        GameObject playerObj = Instantiate(playerPrefab, startPos, Quaternion.identity);
    }
}
