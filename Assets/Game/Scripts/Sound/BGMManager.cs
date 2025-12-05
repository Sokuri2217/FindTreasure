using UnityEngine;

public class BGMManager : SoundManager
{
    [Header("インスタンス保持")]
    protected static BGMManager Instance { get; private set; }


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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
