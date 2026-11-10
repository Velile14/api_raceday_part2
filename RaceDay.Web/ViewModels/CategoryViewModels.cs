namespace RaceDay.Web.ViewModels
{
    public class CategoryFormViewModel
    {
        public CategoryViewModel Category { get; set; } = new();

        public List<EventViewModel> Events { get; set; } = new();
    }
}
