using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineMeshes : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Combine();
        }
    }

    void Combine()
    {
        // 자식 오브젝트들의 MeshFilter 컴포넌트를 가져온다.
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        // CombineInstance 배열 생성
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int vertexCount = 0;
        for (int i = 0; i < meshFilters.Length; i++)
        {
            // MeshFilter가 null인 경우 continue
            if (meshFilters[i].sharedMesh == null) continue;

            // CombineInstance에 Mesh와 Transform 정보 저장
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            // GameObject 비활성화
            meshFilters[i].gameObject.SetActive(false);

            // 정점 수 추가
            vertexCount += meshFilters[i].sharedMesh.vertexCount;
        }

        // MeshFilter 컴포넌트 가져오기
        MeshFilter meshFilter = GetComponent<MeshFilter>() ?? gameObject.AddComponent<MeshFilter>();
        //MeshFilter meshFilter = transform.GetComponent<MeshFilter>();
        // Mesh 생성
        meshFilter.mesh = new Mesh();

        // 정점 수에 따라 IndexFormat 자동 선택
        if (vertexCount > 65535)
        {
            meshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }

        // CombineMeshes 함수를 사용하여 Mesh 결합
        meshFilter.mesh.CombineMeshes(combine);
        // MeshCollider에 Mesh 할당
        GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
        // GameObject 활성화
        transform.gameObject.SetActive(true);

        // 회전과 위치 초기화
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
    }

}