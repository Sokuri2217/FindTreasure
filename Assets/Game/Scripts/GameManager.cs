using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NUnit.Framework;

public class GameManager : MonoBehaviour
{
    [Header("�C���X�^���X�ێ�")]
    protected static GameManager Instance { get; private set; }

    [Header("�X�e�[�W�ݒ�")]
    public int setMaxStage;      //�}�b�v�̍ő�T�C�Y
    public int setMinStage;      //�}�b�v�̍ŏ��T�C�Y
    public int setStage;         //�}�b�v�̌��݃T�C�Y
    public int setTreasure;      //�^�J�����m�̐�
    public int setItem;          //�z���_�V���m�̐�
    public int basicSetTreasure; //�^�J�����m�̊�l
    public int basicSetItem;     //�z���_�V���m�̊�l

    [Header("BGM")]
    public AudioSource bgm;

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
            setStage = setMinStage;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //�X�e�[�W�ݒ�
        {
            //�}�b�v��ɂ�����^�J�����m�ƃz���_�V���m�̏���ݒ�
            CreateStageLimit(setTreasure, basicSetTreasure);
            CreateStageLimit(setItem, basicSetItem);
        }
    }

    //BGM���Đ�
    public void PlayBGM(AudioClip clip)
    {
        bgm.clip = clip;
        bgm.Play();
    }

    //�}�b�v�����ɂ��������ݒ�
    public void CreateStageLimit(int set, int basicSet)
    {
        //�}�b�v�T�C�Y�ɉ���������𒴂��Ȃ��悤�ɂ���
        //�^�J�����m
        if (set > (basicSet + setStage))
            set = (basicSet + setStage);
    }
}
