using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonPRNG.LCG32
{
    public interface ICriteria<T>
    {
        bool CheckConditions(T input);
    }
}
