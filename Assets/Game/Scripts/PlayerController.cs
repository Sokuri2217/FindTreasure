using UnityEngine;
using System.Collections;
using System.Drawing;

public class PlayerController : MonoBehaviour
{
    public float originSpeed;   //��{���x
    public float moveSpeed;   �@//���ݑ��x

    private bool isMoving;          // �ړ����͎��̓��͂��󂯕t���Ȃ�
    private MapCreater mapCreater;  //�}�b�v�͈̔͂��擾
    private Vector3 targetPosition; // �ړ���̍��W�i�ړI�n�j
    private Vector3 moveDirection;
    private GameObject rotationCore; //�J�����̉�]��
    public bool isTopView; //�J�����������낵��Ԃ��ǂ���
    public int dig_width;
    public int dig_height;
    public bool isDig;
    public int digPower;

    void Start()
    {
        //�X�N���v�g�擾
        mapCreater = GameObject.Find("GridManager").GetComponent<MapCreater>();

        //�I�u�W�F�N�g�擾
        rotationCore = GameObject.Find("rotationCore");

        // �����ʒu��ۑ�
        targetPosition = transform.position;

        //����ݒ�
        moveSpeed = originSpeed;
    }

    void Update()
    {
        //�O���b�h�ړ�
        GridMove();
        //�̌@����
        GridDig();
        //�v���C���[��]
        RotationPlayer();
    }
    private void GridMove()
    {
        if (!isMoving)
        {
            float horizontal = 0.0f;
            float vertical = 0.0f;
            moveSpeed = originSpeed;

            // WASD�ňړ�
            if (Input.GetKey(KeyCode.W))
                vertical = 1.0f;
            else if (Input.GetKey(KeyCode.S))
                vertical = -1.0f;
            else if (Input.GetKey(KeyCode.A))
                horizontal = -1.0f;
            else if (Input.GetKey(KeyCode.D))
                horizontal = 1.0f;

            //Shift���͎��͉���
            if(Input.GetKey(KeyCode.LeftShift))
            {
                moveSpeed = originSpeed * 2.0f;
            }

            if (horizontal != 0.0f || vertical != 0.0f)
            {
                moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;
                //�x�N�g�����l�̌ܓ����� �� �������ƃO���b�h�ړ����o�O��\��������
                moveDirection = new Vector3(
                    Mathf.Round(moveDirection.x),
                    0.0f,
                    Mathf.Round(moveDirection.z)
                    );
                //�ړ���̃x�N�g�������߂�
                Vector3 nextPosition = targetPosition + moveDirection;

                //�}�b�v�O or ��Q���`�F�b�N��ǉ��\
                if (IsWithinBounds(nextPosition))
                {
                    StartCoroutine(MoveTo(nextPosition));
                }
            }
        }
        else
        {
            // �Ȃ߂炩�Ɉړ�����iMoveTowards�ŕ�ԁj
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }
    IEnumerator MoveTo(Vector3 destination)
    {
        isMoving = true;
        targetPosition = destination;

        // �ړ������܂őҋ@�i�����ȋ����܂ŕ�ԁj
        while ((transform.position - destination).sqrMagnitude > 0.01f)
        {
            yield return null;
        }

        transform.position = destination; // �ŏI�ʒu���X�i�b�v
        isMoving = false;
    }
    // �}�b�v�͈͓����m�F
    bool IsWithinBounds(Vector3 pos)
    {
        //�ړ��悪�}�b�v�O�ł͂Ȃ� �� true
        //�}�b�v�O �� false
        return pos.x >= 0.0f && pos.x < mapCreater.mapSize && pos.z >= 0.0f && pos.z < mapCreater.mapSize;
    }

    //�̌@����
    private void GridDig()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isDig) 
        {
            isDig = true;
        }
    }

    //�v���C���[��](�J�����͏�Ɍ������)
    private void RotationPlayer()
    {
        //���E�ɉ�]
        if (Input.GetKeyDown(KeyCode.J))
            transform.Rotate(0.0f, -90.0f, 0.0f);
        else if (Input.GetKeyDown(KeyCode.L))
            transform.Rotate(0.0f, 90.0f, 0.0f);

    }
}
