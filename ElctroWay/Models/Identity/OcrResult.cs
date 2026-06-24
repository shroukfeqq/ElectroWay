//namespace ElctroWay.Models.Identity
//{

//        public class OcrResult
//        {
//            public bool IsValid { get; set; }
//            public double Score { get; set; }
//            public string Reason { get; set; }
//            public string ExtractedName { get; set; }
//            public string ExtractedIdNumber { get; set; }
//        }

//}
namespace ElctroWay.Models.Identity
{
    public class OcrResult
    {
        public bool IsValid { get; set; }

        public double Score { get; set; }

        public string Reason { get; set; }

        public string ExtractedName { get; set; }

        public string ExtractedIdNumber { get; set; }

        public double FaceMatchScore { get; set; }
    }
}
