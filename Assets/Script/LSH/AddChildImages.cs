using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AddChildImages : MonoBehaviour
{
    [SerializeField] private Image _parentImage; // �θ� �̹���
    [SerializeField] private float _spacing = 100f; // �� �ڽ� �̹��� ���� x ����
    [SerializeField] private float _fixedY = 0f; // y �� ����
    [SerializeField] private Vector2 _childImageSize = new Vector2(100, 100); // �ڽ� �̹��� ũ��
    [SerializeField] private Vector2 _firstImageOffset = new Vector2(-300, 0); // ù ��° �̹����� ��ġ

    // ���� Ÿ�԰� ��������Ʈ ����
    private Dictionary<DroneTypes, Sprite> _monsterSprites;

    void Awake()
    {
        DataManager.Instance.LoadRoundData();
        InitializeMonsterSprites(); // ��������Ʈ ���� �ʱ�ȭ
    }

    private void InitializeMonsterSprites()
    {
        // ���� Ÿ�԰� ��������Ʈ�� ���� (���� ������)
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
        // �ش� ���������� ���̺��� ������ ��������
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

            // ��������Ʈ ��������
            if (!_monsterSprites.TryGetValue(type, out Sprite sprite))
            {
                Debug.LogError($"No sprite found for Monster Type {type}");
                continue;
            }

            // �� ���� Ÿ�Դ� �ϳ��� �̹��� ����
            GameObject childImageObj = new GameObject($"Child Image {type}");
            childImageObj.transform.SetParent(_parentImage.transform, false);

            // Child Image ������Ʈ �߰�
            Image childImage = childImageObj.AddComponent<Image>();
            childImage.sprite = sprite;

            // RectTransform ����
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
