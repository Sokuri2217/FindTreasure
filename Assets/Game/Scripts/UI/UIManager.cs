using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum  FadeState
{
    START,
    END,
}


public class UIManager : MonoBehaviour
{
    [Header("フェード処理")]
    public bool isSceneMove;
    public int fadeState;
    public GameObject fadePanel;
    public Image fadeImage;
    public float sceneFadeTime;

    [Header("シーン移動")]
    public string sceneName;

    [Header("スクリプト参照")]
    public GameManager gameManager;

    [Header("BGM")]
    public AudioClip bgm;

    [Header("SE")]
    public List<AudioClip> se = new List<AudioClip>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        //スクリプト取得
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        //BGM
        gameManager.PlayBGM(bgm);

        //初回フェード
        //パネル生成
        GameObject createFade = Instantiate(fadePanel, transform);
        fadeImage = createFade.GetComponent<Image>();
        fadeState = (int)FadeState.START;
        //フェード処理
        StartCoroutine(SceneMove());
        isSceneMove = false;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (isSceneMove) return;
    }

    protected IEnumerator SceneMove()
    {
        //シーン移動フラグをtrueにする
        isSceneMove = true;
        float sceneFadeTimer = 0.0f;
        //徐々に色を変えていく
        Color color = fadeImage.color;
        while (sceneFadeTimer < sceneFadeTime)
        {
            //与えられた文字列でフェードを切り替える
            switch (fadeState)
            {
                case (int)FadeState.START:
                    color.a = Mathf.Lerp(1.0f, 0.0f, sceneFadeTimer / sceneFadeTime);
                    break;
                case (int)FadeState.END:
                    color.a = Mathf.Lerp(0.0f, 1.0f, sceneFadeTimer / sceneFadeTime);
                    break;
            }
            fadeImage.color = color;
            sceneFadeTimer += Time.deltaTime;
            yield return null;
        }
        
        fadeImage.color = color;
    }
}
