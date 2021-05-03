using System;
using System.Collections.Generic;
using System.Linq;
using Glue.Delivery.Core.Domain;

namespace Glue.Delivery.Core.Rules
{
    public sealed class StateRuleEngine : IStateRuleEngine
    {
        public StateRuleEngine(IEnumerable<IStateRule> rules)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        private readonly IEnumerable<IStateRule> _rules; 
        
        public (bool Allowed, DeliveryState? Target) ApplyAction(DeliveryAction action, ActionContext context)
        {
            var rule = _rules.FirstOrDefault(r => r.CanApply(action, context));
            return rule?.Apply(action, context) ?? (false, null);
        }
    }
}