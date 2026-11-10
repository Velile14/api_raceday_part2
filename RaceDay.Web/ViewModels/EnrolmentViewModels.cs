namespace RaceDay.Web.ViewModels
{
    public class EnrolmentViewModel
    {
        public int EnrolmentID { get; set; }

        public int EventID { get; set; }

        public int CategoryID { get; set; }

        public int UserID { get; set; }

        public EventViewModel? Event { get; set; }

        public CategoryViewModel? Category { get; set; }
    }

    public class CreateEnrolmentViewModel
    {
        public int EventID { get; set; }

        public int CategoryID { get; set; }

        public List<CategoryViewModel> Categories { get; set; } = new();
    }

    public class CategoryViewModel
    {
        public int CategoryID { get; set; }

        public int EventID { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int MinimumAge { get; set; }

        public int MaximumAge { get; set; }

        public decimal Distance { get; set; }
    }
}
