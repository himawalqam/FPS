using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isGrounded;
    bool isMoving;

    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);



    void Start()
    {
        controller = GetComponent<CharacterController>();
    }


    void Update()
    {
        //����Ƿ��ڵ���
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //����Ĭ���ٶ�
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }
        //��ȡ����
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //�����ƶ�����
        Vector3 move = transform.right * x + transform.forward * z;

        //player�ƶ�
        controller.Move(move * speed * Time.deltaTime);

        //���Player�Ƿ�����Ծ
        if (Input.GetButton("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //ˤ��
        velocity.y += gravity * Time.deltaTime;

        //��Ծ
        controller.Move(velocity * Time.deltaTime);

        //���player�Ƿ����ƶ�
        if (lastPosition != gameObject.transform.position && isGrounded == true)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        lastPosition = gameObject.transform.position;
    }
}
