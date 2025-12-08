using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("コンポーネント参照")]
    public AudioSource source;

    [Header("スクリプト参照")]
    public GameManager gameManager;


    public virtual void Start()
    {
        source = GetComponent<AudioSource>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }
}
