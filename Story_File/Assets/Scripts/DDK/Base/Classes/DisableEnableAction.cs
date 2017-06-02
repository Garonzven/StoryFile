using System;

namespace DDK.Base.Classes
{
    /// <summary>
    /// Generic class to be used as event for behaviors
    /// that need actions on disable / enable state,
    /// and after / before doing "something"
    /// </summary>
    [Serializable]
    public class DisableEnableAction
    {
        public DelayedAction[] onEnable;
        public DelayedAction[] onDisable;
        public DelayedAction[] beforeAction;
        public DelayedAction[] afterAction;
    }
}