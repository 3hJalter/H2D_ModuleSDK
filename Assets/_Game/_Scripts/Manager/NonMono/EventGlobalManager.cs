using HoangHH.DesignPattern;
using Sigtrap.Relays;
using UnityEngine;

namespace HoangHH.Manager
{
    [DefaultExecutionOrder(-200)]
    public class EventGlobalManager : HNonMonoSingleton<EventGlobalManager>
    {
        public Relay OnEverySecondTick { get; } = new();
    }
}
