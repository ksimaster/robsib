using System;
using UnityEngine;

public class MoveGeneral : MonoBehaviour
{
    // public float jumpPower;
    // private float gravityForce = -10; // гравитация персонажа
    // private Vector3 moveVector; // направление движения персонажа
    // private Quaternion target;
    // private bool isHalfLeft, isHalfRight;
    public bool isEasy = false;
    private Rigidbody rig;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(PlayerConstants.MoveMode))
        {
            isEasy = PlayerPrefs.GetInt(PlayerConstants.MoveMode) == 1;
        }
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

    // методы перемещения персонажа
    private void HardMove()
    {
        //перемещение по поверхности
        var moveVector = Vector3.zero;
        moveVector.x = Input.GetAxis("Horizontal") * PlayerConstants.speedMove;
        moveVector.z = Input.GetAxis("Vertical") * PlayerConstants.speedMove;
        if (moveVector.x == 0 && moveVector.z == 0)
        {
            return;
        }

        var objectX = transform.forward.x;
        var objectZ = transform.forward.z;
        var correlation = (moveVector.x * objectX + moveVector.z * objectZ) / Math.Sqrt(moveVector.x * moveVector.x + moveVector.z * moveVector.z + 1e-6) / Math.Sqrt(objectX * objectX + objectZ * objectZ + 1e-6);
        //поворот персонажа

        Vector3 direct = Vector3.RotateTowards(transform.forward, moveVector, 2 * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(direct);
        if (correlation < 0.3f)
        {
            moveVector = moveVector * 0;
        }
        else if(correlation < 0.6f)
        {
            moveVector = moveVector * (float)((correlation - 0.3f) / 0.3f);
        }

        moveVector.y = -rig.drag;
        rig.velocity = moveVector;
    }

    private void EasyMove()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.localPosition += transform.forward * PlayerConstants.speedMove * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.localPosition += -transform.forward * PlayerConstants.speedMove * Time.deltaTime;
        }

        //для поворота при использовании перемещения через transform с moveVector
        //  moveVector.x = Input.GetAxis("Horizontal") * speedMove;
        //moveVector.z = Input.GetAxis("Vertical") * speedMove;

        //поворот с помощью transform.Rotate
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, -PlayerConstants.turnSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, PlayerConstants.turnSpeed * Time.deltaTime);
        }
        #region [Наработки передвижений и поворотов]
        //поворот персонажа
        /*
        if (Vector3.Angle(Vector3.forward, moveVector) > 1f || Vector3.Angle(Vector3.forward, moveVector) == 0)
        {
            Vector3 direct = Vector3.RotateTowards(transform.forward, moveVector, speedMove*Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(direct);
        }
        */
        //способ поворота на moveVector и rigidbody
        /*
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) 
        {
            Vector3 direct = Vector3.RotateTowards(transform.forward, moveVector, speedMove * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(direct);
            moveVector = Vector3.zero;
        }
        */
        //способ поворота на transform, но с moveVector
        /*
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveVector), speedMove * Time.deltaTime);
            Debug.Log(transform.rotation);
        }
       */
        // Поворот через углы Эйлера недоделан
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
        #endregion [Наработки передвижений и поворотов]
    }
}
