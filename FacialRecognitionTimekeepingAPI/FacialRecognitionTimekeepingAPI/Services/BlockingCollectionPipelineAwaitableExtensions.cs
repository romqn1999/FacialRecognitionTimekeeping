using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacialRecognitionTimekeepingAPI.Services
{
    public static class BlockingCollectionPipelineAwaitableExtensions
    {
        /// <summary>
        /// Generate a step take input of type <typeparamref name="TInput"/> execute by <paramref name="stepAction"/> then add this step to end of <paramref name="pipelineBuilder"/>
        /// </summary>
        /// <typeparam name="TInput">Type of input for step action</typeparam>
        /// <typeparam name="TOutput">Type of output for step action</typeparam>
        /// <typeparam name="TInputOuter">Type of input for pipeline</typeparam>
        /// <typeparam name="TOutputOuter">Type of output for pipeline</typeparam>
        /// <param name="inputType">Type of input for step action</param>
        /// <param name="pipelineBuilder">The pipeline builder</param>
        /// <param name="stepAction">The step action</param>
        /// <returns>Return default value of type <typeparamref name="TOutput"/></returns>
        public static TOutput AddStep<TInput, TOutput, TInputOuter, TOutputOuter>(this TInput inputType,
            BlockingCollectionPipelineAwaitable<TInputOuter, TOutputOuter> pipelineBuilder,
            Func<TInput, TOutput> stepAction)
        {
            var pipelineStep = pipelineBuilder.GenerateStep<TInput, TOutput>();
            pipelineStep.StepAction = stepAction;
            return default(TOutput);
        }
    }
}
