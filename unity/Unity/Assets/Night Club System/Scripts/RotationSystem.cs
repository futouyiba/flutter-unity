using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSystem : MonoBehaviour
{
    private enum RotateAxis
    {
        X, Y, Z
    }

    private Vector3 curRotVector = Vector3.zero;
    private float vel = 0;

    [Header("Main Settings")]
    [Tooltip("Axis of rotation / Ось вращения")][SerializeField] private RotateAxis axis = RotateAxis.X;
    [Tooltip("Starting direction of rotation / Стартовое направление вращения")][SerializeField] private bool positiveDirection = true;
    [Tooltip("Rotate in local coordinates? / Вращать в локальных координатах?")][SerializeField] private bool local = false;
    [Tooltip("Rotation without limits / Вращение без ограничений")][SerializeField] private bool simpleRotation = false;
    [Tooltip("The initial angle of rotation of the object (enter manually) / Начальный угол поворота объекта(вписать вручную)")][SerializeField] private Vector3 startRotVector = Vector3.zero;
    [Tooltip("Rotation speed in unlimited mode / Скорость вращения в режиме без ограничения")][SerializeField] private float speedSimpleRotation = 30;
    [Tooltip("Smoothing Time / Время сглаживания")][SerializeField] private float smoothTime = 1;
    [Tooltip("Smoothing braking (the less the smoother) / Сглаживание торможения (чем меньше тем плавнее)")][SerializeField] private float smoothlyStop = 1;
    [Tooltip("Min rotation angle / Мин угол вращения")][SerializeField] private float minAngle = -180;
    [Tooltip("Max angle of rotation / Макс угол вращения")][SerializeField] private float maxAngle = 180;

    private void Awake() 
    {
        curRotVector = startRotVector;
    }

    private void Update() 
    {
        if(simpleRotation)
        {
            SimpleRotation();
        }
        else
        {
            Rotation();
        }
    }

    private void SimpleRotation()
    {
        if (local)
        {
            transform.Rotate(Time.fixedDeltaTime * new Vector3(axis == RotateAxis.X ? speedSimpleRotation : 0, axis == RotateAxis.Y ? speedSimpleRotation : 0, axis == RotateAxis.Z ? speedSimpleRotation : 0), Space.Self);
        }
        else
        {
            transform.Rotate(Time.fixedDeltaTime * new Vector3(axis == RotateAxis.X ? speedSimpleRotation : 0, axis == RotateAxis.Y ? speedSimpleRotation : 0, axis == RotateAxis.Z ? speedSimpleRotation : 0), Space.World);
        }
    }

    private void Rotation()
    {
        if(local)
        {
            switch(axis)
            {
                case RotateAxis.X:
                    curRotVector.x = Mathf.SmoothDamp(curRotVector.x, positiveDirection ? maxAngle + smoothlyStop : minAngle - smoothlyStop, ref vel, smoothTime);
                break;

                case RotateAxis.Y:
                    curRotVector.y = Mathf.SmoothDamp(curRotVector.y, positiveDirection ? maxAngle + smoothlyStop : minAngle - smoothlyStop, ref vel, smoothTime);
                break;

                case RotateAxis.Z:
                    curRotVector.z = Mathf.SmoothDamp(curRotVector.z, positiveDirection ? maxAngle + smoothlyStop : minAngle - smoothlyStop, ref vel, smoothTime);
                break;
            }

            transform.localEulerAngles = curRotVector;
        }
        else
        {
            switch(axis)
            {
                case RotateAxis.X:
                    curRotVector.x = Mathf.SmoothDamp(curRotVector.x, positiveDirection ? maxAngle + smoothlyStop : minAngle - smoothlyStop, ref vel, smoothTime);
                break;

                case RotateAxis.Y:
                    curRotVector.y = Mathf.SmoothDamp(curRotVector.y, positiveDirection ? maxAngle + smoothlyStop : minAngle - smoothlyStop, ref vel, smoothTime);
                break;

                case RotateAxis.Z:
                    curRotVector.y = Mathf.SmoothDamp(curRotVector.y, positiveDirection ? maxAngle + smoothlyStop : minAngle - smoothlyStop, ref vel, smoothTime);
                break;
            }

            transform.eulerAngles = curRotVector;
        }

        if(positiveDirection)
        {
            switch(axis)
            {
                case RotateAxis.X:
                    if(curRotVector.x >= maxAngle)
                    {
                        positiveDirection = false;
                    }
                break;

                case RotateAxis.Y:
                    if(curRotVector.y >= maxAngle)
                    {
                        positiveDirection = false;
                    }                
                break;

                case RotateAxis.Z:
                    if(curRotVector.z >= maxAngle)
                    {
                        positiveDirection = false;
                    }                
                break;
            }
        }
        else
        {
            switch(axis)
            {
                case RotateAxis.X:
                    if(curRotVector.x <= minAngle)
                    {
                        positiveDirection = true;
                    }
                break;

                case RotateAxis.Y:
                    if(curRotVector.y <= minAngle)
                    {
                        positiveDirection = true;
                    }                
                break;

                case RotateAxis.Z:
                    if(curRotVector.z <= minAngle)
                    {
                        positiveDirection = true;
                    }                
                break;
            }           
        }
    }
}