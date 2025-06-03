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

        MapData data = mapDatas[index];

        if (data == null)
        {
            Debug.LogError($"mapDatas[{index}]가 null입니다.");
            return;
        }

        if (data.blocks == null || data.blocks.Count == 0)
        {
            Debug.LogError("저장할 블록 데이터가 없습니다.");
            return;
        }

        // JSON 직렬화
        string json = JsonUtility.ToJson(data, true);

        // 디렉터리 생성
        string dir = Path.Combine(Application.dataPath, "Resources/MapData");
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
    public MapData LoadMapData(int mapIndex)
    {
        TextAsset mapJson = Resources.Load<TextAsset>($"MapData/Map{mapIndex}");

        if (mapJson == null)
        {
            Debug.LogError($"Map JSON 파일을 찾을 수 없습니다: MapData/Map{mapIndex}");
            return null;
        }

        MapData data = JsonUtility.FromJson<MapData>(mapJson.text);

        if (data == null || data.blocks == null || data.blocks.Count == 0)
        {
            Debug.LogError("Map data load failed or empty.");
            return null;
        }

        return data;
    }

}
