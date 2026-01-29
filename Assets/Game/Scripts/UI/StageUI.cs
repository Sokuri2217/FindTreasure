using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageUI : UIManager
{
    [Header("フェーズ関係")]
    public bool[] isPhase;
    public Color[] phaseColor;
    public Sprite[] phaseImage;
    public Image phaseFrame;
    public Image currentPhase;
    public int currentTurn;
    public Text turnText;
    public Text useItemCountText;

    [Header("フェーズの計測")]
    public float[] phaseLimit;
    public float timer;
    public Image timerImage;

    [Header("表示パネル")]
    public GameObject itemDataPanel;  //アイテムの詳細
    public GameObject inventoryPanel; //所持アイテム一覧
    public GameObject gamePanel;      //ゲーム画面
    public GameObject clearPanel;     //ゲームクリア画面
    public GameObject overPanel;      //ゲームオーバー画面
    public GameObject resultPanel;    //リザルト画面の共通部分
    public GameObject pausePanel;     //一時停止
    public GameObject notPausePanel;  //一時停止

    [Header("アイテム情報")]
    public Image icon;      //アイテム画像
    public Text itemName;   //名前
    public Text get;        //獲得時効果
    public Text hold;       //常在効果
    public Text active;     //任意効果
    public Sprite nullSlot; //空きスロット画像

    [Header("インベントリ情報")]
    public Image[] slotImage;         //アイテムスロット
    public Vector3[] originSlotScale; //通常サイズ
    public float zoomNum;             //拡大率
    public GameObject useActiveImage;

    [Header("アイテム効果")]
    public bool isTimeStop;
    public float stopTimer;
    public float stopLimit;

    [Header("フラグ")]
    public bool isPause;
    public bool gameClear;
    public bool gameOver;
    public bool isFocusItem;

    [Header("ステージ関係")]
    public int clearTurn;     //クリア条件
    public int allTreasures;  //クリアに必要なタカラモノの数

    [Header("ポーズ画面")]
    public bool[] isSelectOption;
    public int isSelected;
    public int maxSelectNum;
    public GameObject[] pauseMenuPanel;
    public Transform[] pauseMenu;
    public Image[] pauseMenuImage;
    public Vector3[] originPauseScale;//通常サイズ
    public float zoomPauseNum;        //拡大率
    public Text gimmickText;
    public GameObject pauseMenuObj;
    public GameObject openPauseMenu;
    public GameObject closePauseMenu;

    [Header("リザルト画面")]
    public int setMoveScene;
    public Image[] setMode;
    public Image selectImage;

    [Header("シーン移動")]
    public string menu;
    public string currentScene;

    [Header("プレイヤー参照")]
    private GameObject playerObj; 

    [Header("スクリプト参照")]
    private PlayerController player;
    private Inventory inventory;

    [Header("効果音")]
    public AudioClip useItem;

    enum SE
    {
        OPENFOCUS,
        CLOSEFOCUS,
        SELECTINVENTORY,
        CHANGEPHASE,
        GAMECLEAR,
        GAMEOVER,
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        //初回設定
        isPhase[(int)Phase.ITEM] = true;
        clearTurn = gameManager.clearTurnLimit[gameManager.mapNum];
        allTreasures = gameManager.setTreasure;
        originSlotScale = new Vector3[slotImage.Length];
        for(int i = 0; i < originSlotScale.Length; i++)
            originSlotScale[i] = slotImage[i].transform.localScale;
        for (int i = 0; i < pauseMenuImage.Length; i++)
            originPauseScale[i] = pauseMenuImage[i].transform.localScale;
        //パネル
        itemDataPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        gamePanel.SetActive(true);
        clearPanel.SetActive(false);
        overPanel.SetActive(false);
        resultPanel.SetActive(false);
        pausePanel.SetActive(false);
        for (int i = 0; i < pauseMenuPanel.Length; i++)
        {
            pauseMenuPanel[i].SetActive(false);
        }

        //ターンをUIに反映
        turnText.text = currentTurn.ToString();

        //現在のシーン名を保存
        currentScene = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        //プレイヤー取得
        if (playerObj == null)
            playerObj = GameObject.FindWithTag("Player");
        //スクリプト取得
        if (player == null)
        {
            player = playerObj.GetComponent<PlayerController>();
            player.digCurrent = player.digLimit;
            player.useItem = player.useItemLimit;
        }
            
        if (inventory == null)
            inventory = playerObj.GetComponent<Inventory>();

        //一時停止
        OpenPauseMenu();
        //結果判定
        GameResult();

        //プレイ結果
        if (gameClear)
        {
            GameClear();
            return;
        }
        if(gameOver)
        {
            GameOver();
            return;
        }

        if (isPause) return;

        //フェーズ表示
        ChangePhaseImage();
        //採掘フェーズの強制終了
        if (player.digCurrent <= 0)
            EndDig();
        if (player.useItem <= 0)
            EndItem();
        for (int i = 0; i < phaseLimit.Length; i++)
        {
            if (isPhase[i])
            {
                //フェーズ変更までの時間計測
                PhaseTimer(i);
            }
        }
        //ターンの制御
        if (currentTurn <= 1)
        {
            //ターン数が1を下回らないようにする
            currentTurn = 1;
        }

        //現在のフェーズをスキップ
        SkipPhase();

        //アイテムの詳細表示
        CheckHitItem();
        //所持アイテムを確認
        CheckInventory();
        //使用可能ホリダシモノ数
        UseItemCount();
    }

    //フェーズごとに色やUIを変える
    public void ChangePhaseImage()
    {
        //フェーズ表示用の枠の色情報を取得
        Color color = phaseFrame.color;

        //現在のフェーズに合わせて枠の色やフェーズ状態を切り替える
        for (int i = 0; i < isPhase.Length; i++)  
        {
            if (isPhase[i]) 
            {
                currentPhase.sprite = phaseImage[i];
                color = phaseColor[i];
                phaseFrame.color = color;
                Color timerColor = timerImage.color;
                timerColor = phaseColor[i];
                timerImage.color = color;
                timerImage.fillAmount = 1.0f - Mathf.Clamp01(timer / phaseLimit[i]);
            }
        }
    }

    //フェーズ変更までの時間計測
    public void PhaseTimer(int currentPhase)
    {
        //インベントリが開かれている場合時間経過を止める
        if (inventoryPanel.activeSelf) return;
        //アイテムの効果
        if (isTimeStop && stopTimer >= 0.01f) 
        {
            stopTimer -= Time.deltaTime;
        }
        else
        {
            isTimeStop = false;
            stopTimer = stopLimit;
            timer += Time.deltaTime;
            if (timer > phaseLimit[currentPhase])
            {
                timer = 0.0f;
                if (isPhase[(int)Phase.ITEM])
                    EndItem();
                else if (isPhase[(int)Phase.DIG])
                    EndDig();
            }
        }
            
    }

    public void UseItemCount()
    {
        useItemCountText.text = player.useItem.ToString();
    }

    public void EndItem()
    {
        timer = 0.0f;
        isPhase[(int)Phase.ITEM] = false;
        isPhase[(int)Phase.DIG] = true;
        inventoryPanel.SetActive(false);
        itemDataPanel.SetActive(false);
        player.digCurrent = player.digLimit;
        player.useItem = player.useItemLimit;
        seManager.PlaySE(se[(int)SE.CHANGEPHASE]);
    }

    public void EndDig()
    {
        timer = 0.0f;
        isPhase[(int)Phase.DIG] = false;
        isPhase[(int)Phase.ITEM] = true;
        inventoryPanel.SetActive(false);
        itemDataPanel.SetActive(false);
        currentTurn++;
        turnText.text = currentTurn.ToString();
        player.digCurrent = player.digLimit;
        seManager.PlaySE(se[(int)SE.CHANGEPHASE]);
    }

    public void SkipPhase()
    {
        if (Input.GetKeyDown(KeyCode.H))  
        {
            if (isPhase[(int)Phase.ITEM])
            {
                EndItem();
            }
            else if (isPhase[(int)Phase.DIG])
            {
                EndDig();
            }
        }
    }

    //取得可能なアイテムの詳細を確認
    public void CheckHitItem()
    {
        if (inventoryPanel.activeSelf) return;

        if (player.hitItem != null && !player.isMoving)
        {
            ItemBase itemBase = player.hitItem;
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!isFocusItem) 
                {
                    isFocusItem = true;
                    //SEを再生
                    seManager.PlaySE(se[(int)SE.OPENFOCUS]);
                }
                else
                {
                    isFocusItem = false;
                    //SEを再生
                    seManager.PlaySE(se[(int)SE.CLOSEFOCUS]);
                }
                itemDataPanel.SetActive(isFocusItem);
            }
            //アイテムの情報を取得しUIに反映
            icon.sprite = itemBase.icon;
            itemName.text = itemBase.itemName;
            get.text = itemBase.description[(int)Item.GET];
            hold.text = itemBase.description[(int)Item.HOLD];
            active.text = itemBase.description[(int)Item.ACTIVE];
        }
        else
        {
            //詳細パネルを非表示
            itemDataPanel.SetActive(false);
            //アイテム情報を空にする
            icon.sprite = null;
            itemName.text = null;
            get.text = null;
            hold.text = null;
            active.text = null;
        }
    }

    //所持アイテムを確認
    public void CheckInventory()
    {
        //インベントリを表示
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isFocusItem = false;
            if (!inventoryPanel.activeSelf)
            {
                inventoryPanel.SetActive(true);
                itemDataPanel.SetActive(true);
            }
            else
            {
                inventoryPanel.SetActive(false);
                itemDataPanel.SetActive(false);
            }

        }

        //確認するアイテムを選択
        if (inventoryPanel.activeSelf)
        {
            //詳細パネルを表示
            itemDataPanel.SetActive(true);

            //インベントリの行数を選択（縦・横）
            if (Input.GetKeyDown(KeyCode.W))
            {
                inventory.lineHeight--;
                seManager.PlaySE(se[(int)SE.SELECTINVENTORY]);
            }
                
            else if (Input.GetKeyDown(KeyCode.S))
            {
                inventory.lineHeight++;
                seManager.PlaySE(se[(int)SE.SELECTINVENTORY]);
            }
                
            else if (Input.GetKeyDown(KeyCode.A))
            {
                inventory.lineWidth--;
                seManager.PlaySE(se[(int)SE.SELECTINVENTORY]);
            }
                
            else if (Input.GetKeyDown(KeyCode.D))
            {
                inventory.lineWidth++;
                seManager.PlaySE(se[(int)SE.SELECTINVENTORY]);
            }

            //超過しないよう制御
            if (inventory.lineWidth >= inventory.lineMaxWidth)
            {
                inventory.lineWidth = 0;
                inventory.lineHeight++;
            }
            if (inventory.lineWidth < 0)
            {
                inventory.lineWidth = (inventory.lineMaxWidth - 1);
                inventory.lineHeight--;
            }
            if (inventory.lineHeight >= inventory.lineMaxHeight)
                inventory.lineHeight = 0;
            if (inventory.lineHeight < 0)
                inventory.lineHeight = (inventory.lineMaxHeight - 1);

            //行数に基づいて選択中のアイテムを設定
            // 横 ＋ ( 縦 × 横の最大値 ) = インベントリの番号
            inventory.isSelectItem = (inventory.lineWidth + (inventory.lineHeight * inventory.lineMaxWidth));
            
            //選択中のアイテムアイコンを拡大表示
            for (int i = 0; i < slotImage.Length; i++)
            {
                if (inventory.items[i] != null && inventory.items[i].itemBase != null) 
                {
                    //各スロットにアイコンを表示
                    slotImage[i].sprite = inventory.items[i].itemBase.icon;
                }
                else
                {
                    //各スロットに空白アイコンを表示
                    slotImage[i].sprite = nullSlot;
                }
                //現在選択中のアイテムに関する情報を表示
                if ((inventory.isSelectItem == i))
                {
                    //拡大させる
                    Transform imageScale = slotImage[i].transform;
                    imageScale.localScale = new Vector3(
                        (originSlotScale[i].x * zoomNum),
                        (originSlotScale[i].y * zoomNum),
                        (originSlotScale[i].z * zoomNum)
                        );
                    slotImage[i].transform.localScale = imageScale.localScale;

                    if (inventory.items[i] != null && inventory.items[i].itemBase != null) 
                    {
                        //アイテムの情報を取得しUIに反映
                        icon.sprite = inventory.items[i].itemBase.icon;
                        itemName.text = inventory.items[i].itemBase.itemName;
                        get.text = inventory.items[i].itemBase.description[(int)Item.GET];
                        hold.text = inventory.items[i].itemBase.description[(int)Item.HOLD];
                        active.text = inventory.items[i].itemBase.description[(int)Item.ACTIVE];
                    }
                    else
                    {
                        //選択スロットにアイテムが格納されていないとき
                        icon.sprite = nullSlot;
                        itemName.text = null;
                        get.text = null;
                        hold.text = null;
                        active.text = null;
                    }

                    bool canUse = (!isPhase[(int)Phase.DIG] &&
                        inventory.items[i] != null &&
                        inventory.items[i].itemBase != null &&
                        !inventory.items[i].isUseActive &&
                        !inventory.items[i].isCoolDown &&
                        (inventory.items[i].itemBase.originDuration != 0 ||
                        inventory.items[i].itemBase.originCoolTime != 0));

                    if (useActiveImage != null) 
                    {
                        useActiveImage.SetActive(canUse);
                        if (Input.GetKeyDown(KeyCode.Space) && canUse)    
                        {
                            inventory.items[i].itemBase.OnUse(player, this);
                            inventory.items[i].isUseActive = true;
                            inventory.items[i].useActiveTurn = currentTurn;
                            inventory.isActiveItems.Add(inventory.items[i]);
                            seManager.PlaySE(useItem);
                            player.useItem--;
                        }
                    }
                }
                //非選択中のアイテムに関する処理
                else
                {
                    //拡大率を普通に戻す
                    slotImage[i].transform.localScale = originSlotScale[i];

                }
            }
        }
    }

    //ポーズ画面
    public void OpenPauseMenu()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            switch (isPause)
            {
                case true:
                    notPausePanel.SetActive(true);
                    for (int i = 0; i < pauseMenuPanel.Length; i++)
                    {
                        pauseMenuPanel[i].SetActive(false);
                    }
                    isPause = false;
                    pausePanel.SetActive(false);
                    pauseMenuObj.SetActive(false);
                    closePauseMenu.SetActive(false);
                    break;
                case false:
                    notPausePanel.SetActive(false);
                    isPause = true;
                    pausePanel.SetActive(true);
                    pauseMenuObj.SetActive(true);
                    break;
            }
        }
        if (isPause && pausePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                isSelected--;
                if (isSelected < 0)
                {
                    isSelected = (isSelectOption.Length - 1);
                }
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                isSelected++;
                if (isSelected >= isSelectOption.Length)
                {
                    isSelected = 0;
                }
            }

            for (int i = 0; i < isSelectOption.Length; i++)
            {
                isSelectOption[i] = false;
                if (i == isSelected)
                {
                    isSelectOption[i] = true;
                }
            }

            //選択中の項目を強調表示
            for (int i = 0; i < isSelectOption.Length; i++)
            {
                if (isSelectOption[i])
                {
                    Transform imageScale = pauseMenuImage[i].transform;
                    imageScale.localScale = new Vector3(
                        (originPauseScale[i].x * zoomPauseNum),
                        (originPauseScale[i].y * zoomPauseNum),
                        (originPauseScale[i].z * zoomPauseNum)
                        );
                    pauseMenuImage[i].transform.localScale = imageScale.localScale;
                    pauseMenuImage[i].color = Color.white;
                }
                else
                {
                    pauseMenuImage[i].transform.localScale = originPauseScale[i];
                    Color32 color = new Color32(150, 150, 50, 255);
                    pauseMenuImage[i].color = color;
                }
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                switch (isSelected)
                {
                    case (int)Pause.CONTROL:
                        OpenControlPanel();
                        break;
                    case (int)Pause.GIMMICK:
                        OpenGimmickPanel();
                        break;
                    case (int)Pause.EXIT:
                        ResultSceneMove(menu);
                        break;
                    default:
                        break;
                }
            }
        }
        else if (isPause && !pausePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                switch (isSelected)
                {
                    case (int)Pause.CONTROL:
                        OpenControlPanel();
                        break;
                    case (int)Pause.GIMMICK:
                        OpenGimmickPanel();
                        break;
                    case (int)Pause.EXIT:
                        break;
                    default:
                        break;
                }
            }
        }
    }

    //結果の判定
    public void GameResult()
    {
        if (currentTurn > clearTurn && !gameOver)
        {
            gameOver = true;
            seManager.PlaySE(se[(int)SE.GAMEOVER]);
        }
        else if (player.isGetTreasure >= allTreasures && !gameClear)  
        {
            gameClear = true;
            seManager.PlaySE(se[(int)SE.GAMECLEAR]);
        }

        if (gameClear || gameOver)
            SelectResultMenu();
    }

    public void GameClear()
    {
        bgmManager.StopBGM();
        gamePanel.SetActive(false);
        clearPanel.SetActive(gameClear);
        resultPanel.SetActive(true);
    }

    public void GameOver()
    {
        bgmManager.StopBGM();
        gamePanel.SetActive(false);
        overPanel.SetActive(gameOver);
        resultPanel.SetActive(true);
    }

    public void SelectResultMenu()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            setMoveScene--;
            if (setMoveScene < 0) 
            {
                setMoveScene = (setMode.Length - 1);
            }
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            setMoveScene++;
            if (setMoveScene >= setMode.Length) 
            {
                setMoveScene = 0;
            }
        }

        for (int i = 0; i < setMode.Length; i++) 
        {
            if (i == setMoveScene)
            {
                Transform selectPos = selectImage.transform;
                selectPos.position = new Vector3(
                    selectPos.position.x, 
                    setMode[i].transform.position.y, 
                    selectPos.position.z
                    );
                selectImage.transform.position = selectPos.position;
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            switch(setMoveScene)
            {
                case (int)Result.RETRY:
                    ResultSceneMove(currentScene);
                    break;
                case (int)Result.BACKMENU:
                    ResultSceneMove(menu);
                    break;
            }
        }
    }

    public void OpenControlPanel()
    {
        if (!pauseMenuPanel[(int)Pause.CONTROL].activeSelf)
        {
            pauseMenuPanel[(int)Pause.CONTROL].SetActive(true);
            pausePanel.SetActive(false);
            closePauseMenu.SetActive(true);
            openPauseMenu.SetActive(false);
        }
        else
        {
            pauseMenuPanel[(int)Pause.CONTROL].SetActive(false);
            pausePanel.SetActive(true);
            closePauseMenu.SetActive(false);
            openPauseMenu.SetActive(true);
        }
    }

    public void OpenGimmickPanel()
    {
        if (!pauseMenuPanel[(int)Pause.GIMMICK].activeSelf)
        {
            pauseMenuPanel[(int)Pause.GIMMICK].SetActive(true);
            pausePanel.SetActive(false);
            closePauseMenu.SetActive(true);
            openPauseMenu.SetActive(false);
        }
        else
        {
            pauseMenuPanel[(int)Pause.GIMMICK].SetActive(false);
            pausePanel.SetActive(true);
            closePauseMenu.SetActive(false);
            openPauseMenu.SetActive(true);
        }

        gimmickText.text = gameManager.gimmickDescription[gameManager.mapNum].ToString();
    }

    public void ResultSceneMove(string moveSceneName)
    {
        fadeState = (int)FadeState.END;
        sceneName = moveSceneName;
        StartCoroutine(SceneMove());
    }
}

public enum Phase
{
    ITEM,
    DIG,
}

public enum Pause
{
    CONTROL,
    GIMMICK,
    EXIT,
}

public enum Result
{
    RETRY,
    BACKMENU,
}

public enum Get
{
    TREASURE,
    ITEM
}