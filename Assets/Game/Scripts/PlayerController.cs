using UnityEngine;
using System.Collections;
using System.Drawing;

public class PlayerController : MonoBehaviour
{
    public float originSpeed;   //基本速度
    public float moveSpeed;   　//現在速度

    private bool isMoving;          // 移動中は次の入力を受け付けない
    private MapCreater mapCreater;  //マップの範囲を取得
    private Vector3 targetPosition; // 移動先の座標（目的地）
    private Vector3 moveDirection;
    private GameObject rotationCore; //カメラの回転軸
    public bool isTopView; //カメラが見下ろし状態かどうか
    public int dig_width;
    public int dig_height;
    public bool isDig;
    public int digPower;

    void Start()
    {
        //スクリプト取得
        mapCreater = GameObject.Find("GridManager").GetComponent<MapCreater>();

        //オブジェクト取得
        rotationCore = GameObject.Find("rotationCore");

        // 初期位置を保存
        targetPosition = transform.position;

        //初回設定
        moveSpeed = originSpeed;
    }

    void Update()
    {
        //グリッド移動
        GridMove();
        //採掘処理
        GridDig();
        //プレイヤー回転
        RotationPlayer();
    }
    private void GridMove()
    {
        if (!isMoving)
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
        return pos.x >= 0.0f && pos.x < mapCreater.mapSize && pos.z >= 0.0f && pos.z < mapCreater.mapSize;
    }

    //採掘処理
    private void GridDig()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isDig) 
        {
            isDig = true;
        }
    }

    //プレイヤー回転(カメラは常に後方から)
    private void RotationPlayer()
    {
        //左右に回転
        if (Input.GetKeyDown(KeyCode.J))
            transform.Rotate(0.0f, -90.0f, 0.0f);
        else if (Input.GetKeyDown(KeyCode.L))
            transform.Rotate(0.0f, 90.0f, 0.0f);

    }
}
