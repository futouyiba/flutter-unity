using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Utility
{

    public class JsonMessage
    {
 
    }

    public class JsonCmd
    {
        
    }

    public class Operation : JsonMessage
    {
        public string Op;
        // public string OpData;
    }

    public class Operation<T> : JsonMessage where T:JsonCmd
    {
        public string Op;
        public T OpData;

        public Operation(T data)
        {
            OpData = data;
        }

    }

    public struct Vec2
    {
        public double x;
        public double y;

        public Vec2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class UserInfo : JsonCmd
    {
        public const int MALE = 1;
        public const int FEMALE = 2;

        public int userId;
        public int surfing;
        public string nickName = "";
        public string headPic = "";
        public string intro = "";
        public int sex; //1男，2女
        public long birthday;
        public int micId;
        public int headgearId;
        public int carId;
        public int chatBubbleId;
        public int nickPendantId;
        public int userState;
        public int friendState;
        public int deleteUserId;


        public string userDesc = "";

//禁言时长
        public long privateChatBanTime;
//房间禁言时长
        public long messageBanTime;
// 当前使用的个性进房提示信息
        public string currentIntoVoiceTips;
//陪伴时长
        public long voiceTime;
//封禁时长
        public long userBanTime;

        public int creditLevel;
        public string color;
        public int gifType;  //1v1聊天，GIF类型，1破冰GIF，2表情包，3话题卡
//1v1聊天，音频时长
        public int duration;
        public int userType;
        public string city;
        public long lastActiveTime; //上次活跃时间
        public bool online;   //是否在线
        public bool onlineHidden; //是否隐身
        public bool newUser;   //是否新注册用户
        // private List<UserLevelBean> levelList;
        // private List<CacheUserContractInfo> contractList;
        public bool inRoom;//当前是否在房间
        public bool isInviteMic;//是否已经邀请过上麦
        // private RoomContractInfo contractInfo;//当前显示的契约
        public Vector2 position;
        public int appearance;
    }

    public class UserMove : JsonCmd
    {
        public int userId;
        public long ts;
        public Vector2 position;
    }

    public class UserEnter : JsonCmd
    {
        public int userId;
        public long ts;
        public string nickName;
        public int sex;
        public Vector2 position;
        public int appearance;
    }

    public class MeEnter : JsonCmd
    {
        public int userId;
        public long ts;
        public string nickName;
        public int sex;
        public int appearance;
    }

    public class UserExit : JsonCmd
    {
        public int userId;
        public long ts;
    }

    public class UserList : JsonCmd
    {
        public List<UserInfo> userInfos;

    }

    // public List<int> uids;
    // public List<string> unames;
    // public List<Vector2> positions;
    // public List<int> appearances;
    
    public class MeMove : JsonCmd
    {
        public long ts;
        public Vector2 position;
    }

    public class MyPosition : JsonCmd
    {
        public long ts;
        public Vector2 position;
    }


    public class UserMsg : JsonCmd
    {
        public int userId;
        public long ts;
        public string text;
    }

    public class MeTap : JsonCmd
    {
        
    }
    
    
    

    
}