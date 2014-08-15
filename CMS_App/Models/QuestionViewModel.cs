using System;
using System.Collections.Generic;
using BizLayer;
using BizLayer.ManagerClasses;

// created by Charles Drews
namespace CMS_App.Models
{
    public class QuestionViewModel
    {
        public Guid CourseId { get; set; }

        public Question Question { get; set; }

        public int CountOfResponses { get; set; }

        // probably won't need to display the sum, but keep it now for testing purposes
        public int SumOfResponses { get; set; }

        public float AverageOfResponses { get; set; }

        public QuestionViewModel()
        {
            CourseId = Guid.Empty;
            Question = new Question();
            CountOfResponses = 0;
            SumOfResponses = 0;
            AverageOfResponses = 0;
        }

        public QuestionViewModel(Guid courseId, Guid questionId)
        {
            CourseId = courseId;
            Question = SurveyManager.GetQuestionById(questionId);
            CountOfResponses = SurveyManager.GetResponseCountByCourseAndQuestion(courseId, questionId);
            SumOfResponses = SurveyManager.GetResponseSumByCourseAndQuestion(courseId, questionId);
            AverageOfResponses = SurveyManager.GetAverageRatingByCourseAndQuestion(courseId, questionId);
        }
    }
}