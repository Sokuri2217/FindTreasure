using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("インスタンス保持")]
    protected static GameManager Instance { get; private set; }

    [Header("シーン移動関連")]
    public bool inputCheck;    //入力許可
    public Image fadeImage;    //暗転・明転用オブジェクト
    public float fadeDuration; //暗転・明転にかける時間

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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

    // Update is called once per frame
    void Update()
    {
        //入力許可があるときのみ処理を行う
        if (!inputCheck) return;


    }
}
