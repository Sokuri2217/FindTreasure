using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuUI : UIManager
{
    [Header("主な設定項目(マップ選択・ステージ設定・その他)")]
    public bool[] setting;       //設定項目(マップ選択・ステージ設定・その他)
    public int settingNum;       //選択項目の識別番号
    public Image[] settingImage; //UI反映
    public Vector3[] originScale;  //通常サイズ
    public float zoomNum;        //拡大率

    [Header("ステージ設定")]
    public int mapNum;     //マップ番号
    public int mapMaxNum;  //マップ番号の最大値

    [Header("設定中フラグ(マップ・ホリダシモノ・タカラモノ)")]
    public bool[] isSetStage; //
    public int setNum;        //設定項目の識別番号

    [Header("シーン移動")]
    public string[] stageName;
    public bool changeScene;

    [Header("描画系")]
    public Image enterToStart;
    public float fadeLimit;
    public float fadeTimer;
    public bool fadeIn;
    public bool fadeOut;

    enum Set
    {
        MAP,
        STAGE,
        OTHER,
    }

    enum SetStage
    {
        STAGE,
        ITEM,
        TREASURE,
    }

    enum SE
    {
        SELECTSET,
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        //初回設定
        //点滅するUIはフェードアウトからスタート
        fadeIn = false;
        fadeOut = true;
        fadeTimer = fadeLimit;
        //選択状態
        setting[(int)Set.MAP] = true;
        isSetStage[(int)SetStage.STAGE] = true;

        for (int i = 0; i < setting.Length; i++)
        {
            originScale[i] = settingImage[i].transform.localScale;
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        //Enterでゲーム開始
        if (Input.GetKeyDown(KeyCode.Return)) 
        {
            fadeState = (int)FadeState.END;
            StartCoroutine(SceneMove());
            SceneManager.LoadScene(sceneName);
        }

        //UIの点滅
        FadeInOut();

        //選択中の設定画面
        SelectSettingMode();

        //マップ選択
        if(setting[(int)Set.MAP])
        {
            ChangeMap();
        }
        else if (setting[(int)Set.STAGE])
        {
            ChangeSetting();
            //マップ設定
            if (isSetStage[(int)SetStage.STAGE]) 
                StageSetting();

            //オブジェクト設定
            //タカラモノ設定
            if (isSetStage[(int)SetStage.TREASURE])
                gameManager.setTreasure = ObjectSetting(gameManager.setTreasure, (gameManager.basicSetTreasure + gameManager.setStage));
            //ホリダシモノ設定
            if (isSetStage[(int)SetStage.ITEM])
                gameManager.setItem = ObjectSetting(gameManager.setItem, (gameManager.basicSetItem + gameManager.setStage));
        }

        //選択項目のGUIを制御
        SetUISelectImage();
    }

    //選択中の設定画面
    void SelectSettingMode()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            setting[settingNum] = false;
            settingNum--;
            if (settingNum < 0)
            {
                settingNum = (setting.Length - 1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            setting[settingNum] = false;
            settingNum++;
            if (settingNum >= setting.Length)
            {
                settingNum = 0;
            }
        }
        setting[settingNum] = true;
    }

    //マップを変更
    void ChangeMap()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            mapNum--;
            if (mapNum < 0) 
                mapNum = (mapMaxNum - 1);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            mapNum++;
            if (mapNum >= mapMaxNum) 
                mapNum = 0;
        }

        sceneName = stageName[mapNum];
    }

    //選択項目を変更
    void ChangeSetting()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            isSetStage[setNum] = false;
            setNum++;
            if (setNum >= isSetStage.Length) 
            {
                setNum = 0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            isSetStage[setNum] = false;
            setNum--;
            if (setNum < 0)
            {
                setNum = (isSetStage.Length - 1);
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

    //選択項目のGUIを制御
    public void SetUISelectImage()
    {
        //設定項目の選択
        for (int i = 0; i < setting.Length; i++) 
        {
            if (setting[i]) 
            {
                Transform imageScale = settingImage[i].transform;
                imageScale.localScale = new Vector3(
                    (originScale[i].x * zoomNum),
                    (originScale[i].y * zoomNum),
                    (originScale[i].z * zoomNum)
                    );
                settingImage[i].transform.localScale = imageScale.localScale;
            }
            else
            {
                settingImage[i].transform.localScale = originScale[i];
            }
        }
    }
}
