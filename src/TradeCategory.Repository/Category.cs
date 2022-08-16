﻿using System.Diagnostics;
using TradeCategory.Repository.Strategy;
using System.Linq;

namespace TradeCategory.Repository
{
    public class Category
    {
        private DateTime referenceDate { get; set; }

        List<Trade> trades;

        public Category()
        {
            trades = new List<Trade>();
        }

        public void SetTrade(double value, string clientSector, DateTime nextPaymentDate, DateTime referenceDate)
        {
            this.referenceDate = referenceDate;

            Trade trade = new();
            trade.Value = value;
            trade.ClientSector = clientSector;
            trade.NextPaymentDate = nextPaymentDate;
            trades.Add(trade);
        }

        public List<string> GetCategory()
        {
            RuleExpired ruleExpired = new RuleExpired(referenceDate);
            RuleHighRisk ruleHighRisk = new RuleHighRisk();
            RuleMediumRisk ruleMediumRisk = new RuleMediumRisk();
            RuleNone ruleNone = new RuleNone();

            List<ICategoryRule> categoryRules = new()
            {
                ruleExpired,
                ruleHighRisk,
                ruleMediumRisk,
                ruleNone
            };

            List<string> categories = new();

            try
            {
                foreach (var categoryRule in from trade in trades
                                             from categoryRule in categoryRules
                                             where categoryRule.Verify(trade)
                                             select categoryRule)
                {
                    categories.Add(categoryRule.NameCategory);
                    break;
                }

                return categories;
            }
            catch
            {
                throw;
            }
        }
    }
}
