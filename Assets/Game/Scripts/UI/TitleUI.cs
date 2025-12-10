using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : UIManager
{
    [Header("描画系")]
    public Image pressAnyKey;
    public float fadeLimit;
    public float fadeTimer;
    public bool fadeIn;
    public bool fadeOut;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();

        //初回設定
        //フェードアウトからスタート
        fadeIn = false;
        fadeOut = true;
        fadeTimer = fadeLimit;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

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
            fadeState = (int)FadeState.END;
            StartCoroutine(SceneMove());
        }
    }
}
