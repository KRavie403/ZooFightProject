using UnityEngine;

public class BlockSettings : Singleton<BlockSettings>
{

    private GameObject selectedBlock;

    public void SelectBlock(GameObject block)
    {
        // 기존 블록의 테두리를 제거
        if (selectedBlock != null && selectedBlock != block)
        {
            selectedBlock.transform.Find("Outline").gameObject.SetActive(false);
        }

        // 새로운 블록의 테두리를 활성화
        selectedBlock = block;
        selectedBlock.transform.Find("Outline").gameObject.SetActive(true);
    }
}
