using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("インスタンス保持")]
    protected static GameManager Instance { get; private set; }

    [Header("ステージ設定")]
    public int setMaxStage;      //マップの最大サイズ
    public int setMinStage;      //マップの最小サイズ
    public int setStage;         //マップの現在サイズ
    public int setTreasure;      //タカラモノの数
    public int setItem;          //ホリダシモノの数
    public int basicSetTreasure; //タカラモノの基準値
    public int basicSetItem;     //ホリダシモノの基準値
    public int[] clearTurnLimit; //クリア条件(何ターン以内)

    [Header("ステージ設定")]
    public int mapNum;     //マップ番号
    public int mapMaxNum;  //マップ番号の最大値

    [Header("アイテム情報")]
    public List<ItemBase> items = new List<ItemBase>();

    [Header("固有アイテム")]
    public List<ItemBase> uniqueItems = new List<ItemBase>();

    [Header("サウンド")]
    public AudioSource bgm;
    public AudioSource se;

    void Awake()
    {
        //シングルトン
        {
            // すでにインスタンスが存在する場合は削除
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでオブジェクトを保持
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bgm = GameObject.Find("BGMManager").GetComponent<AudioSource>();
        se = GameObject.Find("SEManager").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //ステージ設定
        {
            //マップ上におけるタカラモノとホリダシモノの上限設定
            setTreasure = CreateStageLimit(setTreasure, basicSetTreasure);
            setItem = CreateStageLimit(setItem, basicSetItem);
        }
    }

    //マップ生成における上限設定
    public int CreateStageLimit(int set, int basicSet)
    {
        //マップサイズに応じた上限を超えないようにする
        //タカラモノ
        if ((basicSet + setStage) <= set) 
            set = (basicSet + setStage);

        return set;
    }
}
