using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;

namespace AnjLab.FX.Net.Behaviors
{
    public class HandleErrorsBehaviorExtensionElement : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new HandleErrorsBehavior();
        }

        public override Type BehaviorType
        {
            get
            {
                return typeof(HandleErrorsBehavior);
            }
        }
    }
}