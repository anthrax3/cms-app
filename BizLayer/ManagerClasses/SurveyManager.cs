using System;
using System.Collections.Generic;
using System.Linq;
using BizLayer.HelperClasses;

// created by Charles Drews
namespace BizLayer.ManagerClasses
{
    public static class SurveyManager
    {
        #region Create
        /// <summary>
        /// Create the survey questions & response options that will be used for all course surveys. This will be called by Global.asax in CMS_App.
        /// </summary>
        public static void CreateSurvey()
        {
            using (var db = new CmsModelContainer())
            {
                if (!db.Questions.Any())
                {
                    // create two questions
                    var question1 = new Question
                    {
                        Id = Guid.NewGuid(),
                        Text = "Please rate this course",
                        Number = 1,
                        ResponseOptions = CreateScaleResponseOptions()
                    };
                    var question2 = new Question
                    {
                        Id = Guid.NewGuid(),
                        Text = "Please rate the instructor(s)",
                        Number = 2,
                        ResponseOptions = CreateScaleResponseOptions()
                    };

                    db.Questions.Add(question1);
                    db.Questions.Add(question2);
                }

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Create and return a list of ResponseOption objects representing a scale of 1 - 5 (poor - excellent)
        /// </summary>
        /// <returns></returns>
        public static List<ResponseOption> CreateScaleResponseOptions()
        {
            var responseOptions = new List<ResponseOption>
            {
                new ResponseOption
                {
                    Id = Guid.NewGuid(),
                    Text = "1 - Poor",
                    Value = 1,
                    SortOrder = 1,
                },
                new ResponseOption
                {
                    Id = Guid.NewGuid(),
                    Text = "2 - Fair",
                    Value = 2,
                    SortOrder = 2,
                },
                new ResponseOption
                {
                    Id = Guid.NewGuid(),
                    Text = "3 - Average",
                    Value = 3,
                    SortOrder = 3,
                },
                new ResponseOption
                {
                    Id = Guid.NewGuid(),
                    Text = "4 - Good",
                    Value = 4,
                    SortOrder = 4,
                },
                new ResponseOption
                {
                    Id = Guid.NewGuid(),
                    Text = "5 - Excellent",
                    Value = 5,
                    SortOrder = 5,
                }
            };

            return responseOptions;
        }

        /// <summary>
        /// Create a new instance of ResponseEvent and save it in the database
        /// </summary>
        /// <param name="enrollmentId"></param>
        /// <param name="responseOptionId"></param>
        /// <returns></returns>
        public static ReturnValue CreateResponseEvent(Guid enrollmentId, Guid responseOptionId)
        {
            var retVal = new ReturnValue();
            using (var db = new CmsModelContainer())
            {
                try
                {
                    // get question associated with specified responseOptionId
                    var question = GetQuestionByResponseOptionId(responseOptionId);

                    // get all response options associated with that question
                    var responseOptions = GetResponseOptionsByQuestionId(question.Id);
                    var responseOptionIds = new List<Guid>();
                    foreach (var responseOption in responseOptions)
                    {
                        responseOptionIds.Add(responseOption.Id);
                    }

                    // check if a response event exists for the specified enrollment/question combo
                    var existingResponseEvent = db.ResponseEvents.FirstOrDefault(e =>
                        e.EnrollmentId == enrollmentId &&
                        responseOptionIds.Contains(e.ResponseOptionId));

                    // if a response does already exist for the specified enrollment/question combo,
                    // then delete it before adding a new response event for that enrollment/question,
                    // which may specify a different response option than the existing response event
                    if (existingResponseEvent != null && existingResponseEvent.Id != Guid.Empty)
                    {
                        db.ResponseEvents.Remove(existingResponseEvent);
                    }

                    // now create a new response event for the specified enrollment & response option
                    var responseEvent = new ResponseEvent
                    {
                        Id = Guid.NewGuid(),
                        EnrollmentId = enrollmentId,
                        ResponseOptionId = responseOptionId,
                    };

                    // add the new response event to the relevant collections
                    var enrollment = db.Enrollments.FirstOrDefault(e => e.Id == enrollmentId);
                    if (enrollment != null)
                    {
                        enrollment.ResponseEvents.Add(responseEvent);
                    }
                    var option = db.ResponseOptions.FirstOrDefault(o => o.Id == responseOptionId);
                    if (option != null)
                    {
                        option.ResponseEvents.Add(responseEvent);
                    }
                        
                    db.SaveChanges();
                    retVal.Success = true;
                }
                catch (Exception ex)
                {
                    retVal.Success = false;
                    retVal.ErrorMessages.Add(ex.ToString());
                }
            }
            return retVal;
        }

        #endregion Create

        #region Read

        /// <summary>
        /// Retrieve the survey questions
        /// </summary>
        /// <returns>List of Question objects</returns>
        public static List<Question> GetSurveyQuestions()
        {
            using (var db = new CmsModelContainer())
            {
                var questions = new List<Question>();
                questions = db.Questions.OrderBy(q => q.Number).ToList();
                return questions;
            }
        }

        /// <summary>
        /// Returns the Question instance specified by the argument questionId
        /// </summary>
        /// <returns></returns>
        public static Question GetQuestionById(Guid questionId)
        {
            using (var db = new CmsModelContainer())
            {
                var question = new Question();
                question = db.Questions.FirstOrDefault(q => q.Id == questionId);
                return question;
            }
        }

        /// <summary>
        /// Returns the Question instance corresponding to the specified ResponseOption instance
        /// </summary>
        /// <param name="responseOptionId"></param>
        /// <returns></returns>
        public static Question GetQuestionByResponseOptionId(Guid responseOptionId)
        {
            using (var db = new CmsModelContainer())
            {
                var responseOption = GetResponseOptionById(responseOptionId);
                var question = GetQuestionById(responseOption.QuestionId);
                return question;
            }
        }

        /// <summary>
        /// Returns the response options associated with the question specified by the argument questionId
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public static List<ResponseOption> GetResponseOptionsByQuestionId(Guid questionId)
        {
            using (var db = new CmsModelContainer())
            {
                var responseOptions = new List<ResponseOption>();
                responseOptions.AddRange(db.ResponseOptions.Where(r => r.QuestionId == questionId).OrderBy(r => r.SortOrder).ToList());
                return responseOptions;
            }
        }

        /// <summary>
        /// Returns the ResponseOption instance specified by the argument responseOptionId
        /// </summary>
        /// <param name="responseOptionId"></param>
        /// <returns></returns>
        public static ResponseOption GetResponseOptionById(Guid responseOptionId)
        {
            using (var db = new CmsModelContainer())
            {
                var responseOption = new ResponseOption();
                responseOption = db.ResponseOptions.FirstOrDefault(r => r.Id == responseOptionId);
                return responseOption;
            }
        }

        /// <summary>
        /// Returns a list of ResponseEvents instances to the specified question for the specified course
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="questionid"></param>
        /// <returns></returns>
        public static List<ResponseEvent> GetResponsesByCourseAndQuestion(Guid courseId, Guid questionId)
        {
            using (var db = new CmsModelContainer())
            {
                var enrollments = db.Enrollments.Where(e => e.Course.Id == courseId);
                var responseOptionIds = db.ResponseOptions.Where(o => o.QuestionId == questionId).Select(o => o.Id);
                var responseEvents = db.ResponseEvents;

                var responses = new List<ResponseEvent>();
                foreach (var enrollment in enrollments)
                {
                    responses.AddRange(enrollment.ResponseEvents.Where(e => e.ResponseOption.QuestionId == questionId));
                }
                return responses;
            }
        }

        /// <summary>
        /// Returns a list of ResponseEvents instances to the specified question for the specified course & user
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="questionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static ResponseEvent GetResponseByCourseQuestionAndUser(Guid courseId, Guid questionId, Guid userId)
        {
            using (var db = new CmsModelContainer())
            {
                var enrollment = db.Enrollments.FirstOrDefault(e => e.Course.Id == courseId &&
                                                                    e.Students.Id == userId);
                var response = new ResponseEvent();
                if (enrollment != null)
                {
                    response = enrollment.ResponseEvents.FirstOrDefault(e =>
                                                        e.ResponseOption.QuestionId == questionId);
                }

                return response;
            }
        }

        /// <summary>
        /// Returns the number of responses to the specified question for the specified course
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public static int GetResponseCountByCourseAndQuestion(Guid courseId, Guid questionId)
        {
            var responses = new List<ResponseEvent>();
            responses = GetResponsesByCourseAndQuestion(courseId, questionId);
            return responses.Count();
        }

        /// <summary>
        /// Returns the sum of the values of the responses to the specified question for the specified course
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public static int GetResponseSumByCourseAndQuestion(Guid courseId, Guid questionId)
        {
            var responses = new List<ResponseEvent>();
            responses = GetResponsesByCourseAndQuestion(courseId, questionId);

            var selectedOptions = new List<ResponseOption>();
            foreach (var response in responses)
            {
                selectedOptions.Add(GetResponseOptionById(response.ResponseOptionId));
            }

            int sum = 0;
            foreach (var selectedOption in selectedOptions)
            {
                sum += selectedOption.Value;
            }

            return sum;
        }

        /// <summary>
        /// Returns average response to the specified question for the specified course
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public static float GetAverageRatingByCourseAndQuestion(Guid courseId, Guid questionId)
        {
            int count = GetResponseCountByCourseAndQuestion(courseId, questionId);
            int sum = GetResponseSumByCourseAndQuestion(courseId, questionId);

            if (count == 0)
                return 0;
            else
                return (float)sum / (float)count;
        }

        #endregion Read
    }
}
