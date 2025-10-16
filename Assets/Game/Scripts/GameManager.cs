using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("�C���X�^���X�ێ�")]
    protected static GameManager Instance { get; private set; }

    [Header("�V�[���ړ��֘A")]
    public bool inputCheck;    //���͋���
    public Image fadeImage;    //�Ó]�E���]�p�I�u�W�F�N�g
    public float fadeDuration; //�Ó]�E���]�ɂ����鎞��

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //�V���O���g��
        {
            // ���łɃC���X�^���X�����݂���ꍇ�͍폜
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject); // �V�[�����܂����ŃI�u�W�F�N�g��ێ�
        }
    }

    // Update is called once per frame
    void Update()
    {
        //���͋�������Ƃ��̂ݏ������s��
        if (!inputCheck) return;


    }
}
