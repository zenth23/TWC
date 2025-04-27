using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TWC.IMS.Models.HelperClasses
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequiredIfAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly RequiredAttribute _hiddenAttribute;
        private object DesiredValue { get; set; }
        private string PropertyName { get; set; }

        /// <summary>
        /// Sample usage: [RequiredIf("IsActive", true)]
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="desiredValue"></param>
        public RequiredIfAttribute(string propertyName, object desiredValue)
        {
            this.PropertyName = propertyName;
            this.DesiredValue = desiredValue;
            _hiddenAttribute = new RequiredAttribute();
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "required"
            };

            rule.ValidationParameters["dependentproperty"] = (context as ViewContext).ViewData.TemplateInfo.GetFullHtmlFieldId(this.PropertyName);
            rule.ValidationParameters["desiredvalue"] = DesiredValue is bool ? DesiredValue.ToString().ToLower() : DesiredValue;

            yield return rule;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dependentValue = validationContext.ObjectInstance.GetType().GetProperty(PropertyName).GetValue(validationContext.ObjectInstance, null);
            if (dependentValue.ToString() == DesiredValue.ToString())
            {
                if (!_hiddenAttribute.IsValid(value))
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new[] { validationContext.MemberName });
                }
            }
            return ValidationResult.Success;
        }
    }
}