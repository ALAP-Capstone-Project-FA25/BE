namespace App.Entity.DTO.Request
{
    public class CreateUpdateTopicDto
    {
        public long Id { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }
        public int OrderIndex { get; set; }
        public long CourseId { get; set; }
    }
}
