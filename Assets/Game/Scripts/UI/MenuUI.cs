using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [Header("�X�e�[�W�ݒ�")]
    public bool[] setting; //�ݒ荀��(�}�b�v�I���E�X�e�[�W�ݒ�)
    public int mapNum;     //�}�b�v�ԍ�
    public int mapMaxNum;  //�}�b�v�ԍ��̍ő�l

    [Header("�ݒ蒆�t���O")]
    public bool[] isSetStage; //�}�b�v
    public int setNum;        //�ݒ荀�ڂ̎��ʔԍ�

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

    enum Set
    {
        MAP,
        STAGE,
        ITEM,
        TREASURE,
    }

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
        //�I�����
        setting[(int)Set.MAP] = true;
        isSetStage[(int)Set.STAGE] = true;
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



        if (setting[(int)Set.STAGE])
        {
            ChangeSetting();
            //�}�b�v�ݒ�
            if (isSetStage[(int)Set.STAGE]) 
                StageSetting();
            //�I�u�W�F�N�g�ݒ�
            {
                //�^�J�����m�ݒ�
                if (isSetStage[(int)Set.TREASURE])
                    gameManager.setTreasure = ObjectSetting(gameManager.setTreasure, (gameManager.basicSetTreasure + gameManager.setStage));
                //�z���_�V���m�ݒ�
                if (isSetStage[(int)Set.ITEM])
                    gameManager.setItem = ObjectSetting(gameManager.setItem, (gameManager.basicSetItem + gameManager.setStage));
            }
        }
    }
    //�I�����ڂ�ύX
    void ChangeSetting()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            setNum++;
            if(setNum>isSetStage.Length)
            {
                setNum = 0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            setNum--;
            if (setNum < 0)
            {
                setNum = isSetStage.Length;
            }
        }
        isSetStage[setNum] = true;
    }


    //�X�e�[�W�ݒ�
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
