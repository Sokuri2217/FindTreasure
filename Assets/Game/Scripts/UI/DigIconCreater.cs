using UnityEngine;
using System.Collections.Generic;

public class DigIconCreater : MonoBehaviour
{
    [Header("描画画像")]
    public List<GameObject> digCount=new List<GameObject>();
    public GameObject digCountIcon;
    public float imgWidth;
    public float imgHeight;
    public int beforeDigCount;

    [Header("スクリプト参照")]
    public PlayerController player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        for (int i = 0; i < player.digCurrent; i++) 
        {
            Vector3 createPos = new Vector3(transform.position.x + (i * imgWidth), 0.0f, 0.0f);
            GameObject iconImage = Instantiate(digCountIcon, createPos, Quaternion.identity);
            digCount.Add(iconImage);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (beforeDigCount != player.digCurrent) 
        {
            AddDigCount(player.digCurrent);
        }
    }

    public void AddDigCount(int count)
    {
        int offset = (beforeDigCount - player.digCurrent);
        if(offset < 0)
        {
            digCount.Remove(digCountIcon);
        }
        Vector3 createPos = new Vector3(transform.position.x + (count * imgWidth), 0.0f, 0.0f);
        GameObject iconImage = Instantiate(digCountIcon, createPos, Quaternion.identity);
    }
}
