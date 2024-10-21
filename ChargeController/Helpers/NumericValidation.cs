using System;
using System.Globalization;
using System.Windows.Controls;

namespace ChargeController.Helpers
{
    public class NumericValidation : ValidationRule
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public NumericValidation()
        {
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int age = 0;

            try
            {
                if (((string)value).Length > 0)
                    age = Int32.Parse((String)value);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"Illegal characters - Numbers only");
            }

            if ((age < Min) || (age > Max))
            {
                return new ValidationResult(false, $"Enter value from {Min} to {Max}.");
            }
            return ValidationResult.ValidResult;
        }
    }
}
