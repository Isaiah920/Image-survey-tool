using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace SurveyTool
{
    public enum GenderEnum
    {
        Male,
        Female,
        Undisclosed
    }
    public enum VisionProblemsEnum
    {
        No,
        Yes
    }
    public enum ExperienceEnum
    {
        Novice,
        Moderate,
        Expert
    }

    public class PersonalInfo : IDataErrorInfo, ISerializable
    {

        #region Fields

        //updated in realtime using binding
        
        //validated:
        private int? age;
        private string name;
        private string dept;
        private int numValidatedTextboxes = 3;

        //bound using RadioButtonEnumConverter as a dataconverter:
        private int gender;
        private int experience;
        private bool vision;

        //keep track of the problems we saw when validating:
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
        public string OtherInfo 
        { 
            get; 
            set; 
        }
        public string VisionProblemsDescription { get; set; }

        public GenderEnum Gender { get; set; }
        public VisionProblemsEnum VisionProblems { get; set; }
        public ExperienceEnum Experience { get; set; }

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

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Name", name);
            info.AddValue("Dept", dept);
            info.AddValue("OtherInfo", OtherInfo);
            info.AddValue("Age", age);
            info.AddValue("Gender", gender);
            info.AddValue("VisionProblems", vision);
            if (vision) info.AddValue("VisionProblemsDescription", VisionProblemsDescription);
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
