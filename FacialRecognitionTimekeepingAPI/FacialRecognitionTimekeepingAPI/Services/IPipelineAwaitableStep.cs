using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacialRecognitionTimekeepingAPI.Services
{
    public interface IPipelineAwaitableStep<TStepIn, TPipeOut>
    {
        BlockingCollection<Item<TStepIn, TPipeOut>> Buffer { get; set; }
    }
}
