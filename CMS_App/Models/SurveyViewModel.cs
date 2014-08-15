using System;
using System.Collections.Generic;
using System.Web;
using BizLayer;
using BizLayer.ManagerClasses;

// created by Charles Drews
namespace CMS_App.Models
{
    public class SurveyViewModel
    {
        public Guid CourseId { get; set; }

        public Guid EnrollmentId { get; set; }

        public Dictionary<Question, ResponseOptionViewModel[]> Survey { get; set; }

        public int CountOfQuestions { get; set; }

        public Models.SelectedOption[] SelectedResponseOptions { get; set; }

        public SurveyViewModel()
        {
            CourseId = Guid.Empty;
            EnrollmentId = Guid.Empty;

            Survey = new Dictionary<Question, ResponseOptionViewModel[]>();
            var questions = SurveyManager.GetSurveyQuestions();
            foreach (var question in questions)
            {
                var responseOptions = SurveyManager.GetResponseOptionsByQuestionId(question.Id);
                var responseOptionViewModels = new List<ResponseOptionViewModel>();
                foreach (var responseOption in responseOptions)
                {
                    responseOptionViewModels.Add(new ResponseOptionViewModel(responseOption));
                }
                Survey.Add(question, responseOptionViewModels.ToArray());
            }

            CountOfQuestions = questions.Count;
            SelectedResponseOptions = new Models.SelectedOption[CountOfQuestions];
        }

        public SurveyViewModel(Course course)
        {
            CourseId = course.Id;
            
            User user;
            var getUserRetVal = UserManager.GetUserByEmailAddress(HttpContext.Current.User.Identity.Name,
                                                                  out user);
            
            Survey = new Dictionary<Question, ResponseOptionViewModel[]>();
            var questions = SurveyManager.GetSurveyQuestions(); 
            
            if (getUserRetVal.Success)
            {
                // if user exists, attempt to get enrollment instance for that user
                var enrollment = CourseManager.GetEnrollmentByCourseIdAndStudentEmail(CourseId, user.Email);
                EnrollmentId = enrollment.Id;

                foreach (var question in questions)
                {
                    var response = SurveyManager.GetResponseByCourseQuestionAndUser(CourseId,
                                                                                    question.Id, user.Id);
                    
                    var responseOptions = SurveyManager.GetResponseOptionsByQuestionId(question.Id);
                    var responseOptionViewModels = new List<ResponseOptionViewModel>();
                    foreach (var responseOption in responseOptions)
                    {
                        if (response != null && response.ResponseOptionId == responseOption.Id)
                        {
                            responseOptionViewModels.Add(new ResponseOptionViewModel(responseOption, true));
                        }
                        else
                        {
                            responseOptionViewModels.Add(new ResponseOptionViewModel(responseOption, false));
                        }
                    }
                    Survey.Add(question, responseOptionViewModels.ToArray());
                }
            }
            else
            {
                // is user doesn't exist, don't bother attempting to get enrollment instance
                EnrollmentId = Guid.Empty;

                foreach (var question in questions)
                {
                    var responseOptions = SurveyManager.GetResponseOptionsByQuestionId(question.Id);
                    var responseOptionViewModels = new List<ResponseOptionViewModel>();
                    foreach (var responseOption in responseOptions)
                    {
                        responseOptionViewModels.Add(new ResponseOptionViewModel(responseOption));
                    }
                    Survey.Add(question, responseOptionViewModels.ToArray());
                } 
            }
            
            CountOfQuestions = questions.Count;
            SelectedResponseOptions = new Models.SelectedOption[CountOfQuestions];
        }
    }
}