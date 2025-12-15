using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : UIManager
{
    [Header("主な設定項目(マップ選択・ステージ設定・その他)")]
    public bool[] setting;       //設定項目(マップ選択・ステージ設定・その他)
    public int settingNum;       //選択項目の識別番号
    public Image[] settingImage; //UI反映
    public Vector3[] originScale;//通常サイズ
    public float zoomNum;        //拡大率

    [Header("ステージ選択")]
    public Text stageNum;
    public Text clearTurnNum;
    public Text gimmick;

    [Header("設定中フラグ(マップ・ホリダシモノ・タカラモノ)")]
    public bool[] isSetStage; //
    public int setNum;        //設定項目の識別番号
    public Text[] isSetText;  //
    public Transform inputNaviImage;
    public Transform[] setSlotPos;
    public GameObject[] settingPanel;
    public GameObject focusPanel;
    public Image focusIcon;
    public Sprite[] focusSprite;
    public Text focusText;
    [TextArea(2, 5)] public string[] explanation;

    [Header("その他")]
    public GameObject other;
    public Image[] otherMenu;
    public Vector3[] originOtherScale;
    public float zoomOtherScale;
    public GameObject[] otherPanel;
    public int otherPanelNum;
    public int maxOtherPanel;
    public Text otherText;
    [TextArea(2, 5)] public string[] otherExplanation;
    public bool isOpenOtherMenu;

    [Header("シーン移動")]
    public string[] stageName;
    public string titleName;
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

    enum SetOther
    {
        SOUND,    //BGM/SE
        GAME,     //UI説明
        LANGUAGE, //用語説明
        ENDGAME,  //ゲーム終了
    }

    enum  ActiveObj
    {
        Open,
        Close
    }

    enum SE
    {
        SELECTMODE,
        SELECTNUM,
        SELECTSTAGESET,
        OPENFOCUS,
        CLOSEFOCUS,

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
            originScale[i] = settingImage[i].transform.localScale;

        for (int j = 0; j < otherMenu.Length; j++)
        {
            originOtherScale[j] = otherMenu[j].transform.localScale;
            otherPanel[j].SetActive(false);
        }
            

        //パネル
        focusPanel.SetActive(false);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        //タイトルに戻る
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            sceneName = titleName;
            fadeState = (int)FadeState.END;
            StartCoroutine(SceneMove());
        }
        //ゲーム開始
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            sceneName = stageName[gameManager.mapNum];
            fadeState = (int)FadeState.END;
            StartCoroutine(SceneMove());
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
        //選択項目をテキストとして表示
        SetUIText();
        //設定画面の各項目説明
        SetStageExplanation();
        //その他の設定
        SetOtherMode();
    }

    //選択中の設定画面
    void SelectSettingMode()
    {
        for (int i = 0; i < settingPanel.Length; i++) 
        {
            settingPanel[i].SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            setting[settingNum] = false;
            settingNum--;
            if (settingNum < 0)
            {
                settingNum = (setting.Length - 1);
            }
            //SEを再生
            seManager.PlaySE(se[(int)SE.SELECTMODE]);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            setting[settingNum] = false;
            settingNum++;
            if (settingNum >= setting.Length)
            {
                settingNum = 0;
            }
            //SEを再生
            seManager.PlaySE(se[(int)SE.SELECTMODE]);
        }
        setting[settingNum] = true;
        settingPanel[settingNum].SetActive(true);
    }

    //マップを変更
    void ChangeMap()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            gameManager.mapNum--;
            if (gameManager.mapNum < 0)
                gameManager.mapNum = (gameManager.mapMaxNum - 1);
            //SEを再生
            seManager.PlaySE(se[(int)SE.SELECTNUM]);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            gameManager.mapNum++;
            if (gameManager.mapNum >= gameManager.mapMaxNum)
                gameManager.mapNum = 0;
            //SEを再生
            seManager.PlaySE(se[(int)SE.SELECTNUM]);
        }
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
            //SEを再生
            seManager.PlaySE(se[(int)SE.SELECTSTAGESET]);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            isSetStage[setNum] = false;
            setNum--;
            if (setNum < 0)
            {
                setNum = (isSetStage.Length - 1);
            }
            //SEを再生
            seManager.PlaySE(se[(int)SE.SELECTSTAGESET]);
        }
        isSetStage[setNum] = true;
        inputNaviImage.position = setSlotPos[setNum].position;
        focusText.text = explanation[setNum].ToString();
    }

    //設定画面の各項目説明
    void SetStageExplanation()
    {
        if (!setting[(int)Set.STAGE]) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            //閉じる
            if(focusPanel.activeSelf)
            {
                focusPanel.SetActive(false);
                focusIcon.sprite = focusSprite[(int)ActiveObj.Open];
                //SEを再生
                seManager.PlaySE(se[(int)SE.CLOSEFOCUS]);
            }
            //開く
            else
            {
                focusPanel.SetActive(true);
                focusIcon.sprite = focusSprite[(int)ActiveObj.Close];
                //SEを再生
                seManager.PlaySE(se[(int)SE.OPENFOCUS]);
            }
        }
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
            //SEを再生
            seManager.PlaySE(se[(int)SE.SELECTNUM]);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            gameManager.setStage--;
            if (gameManager.setStage < gameManager.setMinStage)
            {
                gameManager.setStage = gameManager.setMaxStage;
            }
            //SEを再生
            seManager.PlaySE(se[(int)SE.SELECTNUM]);
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
            //SEを再生
            seManager.PlaySE(se[(int)SE.SELECTNUM]);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            setObj--;
            if (setObj < 1) 
            {
                setObj = setMaxObj;
            }
            //SEを再生
            seManager.PlaySE(se[(int)SE.SELECTNUM]);
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
                settingImage[i].color = Color.white;
            }
            else
            {
                settingImage[i].transform.localScale = originScale[i];
                Color32 color = new Color32(150, 150, 50, 255);
                settingImage[i].color = color;
            }
        }
    }

    //選択項目をテキストとして表示
    public void SetUIText()
    {
        for (int i = 0; i < isSetText.Length; i++) 
        {
            isSetText[(int)SetStage.STAGE].text = gameManager.setStage.ToString();
            isSetText[(int)SetStage.TREASURE].text = gameManager.setTreasure.ToString();
            isSetText[(int)SetStage.ITEM].text = gameManager.setItem.ToString();
        }

        stageNum.text = (gameManager.mapNum + 1).ToString();
        clearTurnNum.text = gameManager.clearTurnLimit[gameManager.mapNum].ToString();
        gimmick.text = gameManager.gimmickDescription[gameManager.mapNum].ToString();
    }

    public void SetOtherMode()
    {
        if (setting[(int)Set.OTHER])
        {
            if (other.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    otherPanelNum = (int)SetOther.SOUND;
                    seManager.PlaySE(se[(int)SE.SELECTNUM]);
                }   
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    otherPanelNum = (int)SetOther.GAME;
                    seManager.PlaySE(se[(int)SE.SELECTNUM]);
                }  
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    otherPanelNum = (int)SetOther.ENDGAME;
                    seManager.PlaySE(se[(int)SE.SELECTNUM]);
                } 
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    otherPanelNum = (int)SetOther.LANGUAGE;
                    seManager.PlaySE(se[(int)SE.SELECTNUM]);
                }
                    

                //選択中の項目を強調表示
                for (int i = 0; i < otherMenu.Length; i++)
                {
                    if (i == otherPanelNum)
                    {
                        Transform imageScale = otherMenu[i].transform;
                        imageScale.localScale = new Vector3(
                            (originOtherScale[i].x * zoomOtherScale),
                            (originOtherScale[i].y * zoomOtherScale),
                            (originOtherScale[i].z * zoomOtherScale)
                            );
                        otherMenu[i].transform.localScale = imageScale.localScale;
                        otherMenu[i].color = Color.white;
                    }
                    else
                    {
                        otherMenu[i].transform.localScale = originOtherScale[i];
                        Color32 color = new Color32(150, 150, 50, 255);
                        otherMenu[i].color = color;
                    }
                }

                //選択項目の内容
                otherText.text = otherExplanation[otherPanelNum].ToString();
            }

            if (Input.GetKeyDown(KeyCode.Space)) 
            {
                if (!isOpenOtherMenu)
                {
                    isOpenOtherMenu = true;
                    other.SetActive(false);
                    otherPanel[otherPanelNum].gameObject.SetActive(true);
                }
                else if (isOpenOtherMenu) 
                {
                    isOpenOtherMenu = false;
                    other.SetActive(true);
                    otherPanel[otherPanelNum].gameObject.SetActive(false);
                }
                
            }

            for (int i = 0; i < otherPanel.Length; i++) 
            {
                if (otherPanel[i].gameObject.activeSelf)
                {
                    switch (i)
                    {
                        case (int)SetOther.SOUND:
                            OtherSound();
                            break;
                        case (int)SetOther.GAME:
                            OtherGame();
                            break;
                        case (int)SetOther.LANGUAGE:
                            OtherLanguage();
                            break;
                        case (int)SetOther.ENDGAME:
                            OtherEndGame();
                            break;
                        default: 
                            break;
                    }
                }
            }

            
        }

    }

    public void OtherSound()
    {

    }
    public void OtherGame()
    {

    }
    public void OtherLanguage()
    {

    }
    public void OtherEndGame()
    {

    }
}
