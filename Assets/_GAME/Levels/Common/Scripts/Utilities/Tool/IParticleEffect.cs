using System.Collections;
using UnityEngine;

namespace Utilities
{
    public interface IParticleEffect
    {
        Transform Transform { get; }
        void Play();
        void Stop();
        void Clear();
    }
}