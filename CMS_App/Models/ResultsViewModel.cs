using System;
using System.Collections.Generic;
using BizLayer;
using BizLayer.ManagerClasses;

// created by Charles Drews
namespace CMS_App.Models
{
    public class ResultsViewModel
    {
        public Course Course { get; set; }

        public List<QuestionViewModel> QuestionViewModels { get; set; }

        public ResultsViewModel()
        {
            Course = new Course();
            QuestionViewModels = new List<QuestionViewModel>();
        }

        public ResultsViewModel(Guid courseId)
        {
            Course = CourseManager.GetCourseById(courseId);
            QuestionViewModels = new List<QuestionViewModel>();
            var questions = SurveyManager.GetSurveyQuestions();
            foreach (var question in questions)
            {
                QuestionViewModels.Add(new QuestionViewModel(courseId, question.Id));
            }
        }
    }
}