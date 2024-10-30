using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using me.cqp.luohuaming.AiPictureDetector.Sdk.Cqp.EventArgs;
using me.cqp.luohuaming.AiPictureDetector.PublicInfos;

namespace me.cqp.luohuaming.AiPictureDetector.Code
{
    public class Event_GroupMessage
    {
        public static FunctionResult GroupMessage(CQGroupMessageEventArgs e)
        {
            FunctionResult result = new FunctionResult()
            {
                SendFlag = false
            };
            try
            {
                foreach (var item in MainSave.Instances.OrderByDescending(x => x.Priority)
                    .Where(item => item.CanExecute(e.Message.Text)))
                {
                    var r = item.Execute(e);
                    if (r.Result && r.SendFlag)
                    {
                        return r;
                    }
                }
                return result;
            }
            catch (Exception exc)
            {
                MainSave.CQLog.Info("异常抛出", exc.Message + exc.StackTrace);
                return result;
            }
        }
    }
}
