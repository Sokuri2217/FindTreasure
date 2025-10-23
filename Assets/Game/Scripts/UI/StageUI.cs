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

    //フェーズ番号
    enum Phase
    {
        MOVE,
        ITEM,
        DIG
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isPhase[(int)Phase.MOVE] = true;
    }

    // Update is called once per frame
    void Update()
    {


        //フェーズ表示
        ChangePhaseImage();
    }

    //フェーズごとに色やUIを変える
    public void ChangePhaseImage()
    {
        //フェーズ表示用の枠の色情報を取得
        Color color = phaseFrame.color;

        //現在のフェーズに合わせて枠の色やフェーズ状態を切り替える
        for (int i = (int)Phase.MOVE; i <= (int)Phase.DIG; i++) 
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
