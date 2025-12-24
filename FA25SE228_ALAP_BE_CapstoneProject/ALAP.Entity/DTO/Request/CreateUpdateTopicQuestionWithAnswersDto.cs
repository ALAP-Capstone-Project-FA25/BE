namespace ALAP.Entity.DTO.Request
{
	public class TopicQuestionAnswerItemDto
	{
		public long Id { get; set; }
		public string Answer { get; set; } = string.Empty;
		public bool IsCorrect { get; set; } = false;
	}

	public class CreateUpdateTopicQuestionWithAnswersDto
	{
		public long Id { get; set; }
		public long TopicId { get; set; }
		public int MaxChoices { get; set; } = 1;
		public string Question { get; set; } = string.Empty;
		public long? ReferrerLessonId { get; set; }
		public List<TopicQuestionAnswerItemDto> Answers { get; set; } = new List<TopicQuestionAnswerItemDto>();
	}
}


