using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurveyTool
{
    class ImageSet
    {
        private List<IQuestions> questionList;
        private List<String> imagePath;
        private int currQuestion = 0;

        #region Properties

            public int NumQuestions 
            { 
                get { return questionList.Count; } 
            }
            public bool HasPrevQuestion
            {
                get { return currQuestion > 0; }
            }
            public bool HasNextQuestion
            {
                get { return currQuestion < NumQuestions-1; }
            }

        #endregion

        #region Public Methods

            public void AddQuestion(IQuestions question)
            {
                questionList.Add(question);
            }
            public void RemoveQuestion(IQuestions question)
            {
                questionList.Remove(question);
            }
            public IQuestions GetPreviousQuestion()
            {
                return null;
            }
            public IQuestions GetNextQuestion()
            {
                return null;
            }

        #endregion

    }
}
