namespace api_raceday_part2.models
{//start of namespace

    public class Result
    {//start of Result class

        public int ResultID { get; set; }
        public int EnrolmentID { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int FinishPosition { get; set; }

        public Enrolment Enrolment { get; set; } = null!;

    }//end of Result class

}//end of namespace
