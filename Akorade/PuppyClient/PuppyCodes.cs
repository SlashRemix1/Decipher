using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doumi.PuppyServer
{
    public enum PuppyClientCodes
    {
        RequestTranslation = 1,
        RequestEdit = 2,
    }

    public enum PuppyServerCodes
    {
        TranslationReponse = 1,
        EditResponse = 2,
    }
}
