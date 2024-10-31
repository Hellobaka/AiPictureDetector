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

            var r = Detector.Detect(path);
            if (r == null || !r.Success)
            {
                sendText.MsgToSend.Add("接口调用失败，查看日志排查问题");
                return result;
            }
            
            if (r.AIScore > r.NotAIScore)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyIsAI, r.AIScore.ToString("f2"), r.ModelName));
            }
            else
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyNotAI, r.NotAIScore.ToString("f2")));
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
