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

            // ? ���� ����ũ ��ġ�� initial�� �ʱ�ȭ
            effect.mask.transform.position = effect.initialPosition;

            // ����Ʈ ����
            effect.PlayEffect();
        }
        else
        {
            Debug.LogWarning($"[TileBehaviour] InteractiveEffect �Ǵ� Ŀ���� ����ũ�� �����Ǿ����ϴ�: {name}");
        }
    }
}
