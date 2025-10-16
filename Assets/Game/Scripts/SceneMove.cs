using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMove : MonoBehaviour
{
    [Header("シーン名")]
    public string sceneName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //何かしらボタンを押すと、シーンを移動
        if (Input.anyKeyDown) 
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
