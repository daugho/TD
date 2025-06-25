using UnityEngine;

public class SoundTest : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SoundManager.Instance.PlaySFX("SFX_1");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SoundManager.Instance.PlaySFX("SFX_2");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SoundManager.Instance.PlaySFX("SFX_3");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SoundManager.Instance.PlaySFX("SFX_4");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SoundManager.Instance.PlaySFX("SFX_5");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SoundManager.Instance.PlaySFX("SFX_6");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SoundManager.Instance.PlaySFX("SFX_7");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SoundManager.Instance.PlaySFX("SFX_8");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SoundManager.Instance.PlaySFX("SFX_9");
        }
    }
}
