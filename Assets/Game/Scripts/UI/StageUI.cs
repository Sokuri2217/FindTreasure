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

    [Header("フェーズ識別番号")]
    public int phaseItem;
    public int phaseMove;
    public int phaseDig; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isPhase[phaseItem] = true;
    }

    // Update is called once per frame
    void Update()
    {
        //フェーズ表示
        ChangePhaseImage();
        //フェーズ変更までの時間計測
        for (int i = 0; i < phaseLimit.Length; i++)
        {
            if (isPhase[i])
            {
                PhaseTimer(i);
            }
        }
    }

    //フェーズごとに色やUIを変える
    public void ChangePhaseImage()
    {
        //フェーズ表示用の枠の色情報を取得
        Color color = phaseFrame.color;

        //現在のフェーズに合わせて枠の色やフェーズ状態を切り替える
        for (int i = phaseItem; i <= phaseDig; i++) 
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
                isPhase[phaseItem] = true;
            }
            else if ((currentPhase + 1) < phaseLimit.Length)
            {
                isPhase[(currentPhase + 1)] = true;
            }
        }
    }
}
