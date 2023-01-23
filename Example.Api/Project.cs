namespace Example.Api
{
    public record UserTask(int ProjectId, string Owner)
    {
        public bool Decision { get; set; }
    }
    public class Project
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }

    }
}