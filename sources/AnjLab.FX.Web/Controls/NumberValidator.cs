using System.Web.UI.WebControls;

namespace AnjLab.FX.Web.Controls
{
    public class NumberValidator : RegularExpressionValidator
    {
        public NumberValidator()
        {
            ValidationExpression = @"\d+";
        }
    }
}
