using UnityEngine;

public class MapGimmick : MonoBehaviour
{
    [Header("スクリプト参照")]
    public StageUI stageUI;
    public PlayerController player;
    public GameManager gameManager;
    public MapCreater mapCreater;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //スクリプト取得
        mapCreater = GetComponent<MapCreater>();
        stageUI = GameObject.Find("StageUI").GetComponent<StageUI>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    public void SetGimmick(int gimmickNum)
    {
        if (player == null) return;

        switch(gimmickNum)
        { 
            case (int)MapNum.MAP1:
                //特にギミックはなし
                break;
            case (int)MapNum.MAP2:
                player.originSpeed *= 0.75f;                //移動速度低下
                player.digLimit -= 1;                       //採掘上限低下
                mapCreater.minDeep[(int)Deep.TREASURE] = 2; //最小シンド低下
                mapCreater.maxDeep[(int)Deep.TREASURE] = 4; //最大シンド低下
                break;
            case (int)MapNum.MAP3:
                player.originSpeed *= 1.2f;                 //移動速度上昇
                mapCreater.minDeep[(int)Deep.TREASURE] = 4; //最小シンド上昇
                mapCreater.maxDeep[(int)Deep.TREASURE] = 8; //最大シンド上昇
                stageUI.phaseLimit[(int)Phase.DIG] *= 0.7f; //採掘フェーズの維持時間を短縮
                break;
            case (int)MapNum.MAP4:
                player.useItem += 1;                        //使用アイテム数上昇
                player.digLimit -= 3;                       //採掘上限大低下
                mapCreater.minDeep[(int)Deep.TREASURE] = 2; //最小シンド低下
                mapCreater.maxDeep[(int)Deep.TREASURE] = 4; //最大シンド低下
                break;
            case (int)MapNum.MAP5:
                player.originSpeed *= 0.7f;                  //移動速度低下
                mapCreater.minDeep[(int)Deep.TREASURE] = 6;  //最小シンド上昇
                mapCreater.maxDeep[(int)Deep.TREASURE] = 10; //最大シンド上昇
                mapCreater.minDeep[(int)Deep.ITEM] = 3;      //ホリダシモノにシンドを追加
                mapCreater.maxDeep[(int)Deep.ITEM] = 6;      //ホリダシモノにシンドを追加
                break;
        }
    }

    enum MapNum
    {
        MAP1,
        MAP2,
        MAP3,
        MAP4,
        MAP5,
    }
}
