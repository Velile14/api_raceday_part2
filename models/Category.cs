namespace api_raceday_part2.models
{//start of namespace

    public class Category
    {//start of Category class

        public int CategoryID { get; set; }
        public int EventID { get; set; }

        public string CategoryName { get; set; } = string.Empty;
        public int MinimumAge { get; set; }
        public int? MaximumAge { get; set; }
        public decimal Distance { get; set; }

        public Event Event { get; set; } = null!;

    }//end of Category class

}//end of namespace
