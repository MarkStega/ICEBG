/*
** MW3 close-menu event arguments
**
** This must match the C# definition found in MBMenuEvents.cs
*/
export function eventArgsCreatorMenuSelectionReport(event) {
    var target = event.target;
    return {
        menuID: target.id,
        menuHeadline: target.typeaheadText,
        reason: JSON.stringify(event.detail.reason)
    };
}
/*
** Register all custom events
*/
export function afterStarted(blazor) {
    console.log("Registering menuselectionreport event");
    blazor.registerCustomEventType('menuselectionreport', {
        browserEventName: "close-menu",
        createEventArgs: eventArgsCreatorMenuSelectionReport
    });
}
