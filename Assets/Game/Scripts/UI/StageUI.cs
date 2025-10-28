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


        //デバッグ用
        if (Input.GetKeyDown(KeyCode.F))
        {
            for (int i = phaseItem; i <= phaseDig; i++) 
            {
                if(isPhase[i])
                {
                    isPhase[i] = false;
                    if ((i + 1) > phaseDig)
                    {
                        isPhase[phaseItem] = true;
                    }
                    else if ((i + 1) <= phaseDig)
                    {
                        isPhase[i + 1] = true;
                    }

                    break;
                }
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
}
