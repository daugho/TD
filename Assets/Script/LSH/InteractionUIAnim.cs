using UnityEngine;

public class InteractionUIAnim : MonoBehaviour
{
    private Animator _animator;
    private bool _isOpen = false;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void ToggleMenu()
    {
        Debug.Log($"ToggleMenu called. IsOpen: {_isOpen}");

        if (_isOpen)
        {
            Debug.Log("Trigger: HideTrig");
            _animator.SetTrigger("HideTrigger");
        }
        else
        {
            Debug.Log("Trigger: OpenTrig");
            _animator.SetTrigger("OpenTrigger");
        }

        _isOpen = !_isOpen;
    }

}
