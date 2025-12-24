namespace ALAP.Entity.DTO.Request
{
    public class SubmitEntryTestDto
    {
        public long EntryTestId { get; set; }
        public Dictionary<long, string> Answers { get; set; } = []; // QuestionId -> OptionCode
    }
}
