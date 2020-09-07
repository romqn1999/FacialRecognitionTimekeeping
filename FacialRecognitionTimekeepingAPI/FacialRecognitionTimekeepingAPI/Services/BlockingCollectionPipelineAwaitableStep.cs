using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacialRecognitionTimekeepingAPI.Services
{
    public class BlockingCollectionPipelineAwaitableStep<TStepIn, TStepOut, TPipeOut> : IPipelineAwaitableStep<TStepIn, TPipeOut>
    {
        public BlockingCollection<Item<TStepIn, TPipeOut>> Buffer { get; set; } = new BlockingCollection<Item<TStepIn, TPipeOut>>();
        public Func<TStepIn, TStepOut> StepAction { get; set; }
    }
}
