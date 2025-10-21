using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [Header("�`��n")]
    public Image pressAnyKey;
    public float fadeLimit;
    public float fadeTimer;
    public bool fadeIn;
    public bool fadeOut;

    [Header("�V�[����")]
    public string sceneName;

    [Header("BGM")]
    public AudioClip bgm;

    [Header("�X�N���v�g�Q��")]
    public GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //�X�N���v�g�擾
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        //����ݒ�
        //BGM
        gameManager.PlayBGM(bgm);
        //�t�F�[�h�A�E�g����X�^�[�g
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

        //�ꕔ�摜��_�ł�����
        Color color = pressAnyKey.color;
        color.a = (fadeTimer / fadeLimit);
        pressAnyKey.color = color;

        //��������{�^���������ƁA�V�[�����ړ�
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
