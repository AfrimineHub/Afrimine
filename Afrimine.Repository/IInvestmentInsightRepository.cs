using Afrimine.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afrimine.Repository
{
    public interface IInvestmentInsightRepository : IRepositoryBase<InvestmentInsight>
    {
        Task<IEnumerable<InvestmentInsight>> GetLatestAsync(int count = 10);
    }
}
