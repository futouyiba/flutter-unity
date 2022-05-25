using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ET.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace ET
{
    public class CharMgr : MonoBehaviour
    {
        [SerializeField]
        protected List<GameObject> charPrefabs;

        protected Dictionary<int, CharMain> charDict;

        private int _id = 0;
        protected int id
        {
            get
            {
                var returnid = _id;
                _id++;
                return returnid;
            }
        }

        private static CharMgr _instance;
        public static CharMgr instance
        {
            get
            {
                return _instance;
            }
            private set{}
        }

        protected int myId = -1;

        public Vector2 myposStored = Vector2.zero;


        void Awake()
        {
            Random.InitState((int)Time.time);
        }

        // Start is called before the first frame update
        void Start()
        {
            _instance = this;
            CursorLockMode lockMode = CursorLockMode.None;
            Cursor.lockState = lockMode;
            charDict = new Dictionary<int, CharMain>();
            // for (int i = 0; i < 10; i++)
            // {
            //     CreateCharView(this.id, DanceFloorHelper.GetRandomDanceFloorPos(), $"I am {i}", Color.white);
            // }
            
            //场景一开始就先发一个Mypos，然后还得存起来
            myposStored = DanceFloorHelper.GetRandomDanceFloorPos();
            var myPos = new MyPosition()
            {
                position = myposStored,
                ts= (int) new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds()
            };
            var msgMyPos = NativeProxy.MakeOp(myPos);
            NativeProxy.Unity2NativeMsg(msgMyPos);
            
            
        }

        // Update is called once per frame
        void Update()
        {
            #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.A))
            {
                Create100TestGuys();
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                var msg =
                    "{\"Op\":\"UserEnter\",\"OpData\":{\"userId\":840167, \"ts\":1652352802993, \"nickName\":\"哈哈\", \"sex\":1}}";
                NativeProxy.instance.Native2UnityMsg(msg);
                // RegisterMe(840167);
                // CreateCharView(this.id,DanceFloorHelper.GetRandomDanceFloorPos(), "hahaha", Color.white);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                EveryOneSpeak("嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿嘿");
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                var msg= "{\"Op\":\"UserMove\",\"OpData\":{\"userId\":840167, \"ts\":1652352835457\", \"position\":{\"x\":0.77,\"y\":0.85}}}";
                NativeProxy.instance.Native2UnityMsg(msg);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                var msg = "{\"Op\":\"UserExit\",\"OpData\":{\"uid\":2339817, \"ts\":1650960605116}}";
                NativeProxy.instance.Native2UnityMsg(msg);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                var msg= "{\"Op\":\"UserList\",\"OpData\":{\"userInfos\":[{\"birthday\":0,\"carId\":0,\"chatBubbleId\":0,\"city\":\"\",\"contractList\":[],\"creditLevel\":0,\"deleteUserId\":0,\"duration\":0,\"friendState\":0,\"gifType\":0,\"headPic\":\"\",\"headgearId\":0,\"inRoom\":false,\"intro\":\"\",\"isInviteMic\":false,\"lastActiveTime\":1652351558146,\"levelList\":[],\"messageBanTime\":0,\"micId\":0,\"newUser\":true,\"nickName\":\"black\",\"nickPendantId\":0,\"online\":true,\"onlineHidden\":false,\"position\":\"0.55245835,0.38888156\",\"privateChatBanTime\":0,\"sex\":2,\"surfing\":530510,\"userBanTime\":0,\"userDesc\":\"\",\"userId\":840174,\"userState\":1,\"userType\":2,\"voiceTime\":0},{\"birthday\":696268800000,\"carId\":0,\"chatBubbleId\":0,\"city\":\"北京\",\"contractList\":[],\"creditLevel\":0,\"deleteUserId\":0,\"duration\":0,\"friendState\":0,\"gifType\":0,\"headPic\":\"user/ed03f571b0ae48f590aa6bb4fe4daad4.jpg\",\"headgearId\":0,\"inRoom\":false,\"intro\":\"\",\"isInviteMic\":false,\"lastActiveTime\":1652351792150,\"levelList\":[],\"messageBanTime\":0,\"micId\":0,\"newUser\":true,\"nickName\":\"哈哈\",\"nickPendantId\":0,\"online\":true,\"onlineHidden\":false,\"position\":\"0.547373175621033,0.463172942399979\",\"privateChatBanTime\":0,\"sex\":1,\"surfing\":530501,\"userBanTime\":0,\"userDesc\":\"\",\"userId\":840167,\"userState\":1,\"userType\":2,\"voiceTime\":0},{\"birthday\":0,\"carId\":0,\"chatBubbleId\":0,\"city\":\"\",\"contractList\":[],\"creditLevel\":0,\"deleteUserId\":0,\"duration\":0,\"friendState\":0,\"gifType\":0,\"headPic\":\"\",\"headgearId\":0,\"inRoom\":false,\"intro\":\"\",\"isInviteMic\":false,\"lastActiveTime\":1652351922902,\"levelList\":[],\"messageBanTime\":0,\"micId\":0,\"newUser\":true,\"nickName\":\"黑色小老虎\",\"nickPendantId\":0,\"online\":true,\"onlineHidden\":false,\"position\":\"1,1\",\"privateChatBanTime\":0,\"sex\":1,\"surfing\":530503,\"userBanTime\":0,\"userDesc\":\"\",\"userId\":840169,\"userState\":1,\"userType\":2,\"voiceTime\":0}]}}";
                NativeProxy.instance.Native2UnityMsg(msg);
            }
            #endif
        }

        public void Create100TestGuys()
        {
            var random_me = -1;
            for (int i = 0; i < 100; i++)
            {
                var char_id = this.id;
                if (char_id == 1)
                {
                    random_me = Random.Range(1, 100);
                    Debug.LogWarning($"my id is {random_me}");
                }

                var view = CreateCharView(char_id, DanceFloorHelper.GetRandomDanceFloorPos(), $"i am {char_id}", -1,
                    Color.white);
                if (char_id == random_me)
                {
                    RegisterMe(char_id);
                }
            }
        }
        
        public void CreateCharNativeCall(string _params)
        {
            CreateCharView(id, DanceFloorHelper.GetRandomDanceFloorPos(), $"I am {_params}",-1, Color.white);
            //todo send the random pos to native app
            
        }

        /// <summary>
        /// 在创建后指出哪个是我
        /// </summary>
        /// <param name="id"></param>
        public void RegisterMe(int id)
        {
            var charMain = GetCharacter(id);
            if (charMain)
            {
                myId = id;
                charMain.isMe = true;
                charMain.SetNameColor(Color.yellow);
                CameraBolt.TriggerEvent("Idle2Follow");
                CameraBolt.TriggerEvent("Follow2Idle");
            }
            else
            {
                Debug.LogError($"charmain for me {id} not found!");
            }
            
        }

        public CharMain GetMe()
        {
            if (myId < 0)
            {
                Debug.LogWarning($"my id is {myId}");
                return null;
            }
            var charmain = GetCharacter(myId);
            if (!charmain)
            {
                Debug.LogError($"charmain for me:{myId} does not exist");
                return null;
            }

            return charmain;
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">unified pos</param>
        /// <param name="name">char name</param>
        /// <param name="name_color"> char name color</param>
        /// <returns></returns>
        public GameObject CreateCharView(int id, Vector2 position, string name,int appearance_id , Color name_color)
        {
            
            if (appearance_id > charPrefabs.Count - 1 || appearance_id < 0)
            {
                Debug.Log($"appearance {appearance_id} does not exist, using random");
                appearance_id = Random.Range(0, charPrefabs.Count - 1);
            }
            var to_create = this.charPrefabs[appearance_id];
            var goCreated = GameObject.Instantiate(to_create);
            var charView = goCreated.GetComponent<CharMain>();
            charView.SetName(name);
            charView.SetNameColor(name_color);
            charView.SetNameColor(name_color);
            if (position.x < -100f && position.y < -100f)
            {//未初始化
                charView.SetVisible(false);
                var truePos = DanceFloorHelper.PosUnified2Scene(new Vector2(-1f, -1f));
                goCreated.transform.position = new Vector3(truePos.x, DanceFloorHelper.GetPivotY(), truePos.y);
            }
            else
            {
                var truePos = DanceFloorHelper.PosUnified2Scene(position);
                goCreated.transform.position = new Vector3(truePos.x, DanceFloorHelper.GetPivotY(), truePos.y);
            }

            
            charDict.Add(id, charView);
            
            return goCreated;

        }


        public void RemoveCharView(int id)
        {
            var charView = charDict[id];
            if (charView != null)
            {
                charDict.Remove(id);
                charView.CharLeave();
            }
        }

        public static KeyValuePair<int,CharMain>? GetRandomChar()
        {
            if (instance.charDict.Count <= 0) return null;
            var rand = Random.Range(0, instance.charDict.Count);
            return instance.charDict.ElementAt(rand);
        }

        public CharMain GetCharacter(int uid)
        {
            var succeed = this.charDict.TryGetValue(uid, out CharMain result);
            if (!succeed)
            {
                Debug.LogWarning($"no user found for {uid}");
                return null;
            }

            return result;
        }

        public void EveryOneSpeak(string text)
        {
            foreach (var charkv in charDict)
            {
                charkv.Value.Speak(text);
            }
        }


    }
}
