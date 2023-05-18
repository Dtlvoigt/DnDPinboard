namespace EasyPinboard.Models
{
    public class ResultsAPI
    {
        public int count { get; set; }
        public List<Result> results { get; set; }

        public class Result
        {
            public string index { get; set; }
            public string name { get; set; }
            public string url { get; set; }
        }
    }
}
