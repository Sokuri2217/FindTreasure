using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [Header("アイテム情報")]
    public ItemBase itemBase;

    [Header("オブジェクト回転")]
    public float rotationSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
}
