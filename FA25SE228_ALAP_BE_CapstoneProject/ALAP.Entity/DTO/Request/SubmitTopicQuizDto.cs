namespace ALAP.Entity.DTO.Request
{
    public class SubmitTopicQuizDto
    {
        public long TopicId { get; set; }
        public long UserTopicId { get; set; }
        public Dictionary<long, List<long>> Answers { get; set; } = new Dictionary<long, List<long>>(); // QuestionId -> List of AnswerIds
        public int? TimeSpent { get; set; } // Seconds
    }
}

