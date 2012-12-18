using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Tool.Wrappers.List;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    internal abstract class AbstractExpression : IConstraint
    {
        private readonly IQuery _query;

        protected AbstractExpression(IQuery query)
        {
            _query = query;
            ((IInternalQuery)_query).Join(this);
        }

        #region IExpression Members

        public virtual bool CanUseIndex()
        {
            return false;
        }

        public abstract IOdbList<string> GetAllInvolvedFields();

        public abstract AttributeValuesMap GetValues();

        public abstract bool Match(object arg1);

        public virtual void Ready()
        {
        }

        public IConstraint And(IConstraint with)
        {
            return new And(_query).Add(this).Add(with);
        }

        public IConstraint Or(IConstraint with)
        {
            return new Or(_query).Add(this).Add(with);
        }

        public IConstraint Not()
        {
            return new Not(_query, this);
        }

        #endregion
    }
}
