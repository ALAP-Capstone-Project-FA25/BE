namespace ALAP.Entity.DTO.Request
{
    public class CreateUpdateEntryTestDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;
        public List<CreateUpdateEntryTestQuestionDto> Questions { get; set; } = [];
    }

    public class CreateUpdateEntryTestQuestionDto
    {
        public long Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int DisplayOrder { get; set; } = 0;
        public List<CreateUpdateEntryTestOptionDto> Options { get; set; } = [];
    }

    public class CreateUpdateEntryTestOptionDto
    {
        public long Id { get; set; }
        public string OptionCode { get; set; } = string.Empty;
        public string OptionText { get; set; } = string.Empty;
        public int DisplayOrder { get; set; } = 0;
        public List<long> CategoryIds { get; set; } = []; // Danh sách môn học liên quan
        public int Weight { get; set; } = 1;
    }
}
