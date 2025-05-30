using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    private float mapWidth = 60.0f;
    private float mapHeight = 40.0f;

    // 아이템 생성 위치
    //public Transform[] points;
    public List<Vector3> spawnItems = new List<Vector3>();

    // 아이템 prefab 저장할 변수
    public GameObject[] items;

    // 아이템 생성할 주기
    public float createTime = 2.0f;

    // 적 아이템의 최대 생성 개수
    public int maxItems = 40;

    // 게임 종료 여부 판단
    public bool isGameOver = false;

    private BlockPlacement blockPlacement = null;


    // Start is called before the first frame update
    void Start()
    {
        //Debug Start SpawnItem.cs
        Debug.Log("Start SpawnItem.cs");

        blockPlacement = GetComponent<BlockPlacement>();

        //points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();   
    
        if(spawnItems.Count >= 0)
        {
            //Debug
            Debug.Log("Start CreateItem");
            StartCoroutine(this.CreateItem());
        }
    }

    IEnumerator CreateItem()
    {
        while(!isGameOver)
        {
            int targetLayer = LayerMask.NameToLayer("Item");
            GameObject[] allItems = GameObject.FindObjectsOfType<GameObject>();
            int itemCount = 0;

            //Debug
            Debug.Log("itemCount: " + itemCount);
            //foreach (GameObject obj in allItems)
            //{
            //    if (obj.layer == targetLayer)
            //    {
            //        itemCount++;
            //    }
            //}

            float x = UnityEngine.Random.Range(0f, mapWidth + 1.0f);
            float y = UnityEngine.Random.Range(0f, mapHeight + 1.0f);
            x -= x % 1;
            y -= y % 1;

            Vector3 itemPosition = new Vector3(x + 0.5f, 0.5f, y + 0.5f);

            if (IsPositionInsideMap(itemPosition))
            {
                if (itemPosition == new Vector3(20.5f, 0.5f, 10.5f) || itemPosition == new Vector3(40.5f, 0.5f, 10.5f))
                {
                    continue;
                }

                if (itemCount < maxItems)
                {
                    yield return new WaitForSeconds(createTime);

                    int idx = Random.Range(1, spawnItems.Count);
                    spawnItems.Add(itemPosition);
                    
                    //Debug
                    Debug.Log("Instantiate Item:" + itemCount);

                    //for (int i = 0; i < spawnItems.Count; i++)
                    //{
                    //    if (blockPlacement.mapCoordinates[FindMapCoordinatesKey(mapCoordinates, spawnItems[i])] != Vector3.zero)
                    //    {
                    //        blockPlacement.mapCoordinates[FindMapCoordinatesKey(mapCoordinates, spawnItems[i])] = new Vector3(0, 0, 0);
                    //        blockPlacement.MarkPositionAsOccupied(spawnItems[i]);
                    //    }
                    //}
                    Instantiate(items[idx], itemPosition, Quaternion.identity);
                    blockPlacement.SetSpawnItems(spawnItems);
                    itemCount++;
                }
                else yield return null;
            }
            else yield return null;
        }
    }

    bool IsPositionInsideMap(Vector3 position)
    {
        return position.x < mapWidth + 0.5f && position.z < mapHeight + 0.5f;
    }
}
