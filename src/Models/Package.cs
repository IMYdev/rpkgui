namespace rpkGUI.Models
{
    public class Package
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public override string ToString() => $"{Name} - {Description}";
    }
}