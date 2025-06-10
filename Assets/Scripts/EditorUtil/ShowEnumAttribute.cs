
namespace DonutEditor
{
    using UnityEngine;

    public class ShowEnumAttribute : PropertyAttribute
    {
        public string enumFieldName;
        public object targetEnumValue;

        public ShowEnumAttribute(string enumFieldName, object targetEnumValue)
        {
            this.enumFieldName = enumFieldName;
            this.targetEnumValue = targetEnumValue;
        }
    }
}