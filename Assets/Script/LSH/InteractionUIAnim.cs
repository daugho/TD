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
            _animator.SetBool("IsUIActive", true);
        }
        else
        {
            _animator.SetBool("IsUIActive", false);
        }

        _isOpen = !_isOpen;
    }

}
