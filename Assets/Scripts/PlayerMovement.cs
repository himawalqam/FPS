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
        //检查是否在地面
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //重置默认速度
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }
        //获取输入
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //创建移动向量
        Vector3 move = transform.right * x + transform.forward * z;

        //player移动
        controller.Move(move * speed * Time.deltaTime);

        //检查Player是否能跳跃
        if (Input.GetButton("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //摔倒
        velocity.y += gravity * Time.deltaTime;

        //跳跃
        controller.Move(velocity * Time.deltaTime);

        //检查player是否在移动
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
