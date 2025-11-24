using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace TidyCooking.Levels
{
    public interface ICookModule
    {
        Task SetUp();
        Task StartModule(System.Action<ICookModule> onEnd);
    }
}