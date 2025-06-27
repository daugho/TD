using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AddChildImages : MonoBehaviour
{
    [SerializeField] private Image _parentImage; // 부모 이미지
    [SerializeField] private float _spacing = 100f; // 각 자식 이미지 간의 x 간격
    [SerializeField] private float _fixedY = 0f; // y 값 고정
    [SerializeField] private Vector2 _childImageSize = new Vector2(100, 100); // 자식 이미지 크기
    [SerializeField] private Vector2 _firstImageOffset = new Vector2(-300, 0); // 첫 번째 이미지의 위치

    // 몬스터 타입과 스프라이트 매핑
    private Dictionary<DroneTypes, Sprite> _monsterSprites;

    void Awake()
    {
        DataManager.Instance.LoadRoundData();
        InitializeMonsterSprites(); // 스프라이트 매핑 초기화
    }

    private void InitializeMonsterSprites()
    {
        // 몬스터 타입과 스프라이트를 매핑 (예시 데이터)
        _monsterSprites = new Dictionary<DroneTypes, Sprite>
        {
            { DroneTypes.Sentinel, Resources.Load<Sprite>("Prefabs/UI/Monster/Sentinel1") },
            { DroneTypes.Warden, Resources.Load<Sprite>("Prefabs/UI/Monster/Warden1") },
            { DroneTypes.CargoShip, Resources.Load<Sprite>("Prefabs/UI/Monster/CargoShip1") },
            { DroneTypes.Scout, Resources.Load<Sprite>("Prefabs/UI/Monster/Scout1") },
            { DroneTypes.Hunter, Resources.Load<Sprite>("Prefabs/UI/Monster/Hunter1") },
            { DroneTypes.Reaper, Resources.Load<Sprite>("Prefabs/UI/Monster/Reaper1") },
            { DroneTypes.Vanguard, Resources.Load<Sprite>("Prefabs/UI/Monster/Vanguard1") },
            { DroneTypes.Juggernaut, Resources.Load<Sprite>("Prefabs/UI/Monster/Juggernaut1") },
            { DroneTypes.Dreadnought, Resources.Load<Sprite>("Prefabs/UI/Monster/Dreadnought1") }
        };
    }

    public void LoadAndDisplayMonsters(int stage, int wave)
    {
        // 해당 스테이지와 웨이브의 데이터 가져오기
        if (DataManager.Instance.RoundDatas.TryGetValue((stage, wave), out RoundData roundData))
        {
            AddChildren(roundData.Monsters);
        }
        else
        {
            Debug.LogError($"No data found for Stage {stage}, Wave {wave}");
        }
    }

    public void AddChildren(List<MonsterSpawnInfo> monsters)
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            DroneTypes type = monsters[i].Type;

            // 스프라이트 가져오기
            if (!_monsterSprites.TryGetValue(type, out Sprite sprite))
            {
                Debug.LogError($"No sprite found for Monster Type {type}");
                continue;
            }

            // 각 몬스터 타입당 하나의 이미지 생성
            GameObject childImageObj = new GameObject($"Child Image {type}");
            childImageObj.transform.SetParent(_parentImage.transform, false);

            // Child Image 컴포넌트 추가
            Image childImage = childImageObj.AddComponent<Image>();
            childImage.sprite = sprite;

            // RectTransform 설정
            RectTransform rectTransform = childImageObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(_firstImageOffset.x + (i * _spacing), _fixedY);
            rectTransform.sizeDelta = _childImageSize;
        }
    }
    public void RemoveAllChildren()
    {
        Transform imageTransform = _parentImage.GetComponent<Transform>();
        foreach (Transform child in imageTransform)
        {
            Destroy(child.gameObject);
        }
    }
}
