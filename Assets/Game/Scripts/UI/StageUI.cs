using UnityEngine;
using UnityEngine.UI;

public class StageUI : MonoBehaviour
{

    [Header("�t�F�[�Y�֌W")]
    public bool[] isPhase;
    public Color[] phaseColor;
    public Sprite[] phaseImage;
    public Image phaseFrame;
    public Image currentPhase;

    //�t�F�[�Y�ԍ�
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


        //�t�F�[�Y�\��
        ChangePhaseImage();
    }

    //�t�F�[�Y���ƂɐF��UI��ς���
    public void ChangePhaseImage()
    {
        //�t�F�[�Y�\���p�̘g�̐F�����擾
        Color color = phaseFrame.color;

        //���݂̃t�F�[�Y�ɍ��킹�Ęg�̐F��t�F�[�Y��Ԃ�؂�ւ���
        for (int i = (int)Phase.MOVE; i <= (int)Phase.DIG; i++) 
        {
            if (isPhase[i]) 
            {
                currentPhase.sprite = phaseImage[i];
                color = phaseColor[i];
            }
        }
        //�ω���̐F�𔽉f
        phaseFrame.color = color;
    }
}
