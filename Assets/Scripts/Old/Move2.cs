using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move2 : MonoBehaviour
{
    public float speedMove;
   // public float jumpPower;

    private float gravityForce; // ���������� ���������
    private Vector3 moveVector; // ����������� �������� ���������
    private Quaternion target;
    private bool isHalfLeft, isHalfRight;

    //������ �� ����������

    // private CharacterController ch_controller;
    /*
    private Animator ch_animator;

    */

    private Rigidbody rig;

    private void Start()
    {   
       // ch_controller = GetComponent<CharacterController>();
        /*
        ch_animator = GetComponent<Animator>();
        */
        rig = GetComponent<Rigidbody>();
        //transform.rotation = Quaternion.Euler(0, -90f, 0);
    }

    private void FixedUpdate()
    {
        CharacterMove();
      //  GamingGravity();
    }

    // ����� ����������� ���������
    private void CharacterMove()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.localPosition += transform.forward * speedMove * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.localPosition += -transform.forward * speedMove * Time.deltaTime;
        }
      /*  if (Input.GetKey(KeyCode.A))
        {
            transform.localPosition += -transform.right * speedMove * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.localPosition += transform.right * speedMove * Time.deltaTime;
        }
    */
        //  if (ch_controller.isGrounded)
        //{
        //����������� �� ����������� ����� Rigidbody ��� � Move1
        /*
        moveVector = Vector3.zero;
        moveVector.x = Input.GetAxis("Horizontal") * speedMove;
        moveVector.z = Input.GetAxis("Vertical") * speedMove;
        rig.velocity = moveVector;
        */
        // Debug.Log(moveVector.x);
        // Debug.Log(moveVector.z);

        //��� �������� ��� ������������� ����������� ����� transform
        moveVector.x = Input.GetAxis("Horizontal") * speedMove;
        moveVector.z = Input.GetAxis("Vertical") * speedMove;
        //�������� ���������
        /*
        if (moveVector.x != 0 || moveVector.z != 0)
        {
            ch_animator.SetBool("Move", true);
        }
        else
        {
            ch_animator.SetBool("Move", false);
        }
         */
        //������� ���������
        /*
        if (Vector3.Angle(Vector3.forward, moveVector) > 1f || Vector3.Angle(Vector3.forward, moveVector) == 0)
        {
            Vector3 direct = Vector3.RotateTowards(transform.forward, moveVector, speedMove*Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(direct);
        }
        */
        //������ �������� �� moveVector � rigidbody
        /*
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) 
        {
            Vector3 direct = Vector3.RotateTowards(transform.forward, moveVector, speedMove * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(direct);
            moveVector = Vector3.zero;
        }
        */
        //������ �������� �� transform, �� � moveVector
        /*
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveVector), speedMove * Time.deltaTime);
            Debug.Log(transform.rotation);
        }
        */
        // ������� ����� ���� ������ ���������
        /*
        if (Input.GetKey(KeyCode.A))
        {
            target = transform.localRotation;

            if (target.y <= 0 && target.y > -0.7f) 
            {
                target = Quaternion.Euler(0, -90f, 0);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, target, speedMove * Time.deltaTime);
                target.w = 0.7f;
            }

            
            
            if (target.y <= -0.7f && target.y > -1f) 
            {
                target = Quaternion.Euler(0, -179.9f, 0);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, target, speedMove * Time.deltaTime);
                isHalfLeft = true;
            }

            if (target.y == -1f)
            {
                target = Quaternion.Euler(0, -270f, 0);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, target, speedMove * Time.deltaTime);
            }

            //if (target.y <= -0.7f && target.y >= -1f) target = Quaternion.Euler(0, -170f, 0);
            // target = Quaternion.Euler(0, -270f, 0);
            // target.y -= 0.1f;

            Debug.Log(target.y);
            //transform.localRotation = Quaternion.Lerp(transform.localRotation, target, speedMove * Time.deltaTime);
            


        }

        if (Input.GetKey(KeyCode.D))
        {
            target = transform.localRotation;

            if (target.y >= 0 && target.y < 0.7f)
            {
                target = Quaternion.Euler(0, 90f, 0);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, target, speedMove * Time.deltaTime);
                target.w = 0.7f;
            }

            
            if (target.y >= 0.7f && target.y <= 1f)
            {
                target = Quaternion.Euler(0, 179.9f, 0);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, target, speedMove * Time.deltaTime);
                isHalfRight = true;
            }

            if (target.y>0.9999f && target.y < 1.009f)
            {
                target = Quaternion.Euler(0, 270f, 0);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, target, speedMove * Time.deltaTime);
            }

            //if (target.y <= -0.7f && target.y >= -1f) target = Quaternion.Euler(0, -170f, 0);
            // target = Quaternion.Euler(0, -270f, 0);
            // target.y -= 0.1f;

            Debug.Log(target.y);
            //transform.localRotation = Quaternion.Lerp(transform.localRotation, target, speedMove * Time.deltaTime);



        }

        // }
        */
            moveVector.y = gravityForce; // ������ ����������, ��������� ����� ��������!!!
      //  ch_controller.Move(moveVector * Time.deltaTime); //����� ������������ �� ������������     
    }

    //����� ����������
    /*
    private void GamingGravity()
    {
        if (!ch_controller.isGrounded)
        {
            gravityForce -= 100f * Time.deltaTime;

            // ch_animator.SetBool("Jump", false);
        }
        else gravityForce = -1f;
        if (Input.GetKeyDown(KeyCode.Space) && ch_controller.isGrounded)
        {
            // gameObject.GetComponent<Rigidbody>().AddForce(0, 3000,0);
            //ch_animator.SetBool("Jump", true);

            // gravityForce = jumpPower;
            //  ch_animator.SetTrigger("Jump");

            //   ch_animator.SetBool("Jump", false);
        }
    }
    */
    
}
