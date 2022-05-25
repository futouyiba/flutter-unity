using System.Collections;
using System.Collections.Generic;
using Bolt;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace ET
{
    public class CameraBoltSF : MonoBehaviour
    {
        [SerializeField] private StateMachine fsm;
        // Start is called before the first frame update
        protected GameObject GoFollowing;
        public bool IsFollowing = false;
        private static readonly Vector3 lookAtCamOffset = new Vector3(0f, 3.1f, 4.36f);
        private static readonly float lookAtCamRotX = 16f;
        private Quaternion initRot;
        private Vector3 initPos;
        private Vector3 farWatchPos;
        private int fakeid = -1;
        [SerializeField]
        public Animator animator;
        CharMain myCharMain;

        private Quaternion followRotQueternion;
        private Transform myCharTransform;
        private float lerpValue;
        private Transform cameraTransform;
        private Vector3 followEuler;
        [SerializeField] private float enterIdleLerpDuration;
        [SerializeField] private float enterFollowLerpDuration;
        private TweenerCore<float,float,FloatOptions> lerpValueTween;
        [SerializeField] private float enterFarWatchLerpDuration;
        public bool IsFarWatching = false;

        public void Init()
        {
            // animator = GetComponent<Animator>();
            CameraBolt.cameraBoltSF = this;
            // fsm = GetComponent<StateMachine>();
            cameraTransform = this.transform;
            this.initPos = cameraTransform.position;
            this.initRot = cameraTransform.rotation;
            followEuler = initRot.eulerAngles + new Vector3(lookAtCamRotX, 0, 0);
            followRotQueternion = Quaternion.Euler(followEuler);
        }

        // Update is called once per frame
        void Update()
        {
            // CheckKeyForFsmMsg();

            if (IsFollowing)
            {
                //following logic
                var position = cameraTransform.position;
                position = Vector3.Lerp(position,
                    myCharTransform.position + lookAtCamOffset, lerpValue);
                cameraTransform.position = position;
                Debug.Log($"cameraTransform pos is {cameraTransform.position}, myCharTransform position is {myCharTransform.position}, lookAtCamOffset is {lookAtCamOffset}, lerpValue is {lerpValue}, cameraTransform.position is {position}");
            }

            if (IsFarWatching)
            {
                cameraTransform.position = Vector3.Lerp(cameraTransform.position,
                    farWatchPos, lerpValue);
            }


        }

        private void CheckKeyForFsmMsg()
        {
            if (Input.GetKey(KeyCode.V))
            {
                Debug.Log("keypad1 is pressed");
                fsm.TriggerUnityEvent("Idle2Follow");
            }

            if (Input.GetKey(KeyCode.Keypad4))
            {
                fsm.TriggerUnityEvent("Lerp2Idle");
            }

            if (Input.GetKey(KeyCode.Keypad2))
            {
                fsm.TriggerUnityEvent("Lerp2Follow");
            }

            if (Input.GetKey(KeyCode.Keypad3))
            {
                fsm.TriggerUnityEvent("Follow2Lerp");
            }
        }


        private void ResetTweens()
        {
            if (lerpValueTween is { active: true })
            {
                Debug.Log("now lerp tween is active, kill it...");
                lerpValueTween.Kill();
            }

            DOTween.Kill(cameraTransform);
        }
        
        public void EnterFollow()
        {
            ResetTweens();
            IsFollowing = true;
            if (myCharMain ==null)
            {
                Debug.LogWarning("myCharMain is null, init it");
                myCharMain = CharMgr.instance.GetMe();
                myCharTransform = myCharMain.transform;
            }

            GoFollowing = myCharMain.gameObject;// for now
            // enterFollowLerpDuration = 1.5f;
            lerpValue = 0.0f;
            this.lerpValueTween = DOTween.To(() => lerpValue, x => lerpValue = x, 1f, enterFollowLerpDuration);
            cameraTransform.DORotate(followEuler, enterFollowLerpDuration);
        }


        public void EnterIdle()
        {
            ResetTweens();
            IsFollowing = false;
            // enterIdleLerpDuration = 1.5f;
            lerpValue = 0f;
            this.lerpValueTween = DOTween.To(() => lerpValue, x => lerpValue = x, 1f, enterIdleLerpDuration);
            cameraTransform.DORotate(initRot.eulerAngles, enterIdleLerpDuration);
            cameraTransform.DOMove(initPos, enterIdleLerpDuration);
        }
        
        public void EnterFarWatch()
        {
            ResetTweens();
            IsFarWatching = true;
            lerpValue = 0f;
            this.farWatchPos = initPos;
            this.farWatchPos.x = GoFollowing.transform.position.x;
            this.lerpValueTween = DOTween.To(() => lerpValue, x => lerpValue = x, 1f, enterFarWatchLerpDuration);
            cameraTransform.DORotate(initRot.eulerAngles, enterFarWatchLerpDuration);
            
        }
        
        public void SwayAnimEnd()
        {
            fsm.TriggerUnityEvent("Sway2Idle");
        }
        
        public void SwayAnimStart()
        {
            DOTween.Kill(cameraTransform);
        }

        public void TriggerEvent(string ev)
        {
            fsm.TriggerUnityEvent(ev);
        }

    }
}
