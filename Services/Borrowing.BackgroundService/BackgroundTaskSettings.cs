namespace BookOnline.Borrowing.BackgroundTask
{
    public class BackgroundTaskSettings
    {
        public int CheckPeriod { get; set; }
        
        public int ConfirmTime { get; set; }
        public string ConnectionString { get; set; }
    }
}
