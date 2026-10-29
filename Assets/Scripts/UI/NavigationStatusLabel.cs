using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class NavigationStatusLabel : MonoBehaviour
{
    static readonly string Prompt = "Tap Start here, then choose a room";
    static readonly string RoomSelected = "Room selected. Tap Start here";
    static readonly string ChooseRoom = "Placed. Choose a room";
    static readonly string Navigating = "Navigating";
    static readonly string Lost = "Tracking lost";

    [SerializeField] GraphAligner aligner;
    [SerializeField] NavigationController navigation;
    [SerializeField] TMP_Text label;

    string shown;

    void OnEnable()
    {
        shown = null;
        Apply();
    }

    void Update()
    {
        Apply();
    }

    void Apply()
    {
        if (label == null)
            return;

        bool routeVisible = aligner != null && aligner.IsAligned
            && navigation != null && navigation.RouteLengthMeters >= 0f;
        ARSessionState state = ARSession.state;

        string next;
        if (routeVisible)
            next = Navigating;
        else if (aligner != null && aligner.IsAligned)
            next = ChooseRoom;
        else if (navigation != null && !string.IsNullOrEmpty(navigation.DestinationId))
            next = RoomSelected;
        else if (state == ARSessionState.None
            || state == ARSessionState.CheckingAvailability
            || state == ARSessionState.NeedsInstall
            || state == ARSessionState.Installing
            || state == ARSessionState.Ready
            || state == ARSessionState.SessionInitializing
            || state == ARSessionState.SessionTracking)
            next = Prompt;
        else
            next = Lost;

        if (ReferenceEquals(shown, next))
            return;

        shown = next;
        label.text = next;
    }
}
