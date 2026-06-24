using ElctroWay.Models.Identity;
using ElctroWay.Service.Interfaces;
using System.Text.RegularExpressions;
using Tesseract;

namespace ElctroWay.Service.Implementations
{
    public class OcrService : IOcrService
    {
        public OcrResult VerifyAsync(
            string frontIdPath,
            string backIdPath,
            string selfiePath,
            string fullNameFromForm)
        {
            var tessDataPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "tessdata"
            );

            string frontText = ExtractText(frontIdPath, tessDataPath);

            string backText = ExtractText(backIdPath, tessDataPath);
           

            var extractedName = ExtractName(frontText);
            Console.WriteLine("===== FRONT TEXT =====");
            Console.WriteLine(frontText);

            Console.WriteLine("===== BACK TEXT =====");
            Console.WriteLine(backText);
            var extractedId =
                ExtractNationalId(frontText + " " + backText);

            double faceScore =
                MockFaceMatch(selfiePath, frontIdPath);

            bool nameMatch =
                Normalize(fullNameFromForm) ==
                Normalize(extractedName);

            bool idValid =
                !string.IsNullOrEmpty(extractedId) &&
                extractedId.Length == 14;

            double score =
                CalculateScore(
                    nameMatch,
                    idValid,
                    faceScore);

            bool isValid = score >= 0.70;

            string reason =
                GetReason(
                    nameMatch,
                    idValid,
                    faceScore);

            return new OcrResult
            {
                IsValid = isValid,
                Score = score,
                FaceMatchScore = faceScore,
                ExtractedName = extractedName,
                ExtractedIdNumber = extractedId,
                Reason = reason
            };
        }

        private string ExtractText(
            string imagePath,
            string tessDataPath)
        {
            using var engine =
                new TesseractEngine(
                    tessDataPath,
                    //"eng",
                    "ara+eng",
                    EngineMode.LstmOnly);
            engine.SetVariable("preserve_interword_spaces", "1");
            engine.SetVariable("user_defined_dpi", "300");

            using var img =
                Pix.LoadFromFile(imagePath);

            using var page =
                engine.Process(img);

            return page.GetText();
        }

        private string ExtractName(string text)
        {
            var lines = text.Split('\n');

            foreach (var line in lines)
            {
                var clean = line.Trim();

                if (clean.Length > 5 &&
                    !clean.Any(char.IsDigit))
                {
                    return clean;
                }
            }

            return string.Empty;
        }

        private string ExtractNationalId(string text)
        {
            var match =
                Regex.Match(text, @"\d{14}");

            return match.Success
                ? match.Value
                : string.Empty;
        }

        private double MockFaceMatch(
            string selfiePath,
            string idImagePath)
        {
            //Face Recognition

            return 0.85;
        }

        private double CalculateScore(
            bool nameMatch,
            bool idValid,
            double faceScore)
        {
            double score = 0;

            if (nameMatch)
                score += 0.4;

            if (idValid)
                score += 0.3;

            score += faceScore * 0.3;

            return score;
        }

        private string GetReason(
            bool nameMatch,
            bool idValid,
            double faceScore)
        {
            if (!idValid)
                return "Invalid National ID";

            if (!nameMatch)
                return "Name mismatch";

            if (faceScore < 0.7)
                return "Face mismatch";

            return "Verification Passed";
        }

        private string Normalize(string value)
        {
            return value?
                .Trim()
                .ToLower()
                .Replace(" ", "")
                ?? string.Empty;
        }
    }
}