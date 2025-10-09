using UnityEngine;
using System.Collections.Generic;

public class MapCreater : MonoBehaviour
{
    public int width;                // 横のマス数
    public int height;               // 縦のマス数
    public float tileSize;           // 1マスの大きさ
    public GameObject tilePrefab;    // プレハブ参照

    public int maxTreasure; //生成するタカラモノの最大値

    private List<TileManager> allTiles = new List<TileManager>();

    void Start()
    {
        GenerateGrid();          // グリッドマップを生成
        PlaceItemsRandomly();    // タイルにアイテムをランダムに埋め込む
        PlacePlayerOnStart();    // プレイヤーをスタート位置に配置
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // タイルを配置する座標を計算（X-Z平面上）
                Vector3 spawnPos = new Vector3(x * tileSize, 0, z * tileSize);

                // タイルプレハブを生成し、親を GridManager に設定
                GameObject tileObj = Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform);
                tileObj.name = $"Tile {x},{z}";

                // Tile スクリプトを取得して初期化
                TileManager tile = tileObj.GetComponent<TileManager>();
                tile.SetHasItem(false); // 初期状態ではアイテムなし

                // リストに追加して後から参照可能に
                allTiles.Add(tile);
            }
        }
    }

    void PlaceItemsRandomly()
    {
        // 設定された個数以内で、ユニークにランダム選択
        int itemCount = Mathf.Min(maxTreasure, allTiles.Count);

        List<TileManager> candidates = new List<TileManager>(allTiles);

        for (int i = 0; i < itemCount; i++)
        {
            int index = Random.Range(0, candidates.Count);
            TileManager selected = candidates[index];
            selected.SetHasItem(true); // アイテムをセット
            candidates.RemoveAt(index); // 重複しないように候補から削除
        }
    }

    void PlacePlayerOnStart()
    {
        // プレイヤーオブジェクトをシーンから探す（名前は"Player"想定）
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            Vector3 startPos = new Vector3(0, 0.5f, 0);
            player.transform.position = startPos;
        }
    }
}
