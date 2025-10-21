using UnityEngine;

public class MenuUI : MonoBehaviour
{
    [Header("設定中フラグ")]
    public bool isSetStage;    //マップ
    public bool isSetTreasure; //タカラモノの個数
    public bool isSetItem;     //ホリダシモノの個数

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
        //BGM
        gameManager.PlayBGM(bgm);
    }

    // Update is called once per frame
    void Update()
    {
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
}
