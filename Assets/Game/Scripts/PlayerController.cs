using UnityEngine;
using System.Collections;
using System.Drawing;

public class PlayerController : MonoBehaviour
{
    [Header("��{���")]
    public float originSpeed;   //��{���x
    public float moveSpeed;   �@//���ݑ��x
    public int dig_width;       //�̌@�͈�(��)
    public int dig_height;      //�̌@�͈�(�c)
    public int digPower;        //�̌@��(���łǂꂾ���[�x��������邩)
    public bool isDig;          //�̌@�����ǂ���

    [Header("�ړ��֌W")]
    private bool isMoving;           //�ړ������ǂ���
    private Vector3 targetPosition;  //�ړ���̍��W
    private Vector3 moveDirection;   //�ړ��x�N�g��

    [Header("�X�N���v�g�Q��")]
    private GameManager gameManager; //�Q�[���̊�{���

    void Start()
    {
        //�X�N���v�g�擾
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        //����ݒ�
        moveSpeed = originSpeed;             //�ړ����x
        targetPosition = transform.position; //���W
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
        return pos.x >= 0.0f && pos.x < (gameManager.setStage * 10) && pos.z >= 0.0f && pos.z < (gameManager.setStage * 10);
    }

    //�̌@����
    private void GridDig()
    {
        //����
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
