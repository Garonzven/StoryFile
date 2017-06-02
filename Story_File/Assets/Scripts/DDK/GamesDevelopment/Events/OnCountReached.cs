using DDK.Base.Events.States;
using DDK.Base.Classes;
using DDK.Base.Extensions;


namespace DDK.GamesDevelopment.Events
{
    /// <summary>
    /// Handles firing events while counting towards
    /// certain target. Useful for games where the user
    /// has to complete task which are countable.
    /// <example>
    /// Like clicking on something a certain number of times
    /// </example>
    /// </summary>
    /// <seealso cref="FinalState" />
    public class OnCountReached : FinalState
    {
        [ReadOnly()]
        public int counter = 0;

        public int target;

        public DelayedAction[] onIncrement;
        public DelayedAction[] onDecrement;
        public DelayedAction[] onCounterSet;
        public DelayedAction[] onTargetReached;

		[DisplayNameAttribute("Set Game.ended On Target Reached")]
        public bool setGameEndOnTarget = false;

        // Use this for initialization
        void Start ()
        {
        }

        public void Increment()
        {
            Increment ( 1 );
        }

        public void Decrement()
        {
            Decrement ( 1 );
        }

        /// <summary>
        /// Increments <see cref="counter"/> plus value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Increment ( int value )
        {
            Set ( counter + value );
            onIncrement.InvokeAll();
        }

        /// <summary>
        /// Decrements <see cref="counter"/> minus value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Decrement ( int value )
        {
            Set ( counter - value );
            onDecrement.InvokeAll();
        }

        /// <summary>
        /// Sets the <see cref="counter"/> equal to value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Set ( int value )
        {
            counter = value;
            onCounterSet.InvokeAll();
            _CheckStatus();
        }

        /// <summary>
        /// Resets the counter
        /// </summary>
        /// <param name="noEvents">
        /// if set to <c>true</c> [no events will be called].
        /// </param>
        public void Reset ( bool noEvents )
        {
            if ( !noEvents )
            {
                counter = 0;
            }
            else
            {
                Set ( 0 );
            }
        }

        /// <summary>
        /// Checks the counter status respective to the target
        /// </summary>
        private void _CheckStatus()
        {
            if ( counter != target )
            {
                return;
            }

            if ( setGameEndOnTarget )
            {
                Game.ended = true;
            }

            onTargetReached.InvokeAll();
            _FinalStateAction();
        }
    }
}
