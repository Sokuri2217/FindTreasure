using UnityEngine;
using UnityEngine.UI;

public class StageUI : MonoBehaviour
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

    //[Header("表示パネル")]
    //public GameObject itemData; //アイテムの詳細

    //[Header("アイテム情報")]
    //public Image icon;    //アイテム画像
    //public Text itemName; //名前
    //public Text get;      //獲得時効果
    //public Text hold;     //常在効果
    //public Text active;   //任意効果

    [Header("プレイヤー参照")]
    private GameObject playerObj; 

    [Header("スクリプト参照")]
    private PlayerController player;
    private Inventory inventory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //初回設定
        isPhase[(int)Phase.ITEM] = true;
        //itemData.SetActive(false);
        currentTurn++;
    }

    // Update is called once per frame
    void Update()
    {
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

    ////取得可能なアイテムの詳細を確認
    //public void CheckHitItem()
    //{
    //    if (player.hitItem != null)   
    //    {
    //        ItemBase itemBase = player.hitItem.itemBase;
    //        if (Input.GetKeyDown(KeyCode.F)) 
    //        {
    //            itemData.SetActive(true);
    //        }
    //        //アイテムの情報を取得しUIに反映
    //        icon.sprite = itemBase.icon;
    //        itemName.text = itemBase.itemName;
    //        get.text = itemBase.description[(int)Item.GET];
    //        hold.text = itemBase.description[(int)Item.HOLD];
    //        active.text = itemBase.description[(int)Item.ACTIVE];

    //    }
        
    //}
}

public enum Phase
{
    ITEM,
    DIG,

}
