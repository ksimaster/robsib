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
    private int cntr = 0;
    private Vector3 prevPosition;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(PlayerConstants.MoveMode))
        {
            isEasy = PlayerPrefs.GetInt(PlayerConstants.MoveMode) == 0;
        }

        if (PlayerPrefs.HasKey(PlayerConstants.PositionX) && PlayerPrefs.HasKey(PlayerConstants.PositionY) &&
            PlayerPrefs.HasKey(PlayerConstants.PositionZ))
        {
            var x = PlayerPrefs.GetFloat(PlayerConstants.PositionX);
            var y = PlayerPrefs.GetFloat(PlayerConstants.PositionY);
            var z = PlayerPrefs.GetFloat(PlayerConstants.PositionZ);
            this.transform.SetPositionAndRotation(new Vector3(x, y, z), this.gameObject.transform.rotation);
        }
    }

    private void Start()
    {
        rig = GetComponent<Rigidbody>();
        prevPosition = transform.localPosition + transform.localPosition * 0;
    }

    private void FixedUpdate()
    {
        SaveProgress();
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
        if (correlation < 0.5f)
        {
            moveVector = moveVector * 0;
        }
        else if(correlation < 0.8f)
        {
            moveVector = moveVector * (float)((correlation - 0.5f) / 0.3f);
        }

        moveVector.y = -rig.drag;
        rig.velocity = moveVector;
    }

    private void EasyMove()
    {
        var move = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            move = transform.forward * PlayerConstants.speedMove * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            move = -transform.forward * PlayerConstants.speedMove * Time.deltaTime;
        }
        
        var prevMove = transform.localPosition - prevPosition;
        var horizontalMove = Math.Sqrt(prevMove.x * prevMove.x + prevMove.z * prevMove.z);
        var verticalMove = prevMove.y;
        if (verticalMove/ horizontalMove > 2 && verticalMove > 0.03) {
            var backWay = prevPosition - transform.localPosition;
            var margin = 0f;
            var correlation = (move.x * backWay.x + move.z * backWay.z) / Math.Sqrt(move.x * move.x + move.z * move.z + 1e-6) / Math.Sqrt(backWay.x * backWay.x + backWay.z * backWay.z + 1e-6);
            correlation += margin;
            correlation = (Math.Abs(correlation) + correlation) / 2; // Обнуляем все что меньше -0.2
            correlation = Math.Sqrt(correlation / (1 + margin));
            move = move * (float)correlation;
        } 
        else
        {
            prevPosition = transform.localPosition + transform.localPosition * 0;
        }

        transform.localPosition += move;
        // для поворота при использовании перемещения через transform с moveVector
        // moveVector.x = Input.GetAxis("Horizontal") * speedMove;
        // moveVector.z = Input.GetAxis("Vertical") * speedMove;

        //поворот с помощью transform.Rotate
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.up, -PlayerConstants.turnSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.up, PlayerConstants.turnSpeed * Time.deltaTime);
        }

        // transform.localPosition.Set(transform.localPosition.x, transform.localPosition.y - rig.drag, transform.localPosition.z);
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

    private void SaveProgress()
    {
        cntr = (++cntr) % 100;
        if (cntr == 1)
        {
            var position = this.transform.position;
            PlayerPrefs.SetFloat(PlayerConstants.PositionX, position.x);
            PlayerPrefs.SetFloat(PlayerConstants.PositionY, position.y);
            PlayerPrefs.SetFloat(PlayerConstants.PositionZ, position.z);
        }
    }
}
