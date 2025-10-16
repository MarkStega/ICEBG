// Because of invocation of setTabsChangeEvent in AfterRenderAsync the first change is not detected. The workaround is to
// lookup the first panel by name
export function setTabsChangeEvent(tabsID, firstPanelAriaControlledBy) {
    const firstPanel = firstPanelAriaControlledBy;
    const tabsElement = document.getElementById(tabsID);
    if (tabsElement != null) {
        console.log("Adding listener for change events");
        let currentPanel = null;
        tabsElement.addEventListener('change', () => {
            const root = tabsElement.getRootNode();
            if (currentPanel) {
                //console.log("currentPanel is not null, direct hide");
                currentPanel.hidden = true;
            }
            else {
                //console.log("currentPanel is null, looking up first panel by '" + firstPanel + "'");
                currentPanel = root.querySelector(`#${firstPanel}`);
                if (currentPanel) {
                    currentPanel.hidden = true;
                }
            }
            ;
            const panelId = tabsElement.activeTab?.getAttribute('aria-controls');
            currentPanel = root.querySelector(`#${panelId}`);
            if (currentPanel) {
                currentPanel.hidden = false;
            }
        });
    }
    ;
}
