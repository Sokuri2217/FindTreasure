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

    [Header("フェーズの計測")]
    public float[] phaseLimit;
    public float timer;

    [Header("表示パネル")]
    public GameObject itemDataPanel;  //アイテムの詳細
    public GameObject inventoryPanel; //所持アイテム一覧
    public Image[] slotImage;         //アイテムスロット
    public Vector3[] originSlotScale;  //通常サイズ
    public float zoomNum;        //拡大率

    [Header("アイテム情報")]
    public Image icon;    //アイテム画像
    public Text itemName; //名前
    public Text get;      //獲得時効果
    public Text hold;     //常在効果
    public Text active;   //任意効果

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
        itemDataPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        currentTurn++;
        originSlotScale = new Vector3[slotImage.Length];
        for(int i = 0; i < originSlotScale.Length; i++)
            originSlotScale[i] = slotImage[i].transform.localScale;
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
        //シーン移動
        BackMenu();
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
    }

    public void EndDig()
    {
        timer = 0.0f;
        isPhase[(int)Phase.DIG] = false;
        isPhase[(int)Phase.ITEM] = true;
        currentTurn++;
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
        if (player.hitItem != null && !player.isMoving) 
        {
            ItemBase itemBase = player.hitItem.itemBase;
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!itemDataPanel.activeSelf)
                {
                    itemDataPanel.SetActive(true);
                }
                else
                {
                    itemDataPanel.SetActive(false);
                }

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
                if (!inventoryPanel.activeSelf)
                    inventoryPanel.SetActive(true);
                else
                    inventoryPanel.SetActive(false);
            }
        }
        else if (isPhase[(int)Phase.ITEM]) 
        {
            inventoryPanel.SetActive(true);
        }

        //確認するアイテムを選択
        if (inventoryPanel.activeSelf)
        {
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
            if (inventory.lineHeight >= inventory.lineMaxHeight)
                inventory.lineHeight = 0;
            if (inventory.lineHeight < 0)
                inventory.lineHeight = inventory.lineMaxHeight;
            if (inventory.lineWidth >= inventory.lineMaxWidth)
                inventory.lineWidth = 0;
            if (inventory.lineWidth < 0)
                inventory.lineWidth = inventory.lineMaxWidth;

            //行数に基づいて選択中のアイテムを設定
            // 横 ＋ ( 縦 × 横の最大値 ) = インベントリの番号
            inventory.isSelectItem = (inventory.lineWidth + (inventory.lineHeight * inventory.lineMaxWidth));

            for (int i = 0; i < slotImage.Length; i++)
            {
                if (inventory.isSelectItem == i)
                {
                    Transform imageScale = slotImage[i].transform;
                    imageScale.localScale = new Vector3(
                        (originSlotScale[i].x * zoomNum),
                        (originSlotScale[i].y * zoomNum),
                        (originSlotScale[i].z * zoomNum)
                        );
                    slotImage[i].transform.localScale = imageScale.localScale;
                }
                else
                {
                    slotImage[i].transform.localScale = originSlotScale[i];
                }
            }

        }
    }

    //メニューに戻る
    public void BackMenu()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            fadeState = (int)FadeState.END;
            StartCoroutine(SceneMove());
            SceneManager.LoadScene(sceneName);
        }
    }
}

public enum Phase
{
    ITEM,
    DIG,

}
