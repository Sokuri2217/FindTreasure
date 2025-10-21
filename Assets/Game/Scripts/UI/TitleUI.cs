using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [Header("描画系")]
    public Image pressAnyKey;
    public float fadeLimit;
    public float fadeTimer;
    public bool fadeIn;
    public bool fadeOut;

    [Header("シーン名")]
    public string sceneName;

    [Header("BGM")]
    public AudioClip bgm;

    [Header("スクリプト参照")]
    public GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //スクリプト取得
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        //初回設定
        //BGM
        gameManager.PlayBGM(bgm);
        //フェードアウトからスタート
        fadeIn = false;
        fadeOut = true;
        fadeTimer = fadeLimit;
    }

    // Update is called once per frame
    void Update()
    {
        if(fadeIn)
        {
            fadeTimer += Time.deltaTime;
            if (fadeTimer >= fadeLimit) 
            {
                fadeIn = false;
                fadeOut = true;
            }
        }
        else if(fadeOut)
        {
            fadeTimer -= Time.deltaTime;
            if (fadeTimer <= 0.0f) 
            {
                fadeOut = false;
                fadeIn = true;
            }
        }

        //一部画像を点滅させる
        Color color = pressAnyKey.color;
        color.a = (fadeTimer / fadeLimit);
        pressAnyKey.color = color;

        //何かしらボタンを押すと、シーンを移動
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
