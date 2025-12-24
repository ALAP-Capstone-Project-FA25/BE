namespace ALAP.Entity.DTO.Response
{
    public class CassoResponse
    {
        public int error { get; set; }
        public List<CassoData> data { get; set; } = new List<CassoData>();
    }
    public class CassoData
    {
     
       public int id { get; set; }
        public string description { get; set; }
        public float amount { get; set; }


    }   
}
