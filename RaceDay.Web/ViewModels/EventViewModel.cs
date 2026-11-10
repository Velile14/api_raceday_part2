namespace RaceDay.Web.ViewModels
{//start of namespace

    public class EventViewModel
    {//start of EventViewModel class

        // Event class properties
        public int EventID { get; set; }

        // EventName property with default value of empty string
        public string EventName { get; set; } = string.Empty;

        // Description property with default value of empty string
        public string Description { get; set; } = string.Empty;

        // EventDate property of type DateOnly
        public DateOnly EventDate { get; set; }

        // Location property with default value of empty string
        public string Location { get; set; } = string.Empty;
        
        // Distance property of type decimal
        public decimal Distance { get; set; }

        // Foreign key to EventType
        public int EventTypeID { get; set; }

        // Foreign key to User
        public int UserID { get; set; }

        // Foreign key to Organizer
        public int OrganizerID { get; set; }

    }//end of EventViewModel class

}//end of namespace
