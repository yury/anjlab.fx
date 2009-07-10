using System.Web.UI.WebControls;

namespace AnjLab.FX.Web.Controls
{
    public class DateValidator : RegularExpressionValidator
    {
        public DateValidator()
        {
            ValidationExpression = @"\d\d\.\d\d\.\d\d\d\d";
        }
    }
}
