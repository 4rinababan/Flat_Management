namespace MyApp.Web.Shared.Components
{
    public class MenuItem
    {
        public string Id { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string IconName { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string? Url { get; set; }   // <── untuk routing
        public List<MenuItem>? Submenu { get; set; }
    }
}