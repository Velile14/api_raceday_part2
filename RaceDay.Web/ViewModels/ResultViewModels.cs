namespace RaceDay.Web.ViewModels
{
    public class ResultViewModel
    {
        public int ResultID { get; set; }

        public int EnrolmentID { get; set; }

        public TimeSpan FinishTime { get; set; }

        public int FinishPosition { get; set; }

        public EnrolmentViewModel? Enrolment { get; set; }
    }

    public class CreateResultViewModel
    {
        public int EventID { get; set; }

        public int EnrolmentID { get; set; }

        public TimeSpan FinishTime { get; set; }

        public int FinishPosition { get; set; }
    }


}
