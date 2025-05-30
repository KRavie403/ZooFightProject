#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif
using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class MapManager : MonoBehaviour
{
    public MapData[] mapDatas = new MapData[4];         // 4개의 맵 데이터
    public GameObject[] blockPrefabs;                              // 블록 프리팹 (타입별)

    private int currentMapIndex = 0;                                  // 현재 맵 인덱스



    /// <summary>
    /// 외부에서 맵 인덱스를 지정할 때 사용
    /// </summary>
    public void SetCurrentMapIndex(int index)
    {
        if (index >= 0 && index < mapDatas.Length)
        {
            currentMapIndex = index;
        }
        else
        {
            Logger.LogError("잘못된 맵 인덱스입니다.");
        }
    }

    /// <summary>
    /// 현재 맵에 새로운 블록 데이터를 저장
    /// </summary>
    /// <param name="blocks"></param>
    public void SaveMapData(List<BlockData> blocks)
    {
        if (currentMapIndex >= 0 && currentMapIndex < mapDatas.Length)
        {
            MapData mapData = mapDatas[currentMapIndex];

            if (mapData != null)
            {
                if (mapData.blocks == null)
                {
                    mapData.blocks = new List<BlockData>();
                }

                mapData.blocks.AddRange(blocks);  // blocks에 있는 모든 BlockData를 추가

                Logger.Log($"{currentMapIndex}번째 맵 데이터 저장");
            }
            else
            {
                Logger.LogError("현재 맵 데이터가 null입니다.");
            }
        }
        else
        {
            Logger.LogError("잘못된 맵 인덱스입니다.");
        }
    }

    public void ExportMapDataToJson(int index)
    {
#if UNITY_EDITOR
        if (index < 0 || index >= mapDatas.Length)
        {
            Debug.LogError("맵 인덱스가 유효하지 않음");
            return;
        }

        // 맵 데이터가 비어있다면 새로 생성
        if (mapDatas[index] == null)
        {
            Debug.LogWarning($"mapDatas[{index}]가 비어있습니다. 새로운 MapData 인스턴스를 생성합니다.");
            mapDatas[index] = ScriptableObject.CreateInstance<MapData>();
        }

        var data = mapDatas[index];

        if (data.blocks == null || data.blocks.Count == 0)
        {
            Debug.LogError("저장할 블록 데이터가 없습니다.");
            return;
        }

        // JSON 직렬화
        string json = JsonUtility.ToJson(data, true);

        // 디렉터리 생성
        string dir = Path.Combine(Application.dataPath, "05.Maps/Resources");
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        // 파일 경로
        string path = Path.Combine(dir, $"Map{index + 1}.json");
        File.WriteAllText(path, json, Encoding.UTF8);

        Debug.Log($"맵 {index + 1} JSON 저장 완료: {path}");

        AssetDatabase.Refresh();
#endif
    }


    /// <summary>
    /// 맵 데이터를 기반으로 블록들을 인스턴스화
    /// </summary>
    public void LoadMapData(int mapIndex)
    {
        // 0~3 사이 랜덤 인덱스 선택
        TextAsset mapJson = Resources.Load<TextAsset>($"MapData/Map{mapIndex}");

        if (mapJson == null)
        {
            Debug.LogError($"Map JSON 파일을 찾을 수 없습니다: {$"MapData/Map{mapIndex}"}");
            return;
        }
        var mapDatas = JsonUtility.FromJson<MapData>(mapJson.text);

        if (mapDatas == null || mapDatas.blocks == null || mapDatas.blocks.Count == 0)
        {
            Debug.LogError("Map data load failed");
            return;
        }

        
        Logger.Log($"맵 로드 시작 - 맵 번호: {mapIndex}, 블록 수: {mapDatas.blocks.Count}");

        foreach (BlockData block in mapDatas.blocks)
        {
            // 블록의 위치 설정
            Vector3 blockPosition = new Vector3(block.x, block.y, block.z);

            // 블록 타입에 맞는 프리팹 선택
            GameObject blockPrefab = blockPrefabs[block.type];

            // 블록 인스턴스 생성
            GameObject blockInstance;
            if (block.type == 3 || block.type == 4 || block.type == 5)
            {
                blockInstance = Instantiate(blockPrefab, blockPosition, Quaternion.Euler(0, 90, 0));
            }
            else
                blockInstance = Instantiate(blockPrefab, blockPosition, Quaternion.identity);

            // 블록에 Block 스크립트 추가 및 초기화
            //BlockObject blockComponent = blockInstance.AddComponent<BlockObject>();
            //blockComponent.Initialize(block.blockNum, block.type, blockPosition);

            Logger.Log($"블록 생성 - 번호: {block.blockNum}, 타입: {block.type}, 위치: {blockPosition}");
        }
        
    }

    public void LoadMapDataFromBackend()
    {
        Where where = new Where();
        where.Equal("mapIndex", currentMapIndex);

        var bro = Backend.GameData.GetMyData("MapData", where, 1);

        if (!bro.IsSuccess())
        {
            Debug.LogError($"[MapManager] 맵 데이터 로드 실패: {bro.GetMessage()}");
            return;
        }

        if (bro.Rows().Count <= 0)
        {
            Debug.LogWarning($"[MapManager] 맵 인덱스 {currentMapIndex}에 해당하는 데이터가 없습니다.");
            return;
        }

        JsonData row = bro.Rows()[0];

        // 서버에서 받아온 blocks 배열
        JsonData blocksJson = row["blocks"];
        List<BlockData> blockDataList = new List<BlockData>();

        for (int i = 0; i < blocksJson.Count; i++)
        {
            JsonData b = blocksJson[i];
            BlockData block = new BlockData()
            {
                blockNum = int.Parse(b["blockNum"].ToString()),
                type = int.Parse(b["type"].ToString()),
                x = float.Parse(b["x"].ToString()),
                y = float.Parse(b["y"].ToString()),
                z = float.Parse(b["z"].ToString())
            };
            blockDataList.Add(block);
        }

        Debug.Log($"[MapManager] 블록 개수: {blockDataList.Count}");

        foreach (BlockData block in blockDataList)
        {
            Vector3 position = new Vector3(block.x, block.y, block.z);
            GameObject prefab = blockPrefabs[block.type];

            GameObject instance;
            if (block.type == 3 || block.type == 4 || block.type == 5)
            {
                instance = Instantiate(prefab, position, Quaternion.Euler(0, 90, 0));
            }
            else
            {
                instance = Instantiate(prefab, position, Quaternion.identity);
            }

            Debug.Log($"[MapManager] 블록 생성 - 번호: {block.blockNum}, 타입: {block.type}, 위치: {position}");
        }
    }
}
