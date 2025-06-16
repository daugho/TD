using INab.Common;
using UnityEngine;
using UnityEngine.UIElements;

public class TestMaskMover : MonoBehaviour
{
    public Transform _customMaskTransform;
    void Start()
    {
        var effect = GetComponent<InteractiveEffect>();
        if (effect != null && _customMaskTransform != null && effect.mask != null)
        {
            effect.initialPosition = new Vector3(1f, 1f, 1f);
            effect.finalPosition = new Vector3(1f, 1f, 3.7f);

            // ? 실제 마스크 위치를 initial로 초기화
            effect.mask.transform.position = effect.initialPosition;

            // 이펙트 실행
            effect.PlayEffect();
        }
        else
        {
            Debug.LogWarning($"[TileBehaviour] InteractiveEffect 또는 커스텀 마스크가 누락되었습니다: {name}");
        }
    }
}
