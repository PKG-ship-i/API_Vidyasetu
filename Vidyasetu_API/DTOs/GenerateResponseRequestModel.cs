namespace Vidyasetu_API.DTOs
{




    public class CommonRequestModel
    {
        public int NumberOfQuestions { get; set; } = 5; // Default value
        public int QuestionsTypeId { get; set; }
        public int DifficultyTypeId { get; set; }
        public long DeviceId { get; set; }
        public int LanguageId { get; set; }
        public int SourceTypeId { get; set; } // Correct type

    }


    public class GenerateVideoRequestModel : CommonRequestModel
    {
        public string VideoUrl { get; set; } = default!; // Correct typesss


    }


    public class GenerateContextRequestModel : CommonRequestModel
    {
        public string Context { get; set; } = default!; // Correct type

    }

    public class GeneratePromptRequestModel : CommonRequestModel
    {
        public string Prompt { get; set; } = default!; // Correct type


    }
}
