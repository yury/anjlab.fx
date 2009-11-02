using System.Web.UI.WebControls;

namespace AnjLab.FX.Web.Controls
{
    public class PriceValidator : RegularExpressionValidator
    {
        private readonly string DefaulRegEx = @"\d+((\.|,)\d+)?";
        private readonly string DefaulRegExAndNegative = @"(-)?\d+((\.|,)\d+)?";

        public PriceValidator()
        {
            ValidationExpression = DefaulRegEx;
        }

        public bool AllowNegative
        {
            set
            {
                ValidationExpression = (value) ? DefaulRegExAndNegative : DefaulRegEx;
            }
        }
    }
}
