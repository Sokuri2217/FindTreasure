using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMove : MonoBehaviour
{
    [Header("�V�[����")]
    public string sceneName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //��������{�^���������ƁA�V�[�����ړ�
        if (Input.anyKeyDown) 
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
