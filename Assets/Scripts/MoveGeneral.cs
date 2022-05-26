using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGeneral : MonoBehaviour
{
    public float speedMove;
    public float turnSpeed;
    // public float jumpPower;

    private float gravityForce; // ���������� ���������
    private Vector3 moveVector; // ����������� �������� ���������
    private Quaternion target;
    private bool isHalfLeft, isHalfRight;
    public bool isEasy = false;
    private Rigidbody rig;

    private void Awake()
    {
        if(PlayerPrefs.GetInt("MoveMode")==0) isEasy = true;
    }

    private void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isEasy) 
        {
            EasyMove();
        }
        else
        {
            HardMove();
        }
    }

    // ������ ����������� ���������
    private void HardMove()
    {
        rig.drag = 0f;
        //����������� �� �����������
        moveVector = Vector3.zero;
        moveVector.x = Input.GetAxis("Horizontal") * speedMove;
        moveVector.z = Input.GetAxis("Vertical") * speedMove;
        rig.velocity = moveVector;

        //������� ���������
        if (Vector3.Angle(Vector3.forward, moveVector) > 1f || Vector3.Angle(Vector3.forward, moveVector) == 0)
        {
            Vector3 direct = Vector3.RotateTowards(transform.forward, moveVector, speedMove * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(direct);
        }
        moveVector.y = gravityForce; // ������ ����������, ��������� ����� ��������!!!
    }
    private void EasyMove()
    {
        rig.drag = 10000f;
        if (Input.GetKey(KeyCode.W))
        {
            transform.localPosition += transform.forward * speedMove * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.localPosition += -transform.forward * speedMove * Time.deltaTime;
        }

        //��� �������� ��� ������������� ����������� ����� transform � moveVector
      //  moveVector.x = Input.GetAxis("Horizontal") * speedMove;
        //moveVector.z = Input.GetAxis("Vertical") * speedMove;

        //������� � ������� transform.Rotate
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, -turnSpeed*Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
        }

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
        */
        moveVector.y = gravityForce; // ������ ����������, ��������� ����� ��������!!! 
    }  
}
