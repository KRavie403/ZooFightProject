using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData
{
    public List<BlockData> blocks;
}

[System.Serializable]
public class BlockData
{
    public int blockNum;                            // 블록 번호
    public int type;                                     // 블록 종류
    public float x;                                        // 블록 위치-x
    public float y;                                        // 블록 위치-y
    public float z;                                        // 블록 위치-z
}
