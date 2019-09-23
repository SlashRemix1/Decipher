using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akorade.Types
{
    public abstract class Sprite<T> : Sprite where T : Sprite<T>
    {
        protected Sprite()
        {
        }

        public abstract T CopyFrom(T value);
    }
}
