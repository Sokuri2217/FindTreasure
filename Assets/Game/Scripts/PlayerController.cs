using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;    // プレイヤーの移動速度
    private bool isMoving = false;  // 移動中は次の入力を受け付けない

    private MapCreater mapCreater;

    private Vector3 targetPosition; // 移動先の座標（目的地）

    void Start()
    {
        mapCreater = GameObject.Find("GridManager").GetComponent<MapCreater>();

        targetPosition = transform.position; // 初期位置を保存
    }

    void Update()
    {
        if (!isMoving)
        {
            Vector3 direction = Vector3.zero;

            // 矢印キーまたはWASDで入力方向を決定
            if (Input.GetKeyDown(KeyCode.W))
                direction = Vector3.forward;
            else if (Input.GetKeyDown(KeyCode.S))
                direction = Vector3.back;
            else if (Input.GetKeyDown(KeyCode.A))
                direction = Vector3.left;
            else if (Input.GetKeyDown(KeyCode.D))
                direction = Vector3.right;

            if (direction != Vector3.zero)
            {
                Vector3 nextPosition = targetPosition + direction;

                // TODO: マップ外 or 障害物チェックを追加可能
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

    // マップ範囲内か確認（10x10のグリッドを想定）
    bool IsWithinBounds(Vector3 pos)
    {
        return pos.x >= 0 && pos.x < mapCreater.width && pos.z >= 0 && pos.z < mapCreater.height;
    }
}
