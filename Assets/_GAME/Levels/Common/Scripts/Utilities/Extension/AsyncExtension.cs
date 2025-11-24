using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Utilities
{
    public static class AsyncExtension
    {
        public static void RunWithLogException(this Task task)
        {
            task.ContinueWith(OnAsyncMethodFailed, TaskContinuationOptions.OnlyOnFaulted);

            void OnAsyncMethodFailed(Task t)
            {
                Debug.LogException(t.Exception);
            }
        }
    }
}