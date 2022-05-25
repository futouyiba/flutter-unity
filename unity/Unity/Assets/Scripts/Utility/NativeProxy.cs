using System;
using System.CodeDom;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using System.Runtime.InteropServices;
using Random = UnityEngine.Random;

namespace ET.Utility
{

    public class NativeProxy
    {

#if UNITY_IPHONE && !UNITY_EDITOR

[DllImport("__Internal")]
private static extern void Unity2NativeMsgIOS(string opJson);

#endif


        private static NativeProxy _instance;
        public static NativeProxy instance
        {
            get { return _instance ??= new NativeProxy(); }
            private set{}
        }

        /// <summary>
        /// 接到消息的处理函数
        /// </summary>
        /// <param name="msg"></param>
        public void Native2UnityMsg(string msg)
        {
            Debug.Log($"native2unity:{msg}");
            var cmd = GetOp(msg);
            var cmdType = MsgCode2Type(cmd);
            switch (cmd)
            {
                case "UserEnter":
                    var userEnter = GetOpdata<UserEnter>(msg);
                    Random.InitState((int)userEnter.userId);
                    CharMgr.instance.CreateCharView(userEnter.userId, userEnter.position, userEnter.nickName,
                        userEnter.appearance, Color.white);
                    break;
                case "MeEnter":
                    var meEnter = GetOpdata<MeEnter>(msg);
                    var pos = CharMgr.instance.myposStored;
                    Random.InitState((int)meEnter.userId);
                    CharMgr.instance.CreateCharView(meEnter.userId, pos, meEnter.nickName, meEnter.appearance,
                        Color.white);
                    CharMgr.instance.RegisterMe(meEnter.userId);
                    //2022.5.11顺序改了，一进场景CharMgr就发MyPos，然后等MeEnter
                    //MeEnter时才创建我自己

                    break;
                case "UserExit":
                    var userExit = GetOpdata<UserExit>(msg);
                    CharMgr.instance.RemoveCharView(userExit.userId);
                    break;
                case "UserList":
                    var userList = GetOpdata<UserList>(msg);
                    var userInfos = userList.userInfos;
                    // Random.InitState(userList.);
                    foreach (var userInfo in userInfos)
                    {
                        var res = CharMgr.instance.GetCharacter(userInfo.userId);
                        if (res == null)
                        {
                            Random.InitState(userInfo.userId);
                            CharMgr.instance.CreateCharView(userInfo.userId, userInfo.position, userInfo.nickName,userInfo.appearance, Color.white);
                        }
                        else
                        {
                            res.Move(userInfo.position);
                        }
                    }
                    break;
                case "UserMove":
                    var userMove = GetOpdata<UserMove>(msg);
                    var chara = CharMgr.instance.GetCharacter(userMove.userId);
                    if (chara != null)
                    {
                        if (!chara.IsVisible)
                        {
                            chara.Teleport(userMove.position);
                            chara.SetVisible(true);
                            CameraBolt.TriggerEvent("Idle2Follow");
                        }
                        else chara.Move(userMove.position);
                    }
                    else
                    {
                        Debug.LogError($"char {userMove.userId} not exist");    
                    }
                    break;
                case "UserMsg":
                    var userMsg = GetOpdata<UserMsg>(msg);
                    var uid = userMsg.userId;
                    var charMain = CharMgr.instance.GetCharacter(uid);
                    charMain.Speak(userMsg.text);
                    break;


            }
            

        }

        /// <summary>
        /// 发送json message去native
        /// </summary>
        /// <param name="json"></param>
        public static void Unity2NativeMsg(string json)
        {
            Debug.Log($"unity2native:{json}");
            #if UNITY_ANDROID && !UNITY_EDITOR

            // AndroidJavaClass jc = new AndroidJavaClass("com.bjzy.showdog.voiceroom.unity.UnityCall");
            // var jo = jc.GetStatic<AndroidJavaObject>("unityCall");
            // jo.Call("test",json);
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            jo.Call("Native2UnityMsg", json);
            #endif

            #if UNITY_IOS && !UNITY_EDITOR
			Unity2NativeMsgIOS(json);
			#endif
        }
        
        
        
        public static void SendMyPos(Vector2 pos)
        {
            var ts = (int) new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            var mypos = new MyPosition
            {
                ts = ts,
                position = pos
            };
            var msg = MakeOp(mypos);
            Unity2NativeMsg(msg);
        }

        public static void SendMeMove(Vector2 pos)
        {
            var ts = (int) new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            var target = new MeMove()
            {
                ts = ts,
                position = pos,
            };
            var msg = MakeOp(target);
            Unity2NativeMsg(msg);
        }

        // public static T Extract<T>(string json) where T : JsonMessage
        // {
        //     return JsonMapper.ToObject<T>(json);
        // }
        //
        // public static string Pack(JsonMessage message)
        // {
        //     return JsonMapper.ToJson(message);
        // }

        // public static JsonData GetOpdata(string json)
        // {
        //     var data = JsonMapper.ToObject(json);
        //     return data["OpData"];
        // }

        public static string GetOp(string json)
        {
            var data = JsonMapper.ToObject(json);
            return data["Op"].ToString();
        }

        public static T GetOpdata<T>(string json) where T:JsonCmd
        {
            var data = JsonMapper.ToObject(json);
            var opData = data["OpData"];
            return JsonMapper.ToObject<T>(opData.ToJson());
        }


        public static Dictionary<string, Type> cmdDict = new Dictionary<string, Type>()
        {
            {"UserEnter",typeof(UserEnter)},
            {"MeEnter",typeof(MeEnter)},
            {"UserExit",typeof(UserExit)},
            {"UserList",typeof(UserList)},
            {"UserMove",typeof(UserMove)},
            {"UserMsg",typeof(UserMsg)},
            {"MeMove",typeof(MeMove)},
            {"MeTap",typeof(MeTap)},
            {"MyPos",typeof(MyPosition)}
        };
        
        public static Type MsgCode2Type(string code)
        {
            var succeed = cmdDict.TryGetValue(code, out Type type);
            if (!succeed)
            {
                Debug.LogError($"failed coverting code for {code}");
                return null;
            }

            return type;
        }

        public static string MsgType2Code(Type type)
        {
            foreach (var kvpair in cmdDict)
            {
                if (kvpair.Value == type)
                {
                    return kvpair.Key;
                }
            }

            Debug.LogError($"failed converting type for {type.ToString()}");
            return null;
        }

        /// <summary>
        /// 构建Op消息
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string MakeOp<T>(T obj) where T : JsonCmd
        {
            var code = MsgType2Code(typeof(T));
            Operation<T> op = new Operation<T>(obj)
            {
                Op = code
            };
            var data = JsonMapper.ToJson(op);
            //todo: get rid of "_t"'s
            return data;
        }
        
        
        
        
    }
}