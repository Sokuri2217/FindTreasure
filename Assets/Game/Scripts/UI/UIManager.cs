using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("フェード処理")]
    public bool isSceneMove;
    public Image fadePanel;
    public float sceneFadeLimit;
    public float sceneFadeTimer;

    [Header("シーン移動")]
    public string sceneName;

    [Header("BGM")]
    public AudioClip bgm;

    [Header("スクリプト参照")]
    public GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        //スクリプト取得
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //BGM
        gameManager.PlayBGM(bgm);

        //初回フェード
        do
        {
            SceneMove("fadeOut");
        }while (isSceneMove);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (isSceneMove) return;
    }

    public void SceneMove(string fade)
    {
        //シーン移動フラグをtrueにする
        isSceneMove = true;
        //徐々に色を変えていく
        Color color = fadePanel.color;
        color.a = (sceneFadeTimer / sceneFadeLimit);
        //与えられた文字列でフェードを切り替える
        switch (fade)
        {
            case "fadeIn":
                if (color.a < 1.0f)
                {
                    sceneFadeTimer += Time.deltaTime;
                }
                else
                {
                    color.a = 1.0f;
                    SceneManager.LoadScene(sceneName);

                }
                break;
            case "fadeOut":
                if (color.a > 0.0f)
                {
                    sceneFadeTimer -= Time.deltaTime;
                }
                else
                {
                    color.a = 0.0f;
                    isSceneMove = false;
                }
                break;
        }
        fadePanel.color = color;
    }
}
