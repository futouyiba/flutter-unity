using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bolt;
using DG.Tweening;
using ET.Utility;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

namespace ET
{
    
    public class CharMain : MonoBehaviour
    {
        // protected bool IsMoving;
        protected Vector3 moveTarget = Vector3.positiveInfinity;
        
        [SerializeField]
        protected TextMeshPro nameTmp;

        [SerializeField] protected float moveSpeed;
        [SerializeField] private TextBubble bubble;
        [SerializeField] private StateMachine fsm;

        [SerializeField]
        public float bubbleTime;
        
        public bool isMe = false;
        public bool isMoving = false;
        public bool isTalking = false;
        public float talkTime = 0f;
        public float mvIdleTime = 0f;

        public float mvStayTime = 0f;

        protected float direction = 1;
        private float oriScaleX;
        // Start is called before the first frame update
        void Start()
        {
            oriScaleX = transform.GetChild(0).localScale.x;
            // Move(DanceFloorHelper.GetRandomDanceFloorPos());
        }

        // Update is called once per frame
        void Update()
        {
            // if (!this.IsMoving)
            // {
            //     Move(DanceFloorHelper.GetRandomDanceFloorPos());
            // }
        }

        private Sequence cur_seq;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">归一化的位置</param>
        public void Move(Vector2 target)
        {
            // var vars = Variables.Object(this.gameObject);
            // var isMoving=vars.Get("IsMoving");
            // if ((bool)isMoving)
            // {
            //     //kill dotween and send event
            //     DOTween.KillAll(this.gameObject);
            //     fsm.TriggerUnityEvent("MoveInterrupt");
            //     // Debug.LogError("char moving, will not exec");
            //     // return;
            // }
            
            //20200513:不管在哪个状态都触发StartMove，在不同地方进行不同响应
            fsm.TriggerUnityEvent("StartMove");
            var scenePos = DanceFloorHelper.PosUnified2Scene(target);
            var targetPos = new Vector3(scenePos.x, transform.position.y, scenePos.y);
            // this.IsMoving = true;

            // CameraBolt.TriggerEvent("FollowRand");
            this.moveTarget = targetPos;
            var distance = Vector3.Distance(this.transform.position, this.moveTarget);
            var duration = distance / moveSpeed;
            //转身
            var spriteChild = this.transform.GetChild(0);
            if (scenePos.x>= this.transform.position.x)
            {
                if (direction<0)
                {
                    direction = 1f;
                    spriteChild.DOScaleX(direction*oriScaleX, 0.3f);
                }
            }
            else
            {
                if (direction > 0)
                {
                    direction = -1f;
                    spriteChild.DOScaleX(direction * oriScaleX, 0.3f);
                }
            }

            cur_seq = DOTween.Sequence();
            cur_seq.Append(this.transform.DOMove(targetPos, duration).OnComplete(MoveEnd));
            cur_seq.Play();
            // Debug.Log($"going to {target}");
            if(isMe) NativeProxy.SendMeMove(target);
        }

        public void MoveEnd()
        {
            fsm.TriggerUnityEvent("MoveEnded");
            if(isMe) CameraBolt.TriggerEvent("Follow2Idle");
            // this.IsMoving = false;
            this.moveTarget = Vector3.positiveInfinity;
            cur_seq = null;
            //
            // var task = Task.Run(()=>Wait(Random.Range(3, 8)));
            // yield return new WaitUntil(()=>task.IsCompleted);
            // StartCoroutine(Wait(Random.Range(3, 8)));
            // Move(DanceFloorHelper.GetRandomDanceFloorPos());

        }

        public void KillCurSeq()
        {
            if (cur_seq==null)
            {
                Debug.LogError("current seq does not exist!");
                return;
            }
            cur_seq.Kill();
            cur_seq = null;
        }

        public void MoveRandom()
        {
            Move(DanceFloorHelper.GetRandomDanceFloorPos());
        }

        protected static IEnumerator Wait(int time)
        {
            Debug.Log($"{Time.time} starting wait for {time} secs");
            yield return new WaitForSeconds(time);
            Debug.Log($"{Time.time} ended wait for {time} secs");
        }
        
        public void SetName(string Name)
        {
            nameTmp.SetText(Name);
        }

        public void SetNameColor(Color color)
        {
            nameTmp.color = color;
        }

        public void CharLeave()
        {
            GameObject.Destroy(this.gameObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        public void Speak(string content)
        {
            bubble.Speak(content);
            fsm.TriggerUnityEvent("StartTalking");
        }

        public void SpeakEnd()
        {
            bubble.HideBubble();
            fsm.TriggerUnityEvent("FinishedTalking");
        }

        public void FollowMe()
        {
            if(!isMe) return;
            var cameraBolt = Camera.main.GetComponent<CameraBolt>();
            CameraBolt.TriggerEvent("Idle2Follow");
        }

        public bool TalkTimeCheck()
        {
            talkTime += Time.deltaTime;
            return talkTime >= this.bubbleTime;
        }

        public bool MoveIdleTimeCheck()
        {
            mvIdleTime += Time.deltaTime;
            return mvIdleTime >= this.mvStayTime;
        }
        
        public int BoltUnityEvAfterTime(int afterTime, string evStr)
        {
            return TimeMgr.instance.AddTimer(afterTime, () => fsm.TriggerUnityEvent(evStr));
        }
        
        public void BoltCancelTimer(int timerId)
        {
            TimeMgr.instance.RemoveTimer(timerId);
        }

        public void Teleport(Vector2 position)
        {
            this.transform.position = DanceFloorHelper.PosUnified2Scene(position);
        }

        [SerializeField] protected GameObject sprite;
        public void SetVisible(bool isVisable)
        {
            this.sprite.SetActive(isVisable);
        }

        public bool IsVisible
        {
            get
            {
                return sprite.activeSelf;
            }
        }
    }
}
