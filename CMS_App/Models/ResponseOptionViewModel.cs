using System;
using BizLayer;

// created by Charles Drews
namespace CMS_App.Models
{
    public class ResponseOptionViewModel
    {
        public ResponseOption ResponseOption { get; set; }
        
        public bool Selected { get; set; }

        public ResponseOptionViewModel()
        {
            ResponseOption = new ResponseOption();
            Selected = false;
        }

        public ResponseOptionViewModel(ResponseOption responseOption)
        {
            ResponseOption = responseOption;
            Selected = false;
        }

        public ResponseOptionViewModel(ResponseOption responseOption, bool selected)
        {
            ResponseOption = responseOption;
            Selected = selected;
        }
    }
}