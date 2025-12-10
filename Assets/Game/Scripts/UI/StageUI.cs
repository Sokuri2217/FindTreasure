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

    [Header("フェーズの計測")]
    public float[] phaseLimit;
    public float timer;

    [Header("表示パネル")]
    public GameObject itemDataPanel;  //アイテムの詳細
    public GameObject inventoryPanel; //所持アイテム一覧
    public GameObject gamePanel;      //ゲーム画面
    public GameObject clearPanel;     //ゲームクリア画面
    public GameObject overPanel;      //ゲームオーバー画面
    public GameObject resultPanel;    //リザルト画面の共通部分
    public GameObject pausePanel;     //一時停止
    public GameObject[] inputPanel;   //入力案内

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
    public GameObject removeImage;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        //初回設定
        isPhase[(int)Phase.ITEM] = true;
        currentTurn++;
        clearTurn = gameManager.clearTurnLimit[gameManager.mapNum];
        allTreasures = gameManager.setTreasure;
        originSlotScale = new Vector3[slotImage.Length];
        for(int i = 0; i < originSlotScale.Length; i++)
            originSlotScale[i] = slotImage[i].transform.localScale;
        //パネル
        itemDataPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        gamePanel.SetActive(true);
        clearPanel.SetActive(false);
        overPanel.SetActive(false);
        resultPanel.SetActive(false);
        inputPanel[(int)Phase.ITEM].SetActive(true);
        inputPanel[(int)Phase.DIG].SetActive(false);
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
            player = playerObj.GetComponent<PlayerController>();
        if (inventory == null)
            inventory = playerObj.GetComponent<Inventory>();

        //シーン移動
        OpenPauseMenu();

        //プレイ結果
        if(gameClear)
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
        for (int i = 0; i < phaseLimit.Length; i++)
        {
            if (isPhase[i])
            {
                //フェーズ変更までの時間計測
                PhaseTimer(i);
            }
        }
        //現在のフェーズをスキップ
        SkipPhase();

        //アイテムの詳細表示
        CheckHitItem();
        //所持アイテムを確認
        CheckInventory();
        //結果判定
        GameResult();
    }

    //フェーズごとに色やUIを変える
    public void ChangePhaseImage()
    {
        //フェーズ表示用の枠の色情報を取得
        Color color = phaseFrame.color;

        //現在のフェーズに合わせて枠の色やフェーズ状態を切り替える
        for (int i = (int)Phase.ITEM; i <= (int)Phase.DIG; i++) 
        {
            if (isPhase[i]) 
            {
                currentPhase.sprite = phaseImage[i];
                color = phaseColor[i];
            }
        }
        //変化後の色を反映
        phaseFrame.color = color;
    }

    //フェーズ変更までの時間計測
    public void PhaseTimer(int currentPhase)
    {
        if (inventoryPanel.activeSelf && isPhase[(int)Phase.DIG]) return;
        timer += Time.deltaTime;
        if (timer >= phaseLimit[currentPhase])
        {
            timer = 0.0f;
            isPhase[currentPhase] = false;
            if ((currentPhase + 1) >= phaseLimit.Length)
            {
                isPhase[(int)Phase.ITEM] = true;
            }
            else if ((currentPhase + 1) < phaseLimit.Length)
            {
                isPhase[(currentPhase + 1)] = true;
            }
        }
    }

    public void EndItem()
    {
        timer = 0.0f;
        isPhase[(int)Phase.ITEM] = false;
        isPhase[(int)Phase.DIG] = true;
        inventoryPanel.SetActive(false);
        itemDataPanel.SetActive(false);
        inputPanel[(int)Phase.ITEM].SetActive(false);
        inputPanel[(int)Phase.DIG].SetActive(true);
    }

    public void EndDig()
    {
        timer = 0.0f;
        isPhase[(int)Phase.DIG] = false;
        isPhase[(int)Phase.ITEM] = true;
        inventoryPanel.SetActive(false);
        itemDataPanel.SetActive(false);
        inputPanel[(int)Phase.ITEM].SetActive(true);
        inputPanel[(int)Phase.DIG].SetActive(false);
        //ターンを経過させる
        currentTurn++;
        //ターンをUIに反映
        turnText.text = currentTurn.ToString();
        //採掘数をリセット
        player.digCurrent = player.digLimit;
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
            ItemBase itemBase = player.hitItem.itemBase;
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!isFocusItem) 
                {
                    isFocusItem = true;
                }
                else
                {
                    isFocusItem = false;
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
        if (isPhase[(int)Phase.DIG])
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
        }
        else if (isPhase[(int)Phase.ITEM])
        {
            inventoryPanel.SetActive(true);
        }

        //確認するアイテムを選択
        if (inventoryPanel.activeSelf)
        {
            //詳細パネルを表示
            itemDataPanel.SetActive(true);

            //インベントリの行数を選択（縦・横）
            if (Input.GetKeyDown(KeyCode.W))
                inventory.lineHeight--;
            else if (Input.GetKeyDown(KeyCode.S))
                inventory.lineHeight++;
            else if (Input.GetKeyDown(KeyCode.A))
                inventory.lineWidth--;
            else if (Input.GetKeyDown(KeyCode.D))
                inventory.lineWidth++;

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
                if (inventory.items[i] != null)
                {
                    //各スロットにアイコンを表示
                    slotImage[i].sprite = inventory.items[i].icon;
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

                    if (inventory.items[i] != null)
                    {
                        //アイテムの情報を取得しUIに反映
                        icon.sprite = inventory.items[i].icon;
                        itemName.text = inventory.items[i].itemName;
                        get.text = inventory.items[i].description[(int)Item.GET];
                        hold.text = inventory.items[i].description[(int)Item.HOLD];
                        active.text = inventory.items[i].description[(int)Item.ACTIVE];

                        //削除アイコンを表示
                        removeImage.SetActive(true);
                    }
                    else
                    {
                        //選択スロットにアイテムが格納されていないとき
                        icon.sprite = nullSlot;
                        itemName.text = "---";
                        get.text = "---";
                        hold.text = "---";
                        active.text = "---";

                        //削除アイコンを非表示
                        removeImage.SetActive(false);
                    }


                    if (Input.GetKeyDown(KeyCode.R) && inventory.items[i] != null) 
                    {
                        inventory.RemoveItem(inventory.items[i]);
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
                    for (int i = 0; i < pauseMenuPanel.Length; i++)
                    {
                        pauseMenuPanel[i].SetActive(false);
                    }
                    isPause = false;
                    break;
                case false:
                    isPause = true;
                    break;
            }
            pausePanel.SetActive(isPause);

            if(pausePanel.activeSelf)
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
                    if (isSelected >= maxSelectNum)
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

                if(Input.GetKeyDown(KeyCode.Space))
                {
                    switch(isSelected)
                    {
                        case (int)Pause.CONTROL:
                            break;
                        case (int)Pause.GIMMICK:
                            break;
                        case (int)Pause.EXIT:
                            ResultSceneMove(menu);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    //結果の判定
    public void GameResult()
    {
        if (currentTurn > clearTurn)
        {
            gameOver = true;
        }
        else if (player.isGetTreasure >= allTreasures) 
        {
            gameClear = true;
        }
    }

    public void GameClear()
    {
        gameClear = true;
        gamePanel.SetActive(false);
        clearPanel.SetActive(gameClear);
        resultPanel.SetActive(true);
        SelectResultMenu();
    }

    public void GameOver()
    {
        gameOver = true;
        gamePanel.SetActive(false);
        overPanel.SetActive(gameOver);
        resultPanel.SetActive(true);
        SelectResultMenu();
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
