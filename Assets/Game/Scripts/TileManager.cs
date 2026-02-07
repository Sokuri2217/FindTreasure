using UnityEngine;

public class TileManager : MonoBehaviour
{
    [Header("タイルの基本情報")]
    public bool hasTreasure;
    public bool hasItem;
    public GameObject treasureObj;
    public GameObject itemObj;
    public int deep;
    public bool dig;

    private GameObject player;
    private PlayerController playerController;
    private Renderer tileRenderer;

    private Vector3Int tilePos;   // タイル座標を整数で保持

    void Start()
    {
        tileRenderer = GetComponent<Renderer>();
        tilePos = Vector3Int.RoundToInt(transform.position);
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            playerController = player.GetComponent<PlayerController>();
        }

        UpdateDigRange();
        DigGrid();
        ChangeGridColor();
    }

    // 採掘範囲判定（軽量版）
    void UpdateDigRange()
    {
        dig = false;

        Vector3Int playerPos = Vector3Int.RoundToInt(player.transform.position);

        int halfW = playerController.dig_width / 2;
        int halfH = playerController.dig_height / 2;

        Vector3Int delta = tilePos - playerPos;

        // プレイヤーの向き（Y軸 90度単位想定）
        int dir = Mathf.RoundToInt(player.transform.eulerAngles.y) % 360;

        int x = delta.x;
        int z = delta.z;

        // 向きによる座標変換（Quaternion不使用）
        switch (dir)
        {
            case 90:
                (x, z) = (-z, x);
                break;
            case 180:
                x = -x; z = -z;
                break;
            case 270:
                (x, z) = (z, -x);
                break;
        }

        if (Mathf.Abs(x) <= halfW && Mathf.Abs(z) <= halfH)
        {
            // 十字判定
            dig = (x == 0 || z == 0);
        }
    }

    void DigGrid()
    {
        if (!playerController.isDig || !dig) return;

        deep -= playerController.digPower;
        if (deep > 0) return;

        if (hasTreasure)
        {
            hasTreasure = false;
            SpawnObject(treasureObj);
        }
        else if (hasItem)
        {
            hasItem = false;
            SpawnObject(itemObj);

            ItemObject itemObject = itemObj.GetComponent<ItemObject>();
            GameManager gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

            itemObject.itemBase =
                Random.Range(0, 1000) <= 4 ?
                gm.uniqueItems[Random.Range(0, gm.uniqueItems.Count)] :
                gm.items[Random.Range(0, gm.items.Count)];
        }
    }

    void SpawnObject(GameObject obj)
    {
        Vector3 pos = new Vector3(
            transform.position.x,
            obj.transform.position.y,
            transform.position.z
        );
        Instantiate(obj, pos, Quaternion.identity);
    }

    void ChangeGridColor()
    {
        tileRenderer.material.color = dig ? Color.red : Color.white;
    }

    public void SetHasItem(bool value)
    {
        hasItem = value;
    }
    public void SetHasTreasure(bool value)
    {
        hasTreasure = value;
    }
}
