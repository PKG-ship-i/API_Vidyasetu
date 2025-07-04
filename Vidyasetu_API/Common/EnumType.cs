using System.ComponentModel;

namespace Vidyasetu_API.Common
{
    public enum QuestionType
    {
        [Description("mcq")]
        MCQ = 1,

        [Description("fillinblank")]
        FillInBlank = 2,

        [Description("opentext")]
        OpenText = 3,

        [Description("truefalse")]
        TrueFalse = 4
    }

    public enum DifficultyLevel
    {
        [Description("easy")]
        Easy = 1,

        [Description("medium")]
        Medium = 2,

        [Description("hard")]
        Hard = 3
    }

    public enum SourceType
    {
        [Description("pdf")]
        Pdf = 1,

        [Description("youtube")]
        Youtube = 2,

        [Description("prompt")]
        Prompt = 3,

        [Description("context")]
        Context = 4
    }

    public enum LanguageType
    {
        [Description("hindi")]
        Hindi = 1,

        [Description("english")]
        English = 2,

        [Description("gujrati")]
        Gujrati = 3
    }
}
