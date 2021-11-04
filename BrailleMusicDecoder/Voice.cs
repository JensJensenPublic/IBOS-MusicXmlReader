using System;
using System.Collections.Generic;
using System.Text;
using MusicXmlReaderModel;

namespace BrailleMusicDecoder
{
    /// <summary>
    /// Convenience class for the TypeAmbiguityHandler
    /// </summary>
    class Voice
    {
        public enum VoiceCategoryEnum
        { 
            Division0,  // Occured before a MeasureDivision mark 
            Division1, // Occured after the first MeasureDivision mark
            Division2, // Occured after the second MeasureDivision mark
            full,  // Occurred immediately after a LullMeasureInAccord mark
            numberOfCategories
        }

        static public VoiceCategoryEnum GetNextCategory(VoiceCategoryEnum currentCategory)
        {
            switch (currentCategory)
            {
                case VoiceCategoryEnum.Division0: return VoiceCategoryEnum.Division1;
                case VoiceCategoryEnum.Division1: return VoiceCategoryEnum.Division2;
                default: throw new Exception("");
            }
        }

        private VoiceCategoryEnum voiceCategory = VoiceCategoryEnum.Division0;
        public VoiceCategoryEnum VoiceCategory { get { return voiceCategory; } }

        private List<InputInterpretation> inputInterpretations = new List<InputInterpretation>();
        public List<InputInterpretation> InputInterpretations { get { return inputInterpretations; } }

        private int voiceNumber = 0;
        public int VoiceNumber { get { return voiceNumber; } }

        private int expectedMeasureDuration;
        public int ExpectedMeasureDuration { get { return expectedMeasureDuration; } }


        private List<InputInterpretation> owningMeasure;
        public List<InputInterpretation> OwningMeasure { get { return owningMeasure; } }

        public InputCategoryEnum CategoriesRepresentedInMeasure()
        {
            InputCategoryEnum result = 0;
            foreach (InputInterpretation ii in owningMeasure)
            {
                result |= ii.Category; 
            }
            return result;
        }

        public string ToCategoryString()
        {
            switch (this.VoiceCategory)
            {
                case VoiceCategoryEnum.full: return "Full";
                case VoiceCategoryEnum.Division0: return "Division0";
                case VoiceCategoryEnum.Division1: return "Division1";
                case VoiceCategoryEnum.Division2: return "Division2";
                default: throw (new Exception(""));
            }
        }


        /// <summary>
        /// Endings of repeats unfortunately do not have their own Category, so we must report them separately, taking the subCategory in account also: 
        /// </summary>
        /// <returns>Return true iff the owning measure contains an ending or a graphic repeat (a double bar preceeded by dots)</returns>
        public bool OwningMeasureContainsEndings()
        {
            foreach (InputInterpretation ii in owningMeasure)
            {
                if (InputCategoryEnum.OtherValues != ii.Category) continue;
                if ((InputSubCategoryEnum.OthervaluesVolta1FirstEnding == ii.SubCategory)
                    || (InputSubCategoryEnum.OthervaluesVolta2SecondEnding == ii.SubCategory)
                    || (InputSubCategoryEnum.OthervaluesVoltaIntervalEnding == ii.SubCategory)
                    || (InputSubCategoryEnum.OthervaluesVoltaNumericEnding == ii.SubCategory)
                    || (InputSubCategoryEnum.OthervaluesDoubleBarPrecededByDots == ii.SubCategory))
                    return true;
            }
            return false;
        }
   

        public void InsertAtEnd(List<InputInterpretation> inputInterpretations, bool clone)
        {
            foreach (InputInterpretation item in inputInterpretations)
            {
                this.inputInterpretations.Add(clone ? item.CloneOfTimingValues() : item);
            }
        }

        public void InsertAtStart(List<InputInterpretation> inputInterpretations, bool clone)
        {
            // We can not insert at start one by one, items would be inverted !
            List<InputInterpretation> itemList = new List<InputInterpretation>();
            foreach (InputInterpretation item in inputInterpretations)
            {
                itemList.Add(clone ?  item.CloneOfTimingValues() : item);
            }
            this.inputInterpretations.InsertRange(0, itemList);
        }
        
        public void Add(InputInterpretation inputInterpretation)
        {
            inputInterpretations.Add(inputInterpretation);
        }

        private Voice()
        { }

        private Voice(int voiceNumber, int expectedMeasureDuration, List<InputInterpretation> owningMeasure,VoiceCategoryEnum voiceCategory)
        {
            this.voiceNumber = voiceNumber;
            this.expectedMeasureDuration = expectedMeasureDuration;
            this.owningMeasure = owningMeasure;
            this.voiceCategory = voiceCategory;
        }

        public static Voice Create(int voiceNumber, int expectedMeasureDuration, List<InputInterpretation> owningMeasure)
        {
            return new Voice(voiceNumber, expectedMeasureDuration, owningMeasure, VoiceCategoryEnum.Division0);
        }

        public static Voice Create(int voiceNumber, int expectedMeasureDuration, List<InputInterpretation> owningMeasure,VoiceCategoryEnum voiceCategory)
        {
            return new Voice(voiceNumber, expectedMeasureDuration, owningMeasure, voiceCategory);
        }

        /// <summary>
        /// Generate a comprehensive stringrepresentation to be used for debugging
        /// </summary>
        /// <returns></returns>
        public string ToShortDebugString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (InputInterpretation i in this.inputInterpretations)
            {
                sb.Append(i.ToShortDebugString()+ " ");
            }
            return sb.ToString();
        }
    }

}
