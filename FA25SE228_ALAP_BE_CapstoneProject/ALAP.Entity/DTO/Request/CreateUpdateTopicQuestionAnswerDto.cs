namespace App.Entity.DTO.Request
{
	public class CreateUpdateTopicQuestionAnswerDto
	{
		public long Id { get; set; }
		public long TopicQuestionId { get; set; }
		public string Answer { get; set; } = string.Empty;
		public bool IsCorrect { get; set; } = false;
	}
}


