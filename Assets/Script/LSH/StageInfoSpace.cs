using UnityEngine;
using UnityEngine.UI;

public class StageInfoSpace : MonoBehaviour
{
    public Image space1; // ù ��° ����
    public Image space2; // �� ��° ����
    public Sprite sprite1; // ù ��° ��������Ʈ
    public Sprite sprite2; // �� ��° ��������Ʈ

    void Start()
    {
        // ������ ������ ��������Ʈ �߰�
        space1.sprite = sprite1;
        space2.sprite = sprite2;
    }
}
