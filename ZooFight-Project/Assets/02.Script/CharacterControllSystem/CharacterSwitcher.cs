using UnityEngine;
using UnityEngine.UI;

public class CharacterSwitcher : MonoBehaviour
{
    public GameObject[] characterPrefabs;           // 캐릭터 프리팹 배열
    public Vector3 spawnPoint = Vector3.zero;    // 캐릭터가 스폰될 위치

    private int _curIndex = 0;                    // 현재 캐릭터 인덱스
    [SerializeField]private GameObject currentCharacter;    // 현재 활성화된 캐릭터

    [SerializeField]private Button _rightButton; 
    [SerializeField]private Button _leftButton;
    [SerializeField]private Button _selectButton;       // 캐릭터 선택 버튼

    private void Awake()
    {
        BackendGameData.Inst.onGameDataLoadEvent.AddListener(UpdateGameData);
        if (currentCharacter != null)
        {
            spawnPoint = currentCharacter.transform.position;
        }

    }
    private void Start()
    {
        // 처음 캐릭터를 스폰
        SpawnCharacter(_curIndex);

        //// 버튼 클릭 이벤트에 메서드 연결
        //_rightButton.onClick.AddListener(OnRightButtonClick);
        //_leftButton.onClick.AddListener(OnLeftButtonClick);
    }

    public void SelectCharacter()
    {
        //Gamemanaer.userCharacter = curIndex; // 캐릭터 번호 넘기기
        BackendGameData.Inst.UserGameData.character = _curIndex;
        BackendGameData.Inst.GameDataUpdate();

        Debug.Log($"저장 후 캐릭터 번호: {BackendGameData.Inst.UserGameData.character}");
    }

    public void OnRightButtonClick()
    {
        // 인덱스를 증가시키고 배열 크기를 넘어가면 0으로 설정
        _curIndex = (_curIndex + 1) % characterPrefabs.Length;
        SpawnCharacter(_curIndex);
    }

    public void OnLeftButtonClick()
    {
        // 인덱스를 감소시키고 0보다 작아지면 배열의 마지막 인덱스로 설정
        _curIndex = (_curIndex - 1 + characterPrefabs.Length) % characterPrefabs.Length;
        SpawnCharacter(_curIndex);
    }

    private void SpawnCharacter(int index)
    {
        // 기존 캐릭터 삭제
        if (currentCharacter != null)
        {
            // 기존 캐릭터의 transform 값을 가져옴
            Vector3 oldPosition = currentCharacter.transform.position;
            Quaternion oldRotation = currentCharacter.transform.rotation;
            Vector3 oldScale = currentCharacter.transform.localScale;

            Destroy(currentCharacter);

            // 새로운 캐릭터 스폰
            currentCharacter = Instantiate(characterPrefabs[index], oldPosition, oldRotation);
            currentCharacter.transform.localScale = oldScale;  // 기존 스케일 값 적용
        }
        else
        {
            // 처음 스폰할 때는 spawnPoint 사용
            currentCharacter = Instantiate(characterPrefabs[index], spawnPoint, Quaternion.identity);
        }
    }

    public void UpdateGameData()
    {
        _curIndex = BackendGameData.Inst.UserGameData.character;
        Debug.Log($"업데이트 된 캐릭터 번호: {_curIndex}");
    }
}
