namespace api_raceday_part2.models
{//start of namespace

    public class Event
    {//start of Event class

        //event class properties 
        public int EventID { get; set; }    
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public int EventTypeID { get; set; } // Foreign key to EventType
        public int UserID { get; set; } // Foreign key to User

    }//end of Event class

}//end of namespace
