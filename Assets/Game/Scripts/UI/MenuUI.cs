using UnityEngine;
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
    public bool isInputSpace;

    [Header("音の設定")]
    public bool firstSoundSet;
    public int soundNum;
    public int maxSoundNum;
    public float[] volume;
    public Transform[] soundMeter;
    public GameObject soundMeterImage;
    public Transform[] soundInput;
    public Transform soundInputNavi;
    [Header("ゲームUIの確認")]
    public int gameNum;
    public int maxGameNum;
    [Header("用語の確認")]
    public int languageNum;
    public int maxLanguageNum;
    public Image[] selectLanguageImage;
    public Vector3[] originLanguageScale;
    public float zoomLanguageScale;
    public Text languageText;
    [TextArea(2, 5)] public string[] languageExplanation;
    [Header("ゲームを終了")]
    public int endGameNum;
    public int maxEndGameNum;
    public Image[] selectEndGameImage;
    public Vector3[] originEndGameScale;
    public float zoomEndGameScale;

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
        SELECTSOUNDSET,
    }

    enum Sound
    {
        MASTER,
        BGM,
        SE
    }

    enum LANGUAGE
    {
        TAKARAMONO,
        HORIDASIMONO,
        SINDO,
    }

    enum ChangeNum
    {
        PLUS,
        MINUS
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

        for (int i = 0; i < selectLanguageImage.Length; i++)
            originLanguageScale[i] = selectLanguageImage[i].transform.localScale;
        for (int i = 0; i < selectEndGameImage.Length; i++)
            originEndGameScale[i] = selectEndGameImage[i].transform.localScale;
        

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
        else if (Input.GetKeyDown(KeyCode.Return) && !otherPanel[(int)SetOther.ENDGAME].gameObject.activeSelf) 
        {
            sceneName = stageName[gameManager.mapNum];
            fadeState = (int)FadeState.END;
            StartCoroutine(SceneMove());
        }

        //決定ボタンの長押し防止
        if (Input.GetKeyUp(KeyCode.Space) && isInputSpace)
        {
            isInputSpace = false;
        }

        if (otherPanel[(int)SetOther.ENDGAME].gameObject.activeSelf)
        {
            enterToStart.gameObject.SetActive(false);
        }
        else
        {
            enterToStart.gameObject.SetActive(true);
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

        if(!firstSoundSet)
        {
            //音量
            volume[(int)Sound.BGM] = bgmManager.source.volume;
            volume[(int)Sound.SE] = seManager.source.volume;
            firstSoundSet = true;
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
        if (Input.GetKeyDown(KeyCode.D))
        {
            gameManager.setStage++;
            if (gameManager.setStage > gameManager.setMaxStage)
                gameManager.setStage = gameManager.setMinStage;
            //SEを再生
            seManager.PlaySE(se[(int)SE.SELECTNUM]);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            gameManager.setStage--;
            if (gameManager.setStage < gameManager.setMinStage)
                gameManager.setStage = gameManager.setMaxStage;
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
                //開く
                if (!isOpenOtherMenu)
                {
                    isInputSpace = true;
                    isOpenOtherMenu = true;
                    other.SetActive(false);
                    otherPanel[otherPanelNum].gameObject.SetActive(true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                //閉じる
                if (isOpenOtherMenu)
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
        else
        {
            for (int i = 0; i < otherPanel.Length; i++)
            {
                otherPanel[i].SetActive(false);
            }
            other.SetActive(true);
            isOpenOtherMenu = false;
        }
    }

    public void OtherSound()
    {
        //調整項目を選択
        soundNum = ChangeSelectNum(soundNum, 0, maxSoundNum, Input.GetKeyDown(KeyCode.S), Input.GetKeyDown(KeyCode.W), se[(int)SE.SELECTSOUNDSET]);

        //音量調整
        if (Input.GetKeyDown(KeyCode.A))
        {
            switch(soundNum)
            {
                case (int)Sound.MASTER:
                    volume[(int)Sound.MASTER] -= 0.2f;
                    break;
                case (int)Sound.BGM:
                    bgmManager.source.volume -= 0.2f;
                    volume[(int)Sound.BGM] -= 0.2f;
                    break;
                case (int)Sound.SE:
                    seManager.source.volume -= 0.2f;
                    volume[(int)Sound.SE] -= 0.2f;
                    break;
            }
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            switch (soundNum)
            {
                case (int)Sound.MASTER:
                    volume[(int)Sound.MASTER] += 0.2f;
                    bgmManager.source.volume = volume[(int)Sound.BGM] * volume[(int)Sound.MASTER];
                    seManager.source.volume = volume[(int)Sound.SE] * volume[(int)Sound.MASTER];
                    break;
                case (int)Sound.BGM:
                    bgmManager.source.volume += 0.2f;
                    volume[(int)Sound.BGM] += 0.2f;
                    break;
                case (int)Sound.SE:
                    seManager.source.volume += 0.2f;
                    volume[(int)Sound.SE] += 0.2f;
                    break;
            }
        }

        for (int i = 0; i < volume.Length; i++)
        {
            if (volume[i] <= 0.0f)
                volume[i] = 0.0f;
            if (volume[i] >= 1.0f)
                volume[i] = 1.0f;
        }

        bgmManager.source.volume = volume[(int)Sound.BGM] * volume[(int)Sound.MASTER];
        seManager.source.volume = volume[(int)Sound.SE] * volume[(int)Sound.MASTER];

        //UIに反映
        for (int i = 0; i < volume.Length; i++) 
        {
            // 既存アイコンを削除
            foreach (Transform child in soundMeter[i]) 
            {
                Destroy(child.gameObject);
            }

            // 新しくアイコンを生成
            for (int j = 0; j < Mathf.Round(volume[i] * 5); j++)
            {
                Instantiate(soundMeterImage, soundMeter[i]);
            }

            if (i == soundNum) 
            {
                soundInputNavi.position = new Vector3(
                    soundInputNavi.position.x,
                    soundInput[i].transform.position.y,
                    soundInputNavi.position.z
                    );
            }
        }
    }
    public void OtherGame()
    {

    }
    public void OtherLanguage()
    {
        languageNum = ChangeSelectNum(languageNum, 0, maxLanguageNum, Input.GetKeyDown(KeyCode.S), Input.GetKeyDown(KeyCode.W), null);

        //選択中の項目を強調表示
        for (int i = 0; i < selectLanguageImage.Length; i++)
        {
            if (i == languageNum)
            {
                Transform imageScale = selectLanguageImage[i].transform;
                imageScale.localScale = new Vector3(
                    (originLanguageScale[i].x * zoomLanguageScale),
                    (originLanguageScale[i].y * zoomLanguageScale),
                    (originLanguageScale[i].z * zoomLanguageScale)
                    );
                selectLanguageImage[i].transform.localScale = imageScale.localScale;
                selectLanguageImage[i].color = Color.white;
                languageText.text = languageExplanation[i].ToString();
            }
            else
            {
                selectLanguageImage[i].transform.localScale = originLanguageScale[i];
                Color32 color = new Color32(150, 150, 50, 255);
                selectLanguageImage[i].color = color;
            }
        }
    }
    public void OtherEndGame()
    {
        const int yes = 0;
        const int no = 1;

        endGameNum = ChangeSelectNum(endGameNum, 0, maxEndGameNum, Input.GetKeyDown(KeyCode.S), Input.GetKeyDown(KeyCode.W), se[(int)SE.SELECTSOUNDSET]);

        //選択中の項目を強調表示
        for (int i = 0; i < selectEndGameImage.Length; i++)
        {
            if (i == endGameNum) 
            {
                Transform imageScale = selectEndGameImage[i].transform;
                imageScale.localScale = new Vector3(
                    (originEndGameScale[i].x * zoomEndGameScale),
                    (originEndGameScale[i].y * zoomEndGameScale),
                    (originEndGameScale[i].z * zoomEndGameScale)
                    );
                selectEndGameImage[i].transform.localScale = imageScale.localScale;
                selectEndGameImage[i].color = Color.white;
            }
            else
            {
                selectEndGameImage[i].transform.localScale = originEndGameScale[i];
                Color32 color = new Color32(150, 150, 50, 255);
                selectEndGameImage[i].color = color;
            }
        }

        //実行
        if (Input.GetKeyDown(KeyCode.Space) && !isInputSpace) 
        {
            isInputSpace = true;
            switch (endGameNum)
            {
                case yes:
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
                    break;
                case no:
                    isOpenOtherMenu = false;
                    other.SetActive(true);
                    otherPanel[otherPanelNum].gameObject.SetActive(false);
                    endGameNum = yes;
                    break;
            }
        }
    }

    public int ChangeSelectNum(int selectNum, int minSelectNum, int maxSelectNum, bool plusInput, bool minusInput, AudioClip clip)
    {
        if(plusInput)
        {
            selectNum++;
            if (selectNum >= maxSelectNum)
                selectNum = minSelectNum;

            if(clip != null)
            {
                //SEを再生
                seManager.PlaySE(clip);
            }
        }
        if(minusInput)
        {
            selectNum--;
            if (selectNum < minSelectNum) 
                selectNum = (maxSelectNum - 1);
            if (clip != null)
            {
                //SEを再生
                seManager.PlaySE(clip);
            }
        }

        return selectNum;
    }
}
