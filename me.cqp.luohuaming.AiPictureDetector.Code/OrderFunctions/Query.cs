using me.cqp.luohuaming.AiPictureDetector.PublicInfos;
using me.cqp.luohuaming.AiPictureDetector.Sdk.Cqp.EventArgs;
using PublicInfos;
using PublicInfos.Model;
using System.IO;
using System.Linq;

namespace me.cqp.luohuaming.AiPictureDetector.Code.OrderFunctions
{
    public class Query : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public int Priority { get; set; } = 1;

        public string GetCommand() => AppConfig.CommandMenu;

        public bool CanExecute(string destStr) => destStr.Contains("[CQ:image");

        public FunctionResult Execute(CQGroupMessageEventArgs e)
        {
            FunctionResult result = new FunctionResult
            {
                Result = true,
                SendFlag = true,
            };
            SendText sendText = new SendText
            {
                SendID = e.FromGroup,
            };
            result.SendObject.Add(sendText);
            var item = MainSave.DelayAPIs.FirstOrDefault(x => x.GroupID == e.FromGroup && x.QQID == e.FromQQ);
            if (item == null)
            {
                result.SendFlag = false;
                result.Result = false;
                return result;
            }
            MainSave.DelayAPIs.Remove(item);

            var image = e.Message.CQCodes.FirstOrDefault(x => x.IsImageCQCode);
            if (image == null)
            {
                sendText.MsgToSend.Add("发送的不是图片，调用失败");
                return result;
            }
            string path = Path.Combine(MainSave.ImageDirectory, "AIDetector");
            Directory.CreateDirectory(path);
            string img = e.CQApi.ReceiveImage(image);
            if (!File.Exists(img))
            {
                e.CQLog.Info("图片保存", $"{img} 路径无效，文件不存在");
                sendText.MsgToSend.Add("图片保存失败");
                return result;
            }
            path = Path.Combine(path, Path.GetFileName(img));
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.Move(img, path);

            var detector = Detector.Get(path);
            if (detector == null || detector.return_code != "0"
                || detector.response == null
                || detector.response.output == null
                || detector.response.output.Length == 0
                || detector.response.output.First().classes.Length == 0)
            {
                e.CQLog.Info("调用接口", $"code={detector.return_code} msg={detector.message}");
                sendText.MsgToSend.Add("接口调用失败，查看日志排查问题");
                return result;
            }
            var list = detector.response.output.First().classes.ToList();
            var ai = list.FirstOrDefault(x => x.name == "ai_generated");
            var notAI = list.FirstOrDefault(x => x.name == "not_ai_generated");
            if (ai == null || notAI == null)
            {
                e.CQLog.Info("调用接口", $"code={detector.return_code} msg={detector.message}");
                sendText.MsgToSend.Add("接口调用失败，查看日志排查问题");
                return result;
            }

            var aiScore = ai.score * 100;
            var notAIScore = notAI.score * 100;
            if (aiScore > notAIScore)
            {
                var highestModel = list.OrderByDescending(x => x.score).FirstOrDefault(x => x.name != "not_ai_generated" && x.name != "ai_generated");
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyIsAI, aiScore.ToString("f2"), highestModel.name));
            }
            else
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyNotAI, notAIScore.ToString("f2")));
            }
            return result;
        }

        public FunctionResult Execute(CQPrivateMessageEventArgs e)
        {
            FunctionResult result = new FunctionResult
            {
                Result = false,
                SendFlag = false,
            };
            return result;
        }
    }
}
