using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    float xRotation = 0;
    float yRotation = 0;

    public float topClamp = -90f;
    public float bottomClamp = 90f;

    void Start()
    {
        //�������������Ļ�м䲢ʹ�䲻�ɼ�
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        //��ȡ�������
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //��x����ת(���²鿴)
        xRotation -= mouseY;

        //������ת�Ƕ�
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

        //��y����ת(���Ҳ鿴)
        yRotation += mouseX;

        //��תӦ����transform
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);

    }
}
