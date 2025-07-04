using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Core;
using Vidyasetu_API.Common;
using Vidyasetu_API.Models;
using System.Text.Json;
using Vidyasetu_API.DTOs.Response;

namespace Vidyasetu_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizEvaluationController : Controller
    {
        private readonly VidyasetuAI_DevContext _db;

        public QuizEvaluationController(VidyasetuAI_DevContext db)
        {
            _db = db;
        }

        [HttpPost("evaluate")]
        public async Task<IActionResult> EvaluateQuiz([FromBody] QuizEvaluationRequest request)
        {
            // Fetch question JSON from DB
            var decodeToken = EncryptDecryptHelper.Decrypt(request.token);

            var userQuizeReponses = await _db.UserQuizeReponses.Where(x => x.RequestId == Convert.ToInt64(decodeToken.requestId)).ToListAsync();
            if (userQuizeReponses.Count > 0)
            {
                request.Answers = userQuizeReponses
                    .Select(x => new UserAnswerModel
                    {
                        QuestionText = x.Question,
                        GivenAnswer = x.UserAnswer
                    })
                    .ToList();
            }
            else
            {
                var userQuizeReponse = request.Answers
                    .Select(x => new UserQuizeReponse
                    {
                        RequestId = Convert.ToInt64(decodeToken.requestId),
                        Question = x.QuestionText,
                        UserAnswer = x.GivenAnswer
                    })
                    .ToList();

                await _db.AddRangeAsync(userQuizeReponse);
                await _db.SaveChangesAsync();
            }

            var userResponse = await _db.UserRequestResponses.FirstOrDefaultAsync(x => x.Id == Convert.ToInt64(decodeToken.requestId));

            if (userResponse == null || string.IsNullOrWhiteSpace(userResponse.QuestionJson))
            {
                return NotFound("Request not found or questions not available.");
            }

            var questions = JsonSerializer.Deserialize<List<QuestionModel>>(userResponse.QuestionJson);

            if (questions == null || !questions.Any())
            {
                return BadRequest("Invalid or empty question data.");
            }

            int total = questions.Count;
            int correct = 0;
            var incorrectList = new List<IncorrectQuestionDetail>();
            var correctList = new List<CorrrectQuestionDetail>();

            foreach (var userAnswer in request.Answers)
            {
                var question = questions.FirstOrDefault(q =>
                    string.Equals(q.QuestionText.Trim(), userAnswer.QuestionText.Trim(), StringComparison.OrdinalIgnoreCase));

                if (question != null)
                {
                    if (string.Equals(question.CorrectAnswer.Trim(), userAnswer.GivenAnswer.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        correctList.Add(new CorrrectQuestionDetail
                        {
                            QuestionText = question.QuestionText,
                            CorrectAnswer = question.CorrectAnswer,
                            GivenAnswer = userAnswer.GivenAnswer,
                            TimeStamp = question.Timestamp
                        });
                        correct++;
                    }
                    else
                    {
                        incorrectList.Add(new IncorrectQuestionDetail
                        {
                            QuestionText = question.QuestionText,
                            CorrectAnswer = question.CorrectAnswer,
                            GivenAnswer = userAnswer.GivenAnswer,
                            TimeStamp = question.Timestamp
                        });
                    }
                }
            }

            double percentage = total == 0 ? 0 : (correct * 100.0) / total;
            string result = percentage >= 50 ? "Pass" : "Fail";

            string medal = percentage switch
            {
                >= 90 => "Gold",
                >= 75 => "Silver",
                >= 50 => "Bronze",
                _ => "None"
            };
            var logdeviceRequest = await _db.DeviceLogDetails
            .FirstOrDefaultAsync(x => x.Id == userResponse.RequestId);
            var response = new QuizEvaluationResponse
            {
                TotalQuestions = total,
                CorrectAnswers = correct,
                TotalTimeTaken = "00:00",
                ScorePercentage = Math.Round(percentage, 2),
                Result = result,
                Medal = medal,
                Title = userResponse.Title ?? string.Empty,
                Flashcards = userResponse.FlashcardJson != null
                    ? JsonSerializer.Deserialize<List<Flashcard>>(userResponse.FlashcardJson)
                    : new List<Flashcard>(),
                Summary = userResponse.SummaryJson ?? string.Empty,
                IncorrectQuestions = incorrectList,
                CorrectQuestions = correctList,
                VideoUrl = logdeviceRequest?.RequestUrl ?? string.Empty,
                Recommendations = userResponse.RecommendationsJson != null
                    ? JsonSerializer.Deserialize<List<Recommendation>>(userResponse.RecommendationsJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })
                    : new List<Recommendation>()
            };

            return Ok(ApiResponse<QuizEvaluationResponse>.CreateSuccess(response, "QuizEvaluation done successfully!"));
        }
    }
}
