using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonPRNG.LCG32
{
    public interface ICriteria<in T>
    {
        bool CheckConditions(T input);
    }

    public static class Criteria
    {
        public static ICriteria<T> OR<T>(params ICriteria<T>[] criterias) => criterias.Length == 1 ? criterias[0] : new OR<T>(criterias);
        public static ICriteria<T> AND<T>(params ICriteria<T>[] criterias) => criterias.Length == 1 ? criterias[0] : new AND<T>(criterias);
    }

    sealed class OR<T> : ICriteria<T>
    {
        private readonly ICriteria<T>[] criterias;
        public bool CheckConditions(T item)
            => criterias.Any(c => c.CheckConditions(item));

        public OR(ICriteria<T>[] criterias) => this.criterias = criterias;
    }
    sealed class AND<T> : ICriteria<T>
    {
        private readonly ICriteria<T>[] criterias;
        public bool CheckConditions(T item)
            => criterias.All(c => c.CheckConditions(item));

        public AND(ICriteria<T>[] criterias) => this.criterias = criterias;
    }

}
