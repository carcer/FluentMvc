namespace FluentMvc.ActionResultFactories
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Constraints;

    public interface IActionResultFactory
    {
        ActionResult Create(ActionResultSelector selector);
        bool ShouldBeReturnedFor(ActionResultSelector selector);
        void SetConstraints(IEnumerable<IConstraint> constraintRegistrations);
        IEnumerable<IConstraint> Constraints { get; }
    }
}