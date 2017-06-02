

using DDK.Base.Events.States;
using DDK.Base.Extensions;
using DDK.Base.Classes;
using DDK.Base.SoundFX;

using UnityEngine;

/// <summary>
/// Add extra parameters to the FinalState class to add extra events/functionality.
/// </summary>
public class StateAfter : FinalState
{
    public bool executeOnStart = false;
    public float delay = 1f;

    [Header ( "Before Delay Actions" )]
    public DelayedAction[] delayedActionsBefore;

    public Sfx.ActiveStates statesToSetBefore;

    [Header ( "After Delay Actions" )]
    public DelayedAction[] delayedActions;

    public Sfx.ActiveStates statesToSet;
    private bool _alreadySetStates;

    void Start()
    {
        if ( !executeOnStart )
        {
            return;
        }

        SetAllStates();
    }

    // Use this for initialization
    protected void OnEnable()
    {
        SetAllStates();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void _AfterDelay()
    {
        delayedActions.InvokeAll();
        statesToSet.events.Invoke();
        statesToSet.enable.SetEnabled ( true );
        statesToSet.disable.SetEnabled ( false );
        statesToSet.activate.SetActiveInHierarchy ( true );
        statesToSet.deactivate.SetActiveInHierarchy ( false );
        _FinalStateAction();
        _alreadySetStates = false;
    }

    public void SetAllStates()
    {
        if ( _alreadySetStates )
        {
            return;
        }

        _alreadySetStates = true;
        delayedActionsBefore.InvokeAll();
        statesToSetBefore.events.Invoke();
        statesToSetBefore.enable.SetEnabled ( true );
        statesToSetBefore.disable.SetEnabled ( false );
        statesToSetBefore.activate.SetActiveInHierarchy ( true );
        statesToSetBefore.deactivate.SetActiveInHierarchy ( false );
        Invoke ( "_AfterDelay", delay );
    }
}