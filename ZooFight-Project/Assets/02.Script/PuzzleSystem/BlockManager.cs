using BackEnd.Tcp;
using BackEnd;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BlockType
{
    Red = 0,
    Blue = 1,
    Block2x1_0 = 2,
    Block2x1_1,
    Block2x1_2,
    Block1x2_0,
    Block1x2_1,
    Block1x2_2,
    Block1x1_0,
    Block1x1_1,
    Block1x1_2,
    Block1x1_3,
    Block1x1_4
}


public class BlockManager : MonoBehaviour
{
    static public BlockManager instance;

    // 맵 정보
    private float mapWidth = 60.0f, mapHeight = 40.0f;

    // 블록 정보
    public GameObject[] block1x1, block1x2, block2x1;
    public GameObject redBlock, blueBlock;
    private Dictionary<int, GameObject> blockDict = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> GetBlockDict() => blockDict;

    // 블록 위치 
    public List<BlockData> blocks = new List<BlockData>();
    private HashSet<Vector3> occupied = new HashSet<Vector3>();
    public Dictionary<int, Vector3> mapCoordinates = new Dictionary<int, Vector3>();
    private Dictionary<int, Vector3> placedBlockPositions = new Dictionary<int, Vector3>();

    public MapManager mapManager; // MapManager를 참조
    private MapData loadedMapData;
    public int mapId = 0;
    private SessionId myPlayerIndex = SessionId.None;

    // CharacterPlacement.cs에서 User Position 값 받음
    private List<Vector3> receivedSpawnUsers = new List<Vector3>();
    private List<Vector3> receivedSpawnItems = new List<Vector3>();

    // 시드값
    public int rnd = 0;
  
    // 로그 확인
    private int blockCount = 0;     // pinkBlock:0 blueBlock:1
    private int blockNum1 = 0;
    private int blockNum2 = 0;
    private int blockNum3 = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
#if UNITY_EDITOR
        //// 랜덤 고정 (seed)
        //SelectSeed(52);
        //Random.InitState(rnd);

        //// 맵 좌표 생성(40x60)
        ////InitializeGrid();

        //// 블록 배치
        ////PlaceBlocks();

        //// 레드, 블루 고정 위치에 배치
        //PlaceTeamBlocks();

        //// 블록 랜덤 배치 (그리드 + 확률 기반)
        //GridBasedWeightedPlacement();

        //// 맵 결과 저장
        //SaveMapData();

        //// -> JSON으로 추출 (index 예: 0, 1, 2, 3 중 하나)
        //mapManager.ExportMapDataToJson(0);
#endif


        myPlayerIndex = Backend.Match.GetMySessionId();

