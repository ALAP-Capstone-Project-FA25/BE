namespace ALAP.Entity.DTO.Response
{
    public class UploadResponse
    {
        public string message { get; set; }
        public UploadData data { get; set; }
        public bool success { get; set; }
        public int statusCode { get; set; }
    }
    public class UploadData
    {
        public string downloadUrl { get; set; }
        public string fileName { get; set; }
        public double fileSizeMb { get; set; }
        public string fileType { get; set; }
        public DateTime uploadTime { get; set; }
    }
}
