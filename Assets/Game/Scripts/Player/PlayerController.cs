using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("基本情報")]
    public float originSpeed; //基本速度
    public float moveSpeed;   //現在速度
    public int dig_width;     //採掘範囲(横)
    public int dig_height;    //採掘範囲(縦)
    public int digPower;      //採掘力(一回でどれだけ深度を下げれるか)
    public bool isDig;        //採掘中かどうか
    public int digLimit;      //採掘回数の上限
    public int digCurrent;    //現在の採掘回数
    public bool getItem;      //アイテムを取得可能かどうか

    [Header("移動関係")]
    public bool isMoving;           //移動中かどうか
    private Vector3 targetPosition;  //移動先の座標
    private Vector3 moveDirection;   //移動ベクトル 

    [Header("アクティブ効果発動中")]
    public List<ItemBase> isActiveItems = new List<ItemBase>();

    [Header("スクリプト参照")]
    public GameManager gameManager; //ゲームの基本情報
    public StageUI stageUI;         //ステージ進行
    public Inventory inventory;     //所持アイテム
    public ItemObject hitItem;      //取得可能アイテム

    void Start()
    {
        //スクリプト取得
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        stageUI = GameObject.Find("StageUI").GetComponent<StageUI>();
        inventory = GetComponent<Inventory>();

        //初回設定
        moveSpeed = originSpeed;             //移動速度
        targetPosition = transform.position; //座標
        digCurrent = digLimit;               //採掘回数

    }

    void Update()
    {
        //グリッド移動
        GridMove();
        //採掘処理
        GridDig();
        //採掘範囲の増減制限
        DigAreaLimit();
        //アイテム取得
        GetItem();
        //プレイヤー回転
        RotationPlayer();
    }
    private void GridMove()
    {
        if (!isMoving && stageUI.isPhase[(int)Phase.DIG])
        {
            float horizontal = 0.0f;
            float vertical = 0.0f;
            moveSpeed = originSpeed;

            // WASDで移動
            if (Input.GetKey(KeyCode.W))
                vertical = 1.0f;
            else if (Input.GetKey(KeyCode.S))
                vertical = -1.0f;
            else if (Input.GetKey(KeyCode.A))
                horizontal = -1.0f;
            else if (Input.GetKey(KeyCode.D))
                horizontal = 1.0f;

            //Shift入力時は加速
            if(Input.GetKey(KeyCode.LeftShift))
            {
                moveSpeed = originSpeed * 2.0f;
            }

            if (horizontal != 0.0f || vertical != 0.0f)
            {
                moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;
                //ベクトルを四捨五入する ← 小数だとグリッド移動がバグる可能性がある
                moveDirection = new Vector3(
                    Mathf.Round(moveDirection.x),
                    0.0f,
                    Mathf.Round(moveDirection.z)
                    );
                //移動先のベクトルを求める
                Vector3 nextPosition = targetPosition + moveDirection;

                //マップ外 or 障害物チェックを追加可能
                if (IsWithinBounds(nextPosition))
                {
                    StartCoroutine(MoveTo(nextPosition));
                }
            }
        }
        else
        {
            // なめらかに移動する（MoveTowardsで補間）
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }
    IEnumerator MoveTo(Vector3 destination)
    {
        isDig = false;
        isMoving = true;
        targetPosition = destination;

        // 移動完了まで待機（小さな距離まで補間）
        while ((transform.position - destination).sqrMagnitude > 0.01f)
        {
            yield return null;
        }

        transform.position = destination; // 最終位置をスナップ
        isMoving = false;
    }
    // マップ範囲内か確認
    bool IsWithinBounds(Vector3 pos)
    {
        //移動先がマップ外ではない → true
        //マップ外 → false
        return pos.x >= 0.0f && pos.x < (gameManager.setStage * 10) && pos.z >= 0.0f && pos.z < (gameManager.setStage * 10);
    }

    //採掘処理
    private void GridDig()
    {
        //入力
        if (Input.GetKeyDown(KeyCode.Space) && stageUI.isPhase[(int)Phase.DIG] && !isDig)
        {
            isDig = true;
            digCurrent--;
        }
        else
        {
            isDig = false;
        }
    }

    //採掘範囲の増減制限
    public void DigAreaLimit()
    {
        if (dig_width < 0) 
        {
            dig_width = 0;
        }
        if (dig_height < 0)
        {
            dig_height = 0;
        }
    }

    //アイテム取得
    private void GetItem()
    {
        if (Input.GetKeyDown(KeyCode.G) && getItem) 
        {
            ItemBase item = hitItem.itemBase;
            bool addItem = inventory.AddItem(item);
            if (addItem)
            {
                getItem = false;
                Destroy(hitItem.gameObject);
            }
        }
    }

    //プレイヤー回転(カメラは常に後方から)
    private void RotationPlayer()
    {
        if (!stageUI.isPhase[(int)Phase.ITEM])
        {
            //左右に回転
            if (Input.GetKeyDown(KeyCode.J))
                transform.Rotate(0.0f, -90.0f, 0.0f);
            else if (Input.GetKeyDown(KeyCode.L))
                transform.Rotate(0.0f, 90.0f, 0.0f);
        }
    }

    //アイテム取得
    public void OnTriggerStay(Collider other)
    {
        if (stageUI.isPhase[(int)Phase.DIG]) 
        {
            if (other.gameObject.CompareTag("Item"))
            {
                getItem = true;
                hitItem = other.GetComponent<ItemObject>();
            }
        }
    }
    //アイテム取得
    public void OnTriggerExit(Collider other)
    {
        if (stageUI.isPhase[(int)Phase.DIG])
        {
            if (other.gameObject.CompareTag("Item"))
            {
                getItem = false;
                hitItem = null;            
            }
        }
    }
}
