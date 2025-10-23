using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [Header("�ݒ蒆�t���O")]
    public bool isSetStage;    //�}�b�v
    public bool isSetTreasure; //�^�J�����m�̌�
    public bool isSetItem;     //�z���_�V���m�̌�

    [Header("�V�[���ړ�")]
    public string sceneName;
    public bool changeScene;

    [Header("�`��n")]
    public Image enterToStart;
    public float fadeLimit;
    public float fadeTimer;
    public bool fadeIn;
    public bool fadeOut;

    [Header("BGM")]
    public AudioClip bgm;

    [Header("�X�N���v�g�Q��")]
    public GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //�X�N���v�g�擾
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        //����ݒ�
        //�_�ł���UI�̓t�F�[�h�A�E�g����X�^�[�g
        fadeIn = false;
        fadeOut = true;
        fadeTimer = fadeLimit;
        //BGM
        gameManager.PlayBGM(bgm);
    }

    // Update is called once per frame
    void Update()
    {
        //Enter�ŃQ�[���J�n
        if (Input.GetKeyDown(KeyCode.Return) && !changeScene) 
        {
            SceneManager.LoadScene(sceneName);
            changeScene = true;
        }

        //UI�̓_��
        FadeInOut();

        //�V�[���ړ����n�܂�Ƒ��̓��͂͏o���Ȃ�
        if (changeScene) return;

        //�}�b�v�ݒ�
        if(isSetStage)
            StageSetting();
        //�I�u�W�F�N�g�ݒ�
        {
            //�^�J�����m�ݒ�
            if (isSetTreasure)
                gameManager.setTreasure = ObjectSetting(gameManager.setTreasure, (gameManager.basicSetTreasure + gameManager.setStage));
            //�z���_�V���m�ݒ�
            if (isSetItem)
                gameManager.setItem = ObjectSetting(gameManager.setItem, (gameManager.basicSetItem + gameManager.setStage));
        }
        
    }

    //�}�b�v�ݒ�
    void StageSetting()
    {
        //�}�b�v�̍L��
        if (Input.GetKeyDown(KeyCode.D))
        {
            gameManager.setStage++;
            if (gameManager.setStage > gameManager.setMaxStage)
            {
                gameManager.setStage = gameManager.setMinStage;
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            gameManager.setStage--;
            if (gameManager.setStage < gameManager.setMinStage)
            {
                gameManager.setStage = gameManager.setMaxStage;
            }
        }
    }

    //�I�u�W�F�N�g�ݒ�
    int ObjectSetting(int setObj, int setMaxObj)
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            setObj++;
            if (setObj > setMaxObj) 
            {
                setObj = 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            setObj--;
            if (setObj < 1) 
            {
                setObj = setMaxObj;
            }
        }

        return setObj;
    }

    //UI�̃t�F�[�h����
    public void FadeInOut()
    {
        if (fadeIn)
        {
            fadeTimer += Time.deltaTime;
            if (fadeTimer >= fadeLimit)
            {
                fadeIn = false;
                fadeOut = true;
            }
        }
        else if (fadeOut)
        {
            fadeTimer -= Time.deltaTime;
            if (fadeTimer <= 0.0f)
            {
                fadeOut = false;
                fadeIn = true;
            }
        }

        //�ꕔ�摜��_�ł�����
        Color color = enterToStart.color;
        color.a = (fadeTimer / fadeLimit);
        enterToStart.color = color;
    }
}
