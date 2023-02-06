using System;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace com.ivp.srm.common
{
    public class ValidationExtension : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new ValidationBehavior();
        }

        public override Type BehaviorType
        {
            get
            {
                return typeof(ValidationBehavior);
            }
        }
    }
}