        // 저장된 맵 불러오기
        if (BackEndMatchManager.GetInstance().IsHost())
        {
            mapId = UnityEngine.Random.Range(1, 5);

            Logger.Log($"호스트가 정한 맵 번호: {mapId}");

            MapIdMessage message = new MapIdMessage(mapId);
            BackEndMatchManager.GetInstance().SendDataToInGame<MapIdMessage>(message);

            loadedMapData = mapManager.LoadMapData(mapId);

            if (loadedMapData != null)
                PlaceBlocks(loadedMapData);
            else
                Debug.LogError("Map data load failed");
        }
        else
        {
            Logger.Log($"호스트 아님");
        }
    }

    private void SelectSeed(int seed)
    {
        rnd = seed;
    }

    /*private void InitializeGrid()
    {
        //for (float y = 0.5f; y < mapHeight + 0.5f; y++)
        //{
        //    for (float x = 0.5f; x < mapWidth + 0.5; x++)
        //    {
        //        mapCoordinates.Add(mapNum, new Vector3(x, 0.5f, y));
        //        mapNum++;
        //    }
        //}
    }
    */

    private void PlaceBlocks(MapData mapData)
    {
        foreach (BlockData block in mapData.blocks)
        {
            if (block.type == 0 || block.type == 1) continue;

            Vector3 blockPosition = new Vector3(block.x, block.y, block.z);

            GameObject blockPrefab = GetBlockPrefab(block.type);

            Quaternion rot = (block.type >= 2 && block.type <= 4)
            ? Quaternion.Euler(0, 90, 0)
            : Quaternion.identity;

            Instantiate(blockPrefab, blockPosition, rot);

#if UNITY_EDITOR || DEBUG
            Debug.Log($"블록 생성 - 번호: {block.blockNum}, 타입: {block.type}, 위치: {blockPosition}");
#endif
        }
    }

    private GameObject GetBlockPrefab(int type)
    {
        Logger.Log($"block type: {type}");
        // 타입에 따라 프리팹 리턴 (직접 조정)
        if (type == 0) return redBlock;
        if (type == 1) return blueBlock;
        if (type >= 2 && type <= 4) return block2x1[type - 2];
        if (type >= 5 && type <= 7) return block1x2[type - 5];
        else return block1x1[type - 8];
    }

    private void PlaceTeamBlocks()
    {

        Vector3 redBlockPosition = new Vector3(20.5f, 0.5f, 10.5f);
        Vector3 blueBlockPosition = new Vector3(40.5f, 0.5f, 10.5f);

        //Instantiate(redBlock, redBlockPosition, Quaternion.identity);
        //Instantiate(blueBlock, blueBlockPosition, Quaternion.identity);

        occupied.Add(redBlockPosition);
        occupied.Add(blueBlockPosition);

        AddBlockData(redBlockPosition, BlockType.Red);
        blockCount++;
        AddBlockData(blueBlockPosition, BlockType.Blue);
        blockCount++;
    }

    private void GridBasedWeightedPlacement()
    {
        int perQuadrant = 300;
        int totalBlocks = 0;

        float midX = mapWidth / 2;
        float midZ = mapHeight / 2;

        Vector2[] xRanges = new Vector2[]
        {
        new Vector2(0, midX),         // 좌상
        new Vector2(midX, mapWidth),  // 우상
        new Vector2(0, midX),         // 좌하
        new Vector2(midX, mapWidth)   // 우하
        };

        Vector2[] zRanges = new Vector2[]
        {
        new Vector2(midZ, mapHeight), // 좌상
        new Vector2(midZ, mapHeight), // 우상
        new Vector2(0, midZ),         // 좌하
        new Vector2(0, midZ)          // 우하
        };

        for (int q = 0; q < 4; q++)
        {
            List<Vector3> candidates = new();

            for (float z = zRanges[q].x + 0.5f; z < zRanges[q].y; z++)
            {
                for (float x = xRanges[q].x + 0.5f; x < xRanges[q].y; x++)
                {
                    candidates.Add(new Vector3(x, 0.5f, z));
                }
            }

            // 무작위로 섞기
            candidates = candidates.OrderBy(_ => UnityEngine.Random.value).ToList();

            int count = 0;
            foreach (var cell in candidates)
            {
                if (count >= perQuadrant) break;
                if (occupied.Contains(cell)) continue;
                if (!HasFreeAdjacent(cell)) continue;

                float roll = UnityEngine.Random.value;

                if (roll < 0.3f)
                    TryPlace1x1(cell);
                else if (roll < 0.65f)
                    TryPlace1x2(cell);
                else
                    TryPlace2x1(cell);

                blockCount++;
                count++;
            }
        }
    }


    private bool HasFreeAdjacent(Vector3 cell)
    {
        Vector3[] directions = {
        Vector3.forward, Vector3.back,
        Vector3.left, Vector3.right
    };

        int freeCount = 0;

        foreach (var dir in directions)
        {
            Vector3 adjacent = cell + dir;
            if (!occupied.Contains(adjacent))
            {
                freeCount++;
                if (freeCount >= 2)
                    return true;
            }
        }

        return false;
    }

    private void TryPlace1x1(Vector3 basePos)
    {
        if (occupied.Contains(basePos)) return;

        blockNum1 %= block1x1.Length;

        AddBlockData(basePos, BlockType.Block1x1_0 + blockNum1);


        Logger.Log($"blockCount: {blockCount}");

        occupied.Add(basePos);

        Instantiate(block1x1[blockNum1++], basePos, Quaternion.identity);
    }

    private void TryPlace1x2(Vector3 basePos)
    {
        Vector3 next = basePos + new Vector3(0, 0, 1);

        if (occupied.Contains(basePos) || occupied.Contains(next)) return;

        blockNum2 %= block1x2.Length;

        Vector3 mid = basePos + new Vector3(0, 0, 0.5f);


        Logger.Log($"blockCount: {blockCount}");

        occupied.Add(basePos);
        occupied.Add(next);

        AddBlockData(mid, BlockType.Block1x2_0 + blockNum2);
        
        Instantiate(block1x2[blockNum2++], mid, Quaternion.identity);
    }

    private void TryPlace2x1(Vector3 basePos)
    {
        Vector3 next = basePos + new Vector3(1, 0, 0);

        if (occupied.Contains(basePos) || occupied.Contains(next)) return;

        blockNum3 %= block2x1.Length;
        Vector3 mid = basePos + new Vector3(0.5f, 0, 0);


        Logger.Log($"blockCount: {blockCount}");

        occupied.Add(basePos);
        occupied.Add(basePos);
        occupied.Add(next);

        AddBlockData(mid, BlockType.Block2x1_0 + blockNum3);

        Instantiate(block2x1[blockNum3++], mid, Quaternion.Euler(0, 90, 0));
    }

    void AddBlockData(Vector3 pos, BlockType type)
    {
        // 새로운 블록 데이터 생성
        blocks.Add(new BlockData
        {
            blockNum = blockCount,
            type = (int)type,
            x = pos.x,
            y = pos.y,
            z = pos.z
        });
    }

    public void SaveMapData()
    {
        mapManager.SaveMapData(blocks);
    }

    public void LoadAndPlaceBlocks(int mapIndex)
    {
        MapData mapData = mapManager.LoadMapData(mapIndex); // 데이터만 가져옴

        if (mapData == null || mapData.blocks == null)
        {
            Debug.LogError("MapData is null or empty.");
            return;
        }

        Debug.Log($"[BlockPlacement] 맵 데이터 로딩 및 배치 시작 - 블록 수: {mapData.blocks.Count}");

        foreach (var block in mapData.blocks)
        {
            Vector3 blockPosition = new Vector3(block.x, block.y, block.z);
            GameObject blockPrefab = block.type < 5 ? block2x1[block.type - 2] : block.type < 8 ? block1x2[block.type - 5] : block1x1[block.type - 8];

            GameObject blockInstance;
            if (block.type == 5 || block.type == 6 || block.type == 7)
                blockInstance = Instantiate(blockPrefab, blockPosition, Quaternion.Euler(0, 90, 0));
            else
                blockInstance = Instantiate(blockPrefab, blockPosition, Quaternion.identity);

            // blockNum 등록
            blockDict[block.blockNum] = blockInstance;

            Debug.Log($"블록 생성 - 번호: {block.blockNum}, 타입: {block.type}, 위치: {blockPosition}");
        }
    }

    public void SetSpawnUsers(List<Vector3> spawnUsers)
    {
        if (receivedSpawnUsers == null)
        {
            receivedSpawnUsers = new List<Vector3>();
        }
        receivedSpawnUsers.Clear();
        receivedSpawnUsers.AddRange(spawnUsers);
    }

    public void SetSpawnItems(List<Vector3> spawnItems)
    {
        if (receivedSpawnItems == null)
        {
            receivedSpawnItems = new List<Vector3>();
        }
        receivedSpawnItems.Clear();
        receivedSpawnItems.AddRange(spawnItems);
    }

    public void OnRecieve(MatchRelayEventArgs args)
    {
        if (args.BinaryUserData == null)
        {
            Logger.LogWarning(string.Format("빈 데이터가 브로드캐스팅 되었습니다.\n{0} - {1}", args.From, args.ErrInfo));
            // 데이터가 없으면 그냥 리턴
            return;
        }
        Message msg = DataParser.ReadJsonData<Message>(args.BinaryUserData);
        if (msg == null)
        {
            return;
        }
        if (BackEndMatchManager.GetInstance().IsHost() != true && args.From.SessionId == myPlayerIndex)
        {
            return;
        }
        switch (msg.type)
        {
            case Protocol.Type.MapId:
                MapIdMessage mapIdMessage = DataParser.ReadJsonData<MapIdMessage>(args.BinaryUserData);
                ProcessMapData(mapIdMessage);
                break;
            default:
                Logger.Log("Unknown protocol type");
                return;
        }
    }

    private void ProcessMapData(MapIdMessage data)
    {
        loadedMapData = mapManager.LoadMapData(data.mapId);

        Logger.Log($"호스트로부터 받은 맵 번호: {data.mapId}");

        if (loadedMapData != null)
            PlaceBlocks(loadedMapData);
        else
            Logger.LogError("Map data load failed");
    }

    /*private void PlaceBlocks()
    {

        //// 랜덤으로 캐릭터 배치
        //for (int i = 0; i < receivedSpawnUsers.Count; i++)
        //{
        //    mapCoordinates[FindMapCoordinatesKey(mapCoordinates, receivedSpawnUsers[i])] = new Vector3(0, 0, 0);
        //    MarkPositionAsOccupied(receivedSpawnUsers[i]);
        //}

        // 랜덤으로 블록 장애물 배치
        
        while (blockCount < 800
    // & maxCount < 400)
        {
            float x = UnityEngine.Random.Range(0f, mapWidth + 1.0f);
            float y = UnityEngine.Random.Range(0f, mapHeight + 1.0f);
            x -= x % 1;
            y -= y % 1;

            float randomValue = UnityEngine.Random.Range(0f, 1f);

            Vector3 blockPosition = new Vector3(x + 0.5f, 0.5f, y + 0.5f);

            // 위치가 맵을 벗어나지 않고, 높이가 mapHeight를 너비가 mapWidth 넘지 않는지 확인
            if (IsPositionInsideMap(blockPosition))
            {
                // 80% 확률로 1:2 또는 2:1 블록 생성
                if (randomValue < 0.8f)
                {
                    if (Random.Range(0f, 1f) < 0.5f) // 50% 확률로 1:2 블록 생성
                    {
                        if (mapCoordinates[FindMapCoordinatesKey(mapCoordinates, blockPosition)] != Vector3.zero)
                        {
                            blockPosition.z += 1.0f;
                            if (mapCoordinates[FindMapCoordinatesKey(mapCoordinates, blockPosition)] != Vector3.zero)
                            {
                                mapCoordinates[FindMapCoordinatesKey(mapCoordinates, blockPosition)] = new Vector3(0, 0, 0);
                                blockPosition.z -= 1.0f;
                                mapCoordinates[FindMapCoordinatesKey(mapCoordinates, blockPosition)] = new Vector3(0, 0, 0);
                                blockPosition.z += 0.5f;

                                blockNum1 %= block1x2.Length;
                                //Debug.Log("blockNum1: " + blockNum1);
                                AddBlockData(blockCount, blockNum1, blockPosition.x, blockPosition.y, blockPosition.z);
                                Instantiate(block1x2[blockNum1++], blockPosition, Quaternion.identity);
                                MarkPositionAsOccupied(blockCount, blockPosition);
                                blockCount++;
                                //UpdateMapData(blockCount, blockNum, blockPosition.x, blockPosition.y, blockPosition.z);
                            }
                        }
                    }
                    else if (Random.Range(0f, 1f) < 0.5f)// 나머지 50% 확률로 2:1 블록 생성
                    {
                        if (mapCoordinates[FindMapCoordinatesKey(mapCoordinates, blockPosition)] != Vector3.zero)
                        {
                            blockPosition.x += 1.0f;
                            if (mapCoordinates[FindMapCoordinatesKey(mapCoordinates, blockPosition)] != new Vector3(0, 0, 0))
                            {
                                mapCoordinates[FindMapCoordinatesKey(mapCoordinates, blockPosition)] = new Vector3(0, 0, 0);
                                blockPosition.x -= 1.0f;
                                mapCoordinates[FindMapCoordinatesKey(mapCoordinates, blockPosition)] = new Vector3(0, 0, 0);
                                blockPosition.x += 0.5f;

                                blockNum2 %= block2x1.Length;
                                //Debug.Log($"blockNum2: {blockNum2 + 3}");
                                AddBlockData(blockCount, blockNum2 + 3, blockPosition.x, blockPosition.y, blockPosition.z);
                                Instantiate(block2x1[blockNum2++], blockPosition, Quaternion.Euler(0, 90, 0)); //Y 축으로 90도 회전 => Quaternion.identity나중에 수정
                                MarkPositionAsOccupied(blockCount, blockPosition);
                                blockCount++;
                                //UpdateMapData(blockCount, blockNum, blockPosition.x, blockPosition.y, blockPosition.z);
                            }
                        }
                    }
                }
                else // 나머지 20% 확률로 1:1 블록 생성
                {
                    if (mapCoordinates[FindMapCoordinatesKey(mapCoordinates, blockPosition)] != Vector3.zero)
                    {
                        mapCoordinates[FindMapCoordinatesKey(mapCoordinates, blockPosition)] = new Vector3(0, 0, 0);

                        blockNum3 %= block1x1.Length;
                        //Debug.Log($"blockNum3: {blockNum3 + 6}");
                        AddBlockData(blockCount, blockNum3 + 6, blockPosition.x, blockPosition.y, blockPosition.z);
                        Instantiate(block1x1[blockNum3++], blockPosition, Quaternion.identity);
                        MarkPositionAsOccupied(blockCount, blockPosition);
                        //UpdateMapData(blockCount, blockNum, blockPosition.x, blockPosition.y, blockPosition.z);
                        blockCount++;
                    }
                }
                maxCount = 0;
#if DEBUG || UNITY_EDITOR
                //Debug.Log("blockCount: " + blockCount);
#endif
            }
        }
        //SaveMapData();        // 맵 데이터 저장

        //for (int i = 0; i < receivedSpawnItems.Count; i++)
        //{
        //    if (mapCoordinates[FindMapCoordinatesKey(mapCoordinates, receivedSpawnItems[i])] != Vector3.zero)
        //    {
        //        mapCoordinates[FindMapCoordinatesKey(mapCoordinates, receivedSpawnItems[i])] = new Vector3(0, 0, 0);
        //        MarkPositionAsOccupied(receivedSpawnItems[i]);
        //    }
        //}
    }


    public static int FindMapCoordinatesKey(Dictionary<int, Vector3> mapCoordinates, Vector3 position)
    {
        foreach (KeyValuePair<int, Vector3> kvp in mapCoordinates)
        {
            if (kvp.Value == position)
            {
                return kvp.Key;
            }
        }
        return 0;
    }

    public void MarkPositionAsOccupied(int num, Vector3 position)   // (Key)블록ID  (Value)및 위치
    {
        placedBlockPositions.Add(num, position);
    }

    bool IsPositionInsideMap(Vector3 position)
    {
        return position.x < mapWidth + 0.5f && position.z < mapHeight + 0.5f;
    }

    public void SetSpawnUsers(List<Vector3> spawnUsers)
    {
        if (receivedSpawnUsers == null)
        {
            receivedSpawnUsers = new List<Vector3>();
        }
        receivedSpawnUsers.Clear();
        receivedSpawnUsers.AddRange(spawnUsers);
    }

    public void SetSpawnItems(List<Vector3> spawnItems)
    {
        if (receivedSpawnItems == null)
        {
            receivedSpawnItems = new List<Vector3>();
        }
        receivedSpawnItems.Clear();
        receivedSpawnItems.AddRange(spawnItems);
    }

    public void SaveMapData()
    {
        mapManager.SaveMapData(blocks);
    }

    //public void LoadMapData()
    //{
    //    placedBlockPositions = mapManager.LoadMapData();

    //    foreach (var kvp in placedBlockPositions)
    //    {
    //        Instantiate(GetBlockPrefabById(kvp.Key), kvp.Value, Quaternion.identity);
    //    }
    //}

    GameObject GetBlockPrefabById(int id)
    {
        // 블록 ID에 따라 적절한 블록 prefab을 반환하는 로직을 구현하세요
        if (id < block1x1.Length)
        {
            return block1x1[id];
        }
        else if (id < block1x1.Length + block1x2.Length)
        {
            return block1x2[id - block1x1.Length];
        }
        else
        {
            return block2x1[id - block1x1.Length - block1x2.Length];
        }
    }
*/
}
