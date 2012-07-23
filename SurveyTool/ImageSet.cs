﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurveyTool
{
    public class ImageSet
    {
        private List<IQuestions> questionList = new List<IQuestions>();
        private List<String> imagePath = new List<string>();
        private int currQuestion = 0;

        #region Properties

            public int NumQuestions 
            { 
                get { return questionList.Count; } 
            }
            public bool HasPrevQuestion
            {
                get { return currQuestion > 0; } //first question is 1
            }
            public bool HasNextQuestion
            {
                get { return currQuestion < NumQuestions -1; }
            }
            public int NumImages
            {
                get { return imagePath.Count; }
            }

        #endregion
        
        #region Public Methods

            public ImageSet()
            {

            }

            public void AddQuestion(IQuestions question)
            {
                questionList.Add(question);
            }
            public void RemoveQuestion(IQuestions question)
            {
                questionList.Remove(question);
            }
            public IQuestions GetCurrentQuestion() //we've just come to this question from another image list; resume where we left off
            {
                return questionList[currQuestion]; //TODO: check this -- getprev at 1 = 0
            }
            public IQuestions GetPreviousQuestion()
            {
                return questionList[--currQuestion]; //TODO: check this -- getprev at 1 = 0
            }
            public IQuestions GetNextQuestion()
            {
                return questionList[++currQuestion]; //TODO: check this -- getnext at 0 = 1

            }
            public void AddPicture(string path)
            {
                imagePath.Add(path);
            }
            public string GetImageAt(int index)
            {
                return imagePath[index];
            }

        #endregion

    }
}
