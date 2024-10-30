namespace PublicInfos.Model
{
    public class DelayAPI
    {
        public DelayAPI(long groupID, long qQID)
        {
            GroupID = groupID;
            QQID = qQID;
        }

        public long GroupID { get; set; }

        public long QQID { get; set; }
    }
}
