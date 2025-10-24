using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [Header("ステージ設定")]
    public bool[] setting; //設定項目(マップ選択・ステージ設定)
    public int mapNum;     //マップ番号
    public int mapMaxNum;  //マップ番号の最大値

    [Header("設定中フラグ")]
    public bool[] isSetStage; //マップ
    public int setNum;        //設定項目の識別番号

    [Header("シーン移動")]
    public string sceneName;
    public bool changeScene;

    [Header("描画系")]
    public Image enterToStart;
    public float fadeLimit;
    public float fadeTimer;
    public bool fadeIn;
    public bool fadeOut;

    [Header("BGM")]
    public AudioClip bgm;

    [Header("スクリプト参照")]
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
        //スクリプト取得
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        //初回設定
        //点滅するUIはフェードアウトからスタート
        fadeIn = false;
        fadeOut = true;
        fadeTimer = fadeLimit;
        //選択状態
        setting[(int)Set.MAP] = true;
        isSetStage[(int)Set.STAGE] = true;
        //BGM
        gameManager.PlayBGM(bgm);
    }

    // Update is called once per frame
    void Update()
    {
        //Enterでゲーム開始
        if (Input.GetKeyDown(KeyCode.Return) && !changeScene) 
        {
            SceneManager.LoadScene(sceneName);
            changeScene = true;
        }

        //UIの点滅
        FadeInOut();

        //シーン移動が始まると他の入力は出来ない
        if (changeScene) return;



        if (setting[(int)Set.STAGE])
        {
            ChangeSetting();
            //マップ設定
            if (isSetStage[(int)Set.STAGE]) 
                StageSetting();
            //オブジェクト設定
            {
                //タカラモノ設定
                if (isSetStage[(int)Set.TREASURE])
                    gameManager.setTreasure = ObjectSetting(gameManager.setTreasure, (gameManager.basicSetTreasure + gameManager.setStage));
                //ホリダシモノ設定
                if (isSetStage[(int)Set.ITEM])
                    gameManager.setItem = ObjectSetting(gameManager.setItem, (gameManager.basicSetItem + gameManager.setStage));
            }
        }
    }
    //選択項目を変更
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


    //ステージ設定
    void StageSetting()
    {
        //マップの広さ
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

    //オブジェクト設定
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

    //UIのフェード処理
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

        //一部画像を点滅させる
        Color color = enterToStart.color;
        color.a = (fadeTimer / fadeLimit);
        enterToStart.color = color;
    }
}
