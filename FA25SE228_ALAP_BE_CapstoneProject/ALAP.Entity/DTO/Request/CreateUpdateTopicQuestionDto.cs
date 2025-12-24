namespace App.Entity.DTO.Request
{
	public class CreateUpdateTopicQuestionDto
	{
		public long Id { get; set; }
		public long TopicId { get; set; }
		public int MaxChoices { get; set; } = 1;
        public string Question { get; set; } = string.Empty;
	}
}


