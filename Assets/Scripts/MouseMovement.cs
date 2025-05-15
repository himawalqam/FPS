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
        //将光标锁定在屏幕中间并使其不可见
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        //获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //绕x轴旋转(上下查看)
        xRotation -= mouseY;

        //限制旋转角度
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

        //绕y轴旋转(左右查看)
        yRotation += mouseX;

        //旋转应用于transform
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);

    }
}
