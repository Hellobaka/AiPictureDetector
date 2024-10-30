using me.cqp.luohuaming.AiPictureDetector.PublicInfos;
using me.cqp.luohuaming.AiPictureDetector.Sdk.Cqp.EventArgs;
using PublicInfos.Model;

namespace me.cqp.luohuaming.AiPictureDetector.Code.OrderFunctions
{
    public class Start : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public int Priority { get; set; } = 10;
        
        public string GetCommand() => AppConfig.CommandMenu;

        public bool CanExecute(string destStr) => destStr.Replace("＃", "#").StartsWith(GetCommand());

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

            MainSave.DelayAPIs.Add(new DelayAPI(e.FromGroup.Id, e.FromQQ.Id));
            sendText.MsgToSend.Add("请在接下来的一条消息内发送需要搜索的图片");
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
