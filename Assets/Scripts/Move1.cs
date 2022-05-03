using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move1 : MonoBehaviour
{
    public float speedMove;
   // public float jumpPower;

    private float gravityForce; // ���������� ���������
    private Vector3 moveVector; // ����������� �������� ���������

    //������ �� ����������
    
    private CharacterController ch_controller;
    /*
    private Animator ch_animator;

    */

    private Rigidbody rig;

    private void Start()
    {
        
        ch_controller = GetComponent<CharacterController>();
        /*
        ch_animator = GetComponent<Animator>();
        */
        rig = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        CharacterMove();
      //  GamingGravity();

    }

    // ����� ����������� ���������
    private void CharacterMove()
    {
        //  if (ch_controller.isGrounded)
        //{
        //����������� �� �����������
        moveVector = Vector3.zero;
        moveVector.x = Input.GetAxis("Horizontal") * speedMove;
        moveVector.z = Input.GetAxis("Vertical") * speedMove;
        rig.velocity = moveVector;
        // Debug.Log(moveVector.x);
        // Debug.Log(moveVector.z);

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
        if (Vector3.Angle(Vector3.forward, moveVector) > 1f || Vector3.Angle(Vector3.forward, moveVector) == 0)
        {
            Vector3 direct = Vector3.RotateTowards(transform.forward, moveVector, speedMove*Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(direct);
        }

        // }
       
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
