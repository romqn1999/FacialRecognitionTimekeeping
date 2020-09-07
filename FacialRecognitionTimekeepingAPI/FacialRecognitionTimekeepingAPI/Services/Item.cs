using System.Threading.Tasks;

namespace FacialRecognitionTimekeepingAPI.Services
{
    public class Item<TStepIn, TPipeOut>
    {
        public TStepIn Input { get; set; }
        public TaskCompletionSource<TPipeOut> TaskCompletionSource { get; set; }
    }
}