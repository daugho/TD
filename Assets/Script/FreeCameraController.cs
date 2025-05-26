using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float lookSpeed = 2f;

    void Update()
    {
        // 마우스 우클릭 상태에서만 회전
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
            float mouseY = -Input.GetAxis("Mouse Y") * lookSpeed;
            transform.eulerAngles += new Vector3(mouseY, mouseX, 0f);
        }

        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) move += transform.forward;
        if (Input.GetKey(KeyCode.S)) move -= transform.forward;
        if (Input.GetKey(KeyCode.A)) move -= transform.right;
        if (Input.GetKey(KeyCode.D)) move += transform.right;
        if (Input.GetKey(KeyCode.Q)) move -= transform.up;
        if (Input.GetKey(KeyCode.E)) move += transform.up;

        transform.position += move * moveSpeed * Time.deltaTime;
    }
}
