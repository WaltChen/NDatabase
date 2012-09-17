using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Core.Query.Criteria
{
    
    public sealed class IsNotNullCriterion : AbstractCriterion
    {
        public IsNotNullCriterion(string attributeName) : base(attributeName)
        {
        }

        public override bool Match(object valueToMatch)
        {
            // If it is a AttributeValuesMap, then gets the real value from the map
            if (valueToMatch is AttributeValuesMap)
            {
                var attributeValues = (AttributeValuesMap) valueToMatch;
                valueToMatch = attributeValues[AttributeName];
            }
            return valueToMatch != null;
        }

        public override AttributeValuesMap GetValues()
        {
            return new AttributeValuesMap();
        }

        public override void Ready()
        {
        }
    }
}