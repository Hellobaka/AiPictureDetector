using PublicInfos.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace me.cqp.luohuaming.AiPictureDetector.PublicInfos
{
    public class AppConfig : ConfigBase
    {
        public AppConfig(string path)
            : base(path)
        {
            LoadConfig();
            Instance = this;
        }

        public static AppConfig Instance { get; private set; }

        public static string CommandMenu { get; set; } = "";

        public static string ReplyIsAI { get; set; } = "";

        public static string ReplyNotAI { get; set; } = "";

        public static string SightEngine_UserID { get; set; } = "";

        public static string SightEngine_UserSecret { get; set; } = "";

        public static DetectorType DetectorType { get; set; } = DetectorType.HiveAI;

        public override void LoadConfig()
        {
            CommandMenu = GetConfig("CommandMenu", "#鉴癌");
            ReplyIsAI = GetConfig("ReplyIsAI", "像啊，很像啊\n是 AI 的概率为 {0}%，可能的来源模型是 {1}");
            ReplyNotAI = GetConfig("ReplyNotAI", "不太像\n不是 AI 的概率为 {0}%");
            SightEngine_UserID = GetConfig("SightEngine_UserID", "");
            SightEngine_UserSecret = GetConfig("SightEngine_UserSecret", "");
            DetectorType = GetConfig("DetectorType", DetectorType.HiveAI);
        }
    }
}