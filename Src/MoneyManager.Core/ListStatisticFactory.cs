﻿using System;
using System.Collections.Generic;
using MoneyManager.Core.StatisticProvider;
using MoneyManager.Foundation;
using MoneyManager.Foundation.Interfaces;
using MoneyManager.Foundation.Model;

namespace MoneyManager.Core
{
    public class ListStatisticFactory
    {
        private readonly IRepository<Category> categoryRepository;
        private readonly ITransactionRepository transactionRepository;

        public ListStatisticFactory(ITransactionRepository transactionRepository,
            IRepository<Category> categoryRepository)
        {
            this.transactionRepository = transactionRepository;
            this.categoryRepository = categoryRepository;
        }

        /// <summary>
        ///     Creates an Statistic Data Provider for a statistic who uses lists. For example the Category Spreading Statistic
        /// </summary>
        /// <param name="type">Type of the statistic</param>
        /// <returns>Instance of the Statistic Data Provider.</returns>
        public IStatisticProvider<IEnumerable<StatisticItem>> CreateListProvider(ListStatisticType type)
        {
            switch (type)
            {
                case ListStatisticType.CategorySpreading:
                    return new CategorySpreadingProvider(transactionRepository, categoryRepository);

                case ListStatisticType.CategorySummary:
                    return new CategorySummaryProvider(transactionRepository, categoryRepository);

                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}