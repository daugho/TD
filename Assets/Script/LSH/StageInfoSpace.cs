using UnityEngine;
using UnityEngine.UI;

public class StageInfoSpace : MonoBehaviour
{
    public Image space1; // 첫 번째 공간
    public Image space2; // 두 번째 공간
    public Sprite sprite1; // 첫 번째 스프라이트
    public Sprite sprite2; // 두 번째 스프라이트

    void Start()
    {
        // 각각의 공간에 스프라이트 추가
        space1.sprite = sprite1;
        space2.sprite = sprite2;
    }
}
