using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [Header("設定中フラグ")]
    public bool isSetStage;    //マップ
    public bool isSetTreasure; //タカラモノの個数
    public bool isSetItem;     //ホリダシモノの個数

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

        //マップ設定
        if(isSetStage)
            StageSetting();
        //オブジェクト設定
        {
            //タカラモノ設定
            if (isSetTreasure)
                gameManager.setTreasure = ObjectSetting(gameManager.setTreasure, (gameManager.basicSetTreasure + gameManager.setStage));
            //ホリダシモノ設定
            if (isSetItem)
                gameManager.setItem = ObjectSetting(gameManager.setItem, (gameManager.basicSetItem + gameManager.setStage));
        }
        
    }

    //マップ設定
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
