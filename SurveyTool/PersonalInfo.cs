using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace SurveyTool
{
    public class PersonalInfo : IDataErrorInfo//, ISerializable
    {

        #region Fields

        //updated in realtime using binding, for validation purposes
        private int? age;
        private string name;
        private string dept;
        private int numValidatedTextboxes = 3;

        //the radio buttons aren't hooked in with databinding currently, due to the fact that working around a weird WPF behaviour 
        //(where the binding "disappears" when a value is set to unselected) is probably more ugly than just using the old-fashioned
        //check-the-form-before-saving-values way (this checking is prompted by calling UpdateFromCurrentRadioButtonValues() )
        //possible TODO: look into binding ways of doing this that are not too ugly
        private int gender;
        private int experience;
        private int vision;
        private string visionProblemsDescription;

        //keep track of the problems we saw:
        private static byte hasProblems = 0; //each bit is one value being used for the check;
                                             //overall value must be 0 for everything to be valid
        private byte[] checkNumbers = new byte[8]; //used to store 1, 2...2^8

        #endregion

        #region Properties

        public static bool DataIsValid
        {
            get { return (hasProblems == 0); }
        }
        public int? Age
        {
            get { return age; }
            set { age = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Dept
        {
            get { return dept; }
            set { dept = value; }
        }

        #endregion

        //Constructor
        public PersonalInfo()
        {
            for (int i = 0; i < 8; i++)
            {
                checkNumbers[i] = (byte)Math.Pow(2, i); //values anded/ored with hasProblems, each index being the next bit starting at the LSB
                if (numValidatedTextboxes > i)
                {
                    hasProblems += checkNumbers[i]; //each field starts off empty, so all invalid.
                }
            }
            
        }

        

        
        



        public void UpdateFromCurrentRadioButtonValues()
        {
        }

    #region Validation methods

        public string Error
        {
            get
            {
                return null;
            }
        }

        public string this[string name]
        {
            get
            {
                string result = null;
                switch (name)
                {
                    case "Age":
                    {
                        if (this.age < 0 || this.age > 150)
                        {
                            hasProblems |= checkNumbers[0]; //set bit to 1
                            result = "Age must not be less than 0 or greater than 150.";
                        }
                        else if (this.age != null)
                        {
                            hasProblems &= (byte)~checkNumbers[0]; //clear bit
                        }
                        break;
                    }
                    case "Name":
                    {
                        if (this.name != null && this.name.Length < 3)
                        {
                            hasProblems |= checkNumbers[1]; //set bit to 1
                            result = "Please enter your full name.";
                        }
                        else if (this.name != null)
                        {
                            hasProblems &= (byte)~checkNumbers[1]; //clear bit
                        }
                        break;
                    }
                    case "Dept":
                    {
                        if (this.dept != null && this.dept.Length < 2)
                        {
                            hasProblems |= checkNumbers[2]; //set bit to 1
                            result = "Please enter your department name (or type 'N/A' if you do not work for RIM).";
                        }
                        else if (this.dept != null)
                        {
                            hasProblems &= (byte)~checkNumbers[2]; //clear bit
                        }
                        break;
                    }
                    default:
                    break;
                }
                return result;
            }
        }

    #endregion

    }
}
