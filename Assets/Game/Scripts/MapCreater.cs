using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCreater : MonoBehaviour
{
    public int mapSize;              //�}�b�v�̃T�C�Y(mapSize * mapSize)
    public float tileSize;           // 1�}�X�̑傫��
    public GameObject tilePrefab;    // �^�C���I�u�W�F�N�g
    public GameObject playerPrefab;  // �v���C���[�I�u�W�F�N�g
    public GameObject treasurePrefab;//�^�J�����m�I�u�W�F�N�g
    public GameObject itemPrefab;    //�z���_�V���m�I�u�W�F�N�g

    //�}�b�v��ɐ������鐔
    public int treasure; //�^�J�����m
    public int item;     //�z���_�V���m

    private List<TileManager> allTiles = new List<TileManager>();

    void Start()
    {
        GenerateGrid();          // �O���b�h�}�b�v�𐶐�
        PlaceItemsRandomly();    // �^�C���ɃA�C�e���������_���ɖ��ߍ���
        PlacePlayerOnStart();    // �v���C���[���X�^�[�g�ʒu�ɔz�u
    }

    void GenerateGrid()
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int z = 0; z < mapSize; z++)
            {
                // �^�C����z�u������W���v�Z�iX-Z���ʏ�j
                Vector3 spawnPos = new Vector3(x * tileSize, 0, z * tileSize);

                // �^�C���v���n�u�𐶐����A�e�� GridManager �ɐݒ�
                GameObject tileObj = Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform);
                tileObj.name = $"Tile {x},{z}";

                // Tile �X�N���v�g���擾���ď�����
                TileManager tile = tileObj.GetComponent<TileManager>();
                // ������Ԃł̓A�C�e���Ȃ�
                tile.SetHasItem(false); 

                // ���X�g�ɒǉ����Čォ��Q�Ɖ\��
                allTiles.Add(tile);
            }
        }
    }

    void PlaceItemsRandomly()
    {
        // �ݒ肳�ꂽ���ȓ��ŁA���j�[�N�Ƀ����_���I��
        int treasureCount = Mathf.Min(treasure, allTiles.Count);
        int itemCount = Mathf.Min(item, allTiles.Count);

        List<TileManager> candidates = new List<TileManager>(allTiles);

        //�^�J�����m��ݒ�
        for (int i = 0; i < treasureCount; i++)
        {
            int index = Random.Range(0, candidates.Count);
            TileManager selected = candidates[index];
            selected.SetHasTreasure(true); // �^�J�����m���Z�b�g
            selected.treasureObj = treasurePrefab; //�I�u�W�F�N�g���i�[
            selected.deep = 1;
            candidates.RemoveAt(index); // �d�����Ȃ��悤�Ɍ�₩��폜
        }
        //�z���_�V���m��ݒ�
        for(int j=0;j<itemCount;j++)
        {
            int index = Random.Range(0, candidates.Count);
            TileManager selected = candidates[index];
            selected.SetHasItem(true);  // �z���_�V���m���Z�b�g
            candidates.RemoveAt(index); // �d�����Ȃ��悤�Ɍ�₩��폜
        }
    }

    void PlacePlayerOnStart()
    {
        Vector3 startPos = new Vector3(Mathf.Round(mapSize / 2), 1.25f, Mathf.Round(mapSize / 2));
        GameObject playerObj = Instantiate(playerPrefab, startPos, Quaternion.identity);
    }
}
