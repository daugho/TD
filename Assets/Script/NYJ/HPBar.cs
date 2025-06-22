using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public void SetMaxHp(float maxHp)
    {
        _slider.maxValue = maxHp;
        _slider.value = maxHp;
    }

    public void SetHp(float hp)
    {
        _slider.value = hp;
    }
}
