namespace SEAR_DataContract
{
    public class HomeDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
    public class NeedJSON
    {
        public string? Name { get; set; }
        public int MeowId { get; set; }
    }
    public class JsonList
    {
        public List<NeedJSON>? JSONList { get; set; }
        public string? ExceptionMessage { get; set; }
    }
    public class RequestGetWithJSONList
    {
        public string? Name { get; set; }
        public int Id { get; set; }
    }
    public class DatabaseUsers
    {
        public string? ID { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Rank { get; set; }
    }
}