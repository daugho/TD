using Firebase;
using UnityEngine;

public class FirebaseInitializer : MonoBehaviour
{
    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                Debug.Log("? Firebase 초기화 완료");
            }
            else
            {
                Debug.LogError($"? Firebase 초기화 실패: {status}");
            }
        });
    }
}
