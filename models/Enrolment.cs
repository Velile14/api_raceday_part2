namespace api_raceday_part2.models
{//start of namespace

    public class Enrolment
    {//start of Enrolment class

        public int EnrolmentID { get; set; }
        public int EventID { get; set; }
        public int CategoryID { get; set; }
        public int UserID { get; set; }

        public Event Event { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public User User { get; set; } = null!;

    }//end of Enrolment class

}//end of namespace
