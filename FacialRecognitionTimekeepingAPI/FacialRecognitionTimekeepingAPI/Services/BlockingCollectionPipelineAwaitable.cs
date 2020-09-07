using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacialRecognitionTimekeepingAPI.Services
{
    public class BlockingCollectionPipelineAwaitable<TPipeIn, TPipeOut>
    {
        #region Private attribute
        List<object> _pipelineSteps = new List<object>();
        #endregion

        #region Constructor(s)
        /// <summary>
        /// Create an instance of pipeline from a steps builder
        /// </summary>
        /// <param name="stepsBuilder">
        /// Steps builder function that take 2 inputs. 
        /// First parameter is pipeline input, constructor will invoke the builder with value of this parameter is default value of <typeparamref name="TPipeIn"/>. 
        /// Second parameter is <see cref="BlockingCollectionPipelineAwaitable{TPipeIn, TPipeOut}"/> it self.
        /// </param>
        /// <example>
        /// </example>
        public BlockingCollectionPipelineAwaitable(Func<TPipeIn, BlockingCollectionPipelineAwaitable<TPipeIn, TPipeOut>, TPipeOut> stepsBuilder)
        {
            stepsBuilder?.Invoke(default(TPipeIn), this); //Invoke just once to build blocking collections
        }

        public BlockingCollectionPipelineAwaitable(Action<BlockingCollectionPipelineAwaitable<TPipeIn, TPipeOut>> stepsBuilder)
        {
            stepsBuilder?.Invoke(this); //Invoke just once to build blocking collections
        }
        #endregion

        #region Method(s)
        public Task<TPipeOut> Execute(TPipeIn input)
        {
            // Input of pipeline also the input for first step
            var first = _pipelineSteps[0] as IPipelineAwaitableStep<TPipeIn, TPipeOut>;
            TaskCompletionSource<TPipeOut> tsk = new TaskCompletionSource<TPipeOut>();
            first.Buffer.Add(/*input*/new Item<TPipeIn, TPipeOut>()
            {
                Input = input,
                TaskCompletionSource = tsk
            });
            return tsk.Task;
        }

        public BlockingCollectionPipelineAwaitableStep<TStepIn, TStepOut, TPipeOut> GenerateStep<TStepIn, TStepOut>()
        {
            var pipelineStep = new BlockingCollectionPipelineAwaitableStep<TStepIn, TStepOut, TPipeOut>();
            var stepIndex = _pipelineSteps.Count;

            Task.Run(() =>
            {
                // Output of this step will be input for next step.
                IPipelineAwaitableStep<TStepOut, TPipeOut> nextPipelineStep = null;

                foreach (var input in pipelineStep.Buffer.GetConsumingEnumerable())
                {
                    bool isLastStep = stepIndex == _pipelineSteps.Count - 1;
                    TStepOut output;
                    try
                    {
                        output = pipelineStep.StepAction(input.Input);
                    }
                    catch (Exception e)
                    {
                        input.TaskCompletionSource.SetException(e);
                        continue;
                    }
                    if (isLastStep)
                    {
                        input.TaskCompletionSource.SetResult((TPipeOut)(object)output);
                    }
                    else
                    {
                        if (isLastStep)
                        {
                            if (typeof(TStepOut) != typeof(TPipeOut))
                            {
                                throw new ArgumentException("Type of last step output and pipeline output not match");
                            }
                        }
                        nextPipelineStep = nextPipelineStep ?? (isLastStep ? null : _pipelineSteps[stepIndex + 1] as IPipelineAwaitableStep<TStepOut, TPipeOut>);
                        if (!isLastStep && nextPipelineStep is null)
                        {
                            throw new ArgumentException($"Cannot get next step of step {stepIndex}. May be type of step {stepIndex} output and step {stepIndex + 1} input not match");
                        }
                        nextPipelineStep.Buffer.Add(new Item<TStepOut, TPipeOut>() { Input = output, TaskCompletionSource = input.TaskCompletionSource });
                    }
                }
            });

            _pipelineSteps.Add(pipelineStep);
            return pipelineStep;

        }

        public BlockingCollectionPipelineAwaitable<TPipeIn, TPipeOut> AddStep<TStepIn, TStepOut>(Func<TStepIn, TStepOut> stepAction)
        {
            var pipelineStep = this.GenerateStep<TStepIn, TStepOut>();
            pipelineStep.StepAction = stepAction;
            return this;
        }
        #endregion
    }
}
