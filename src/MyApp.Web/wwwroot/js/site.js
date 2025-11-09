window.downloadFileFromStream = async (fileName, contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
};

window.removeGlobalClickHandler = function () {
    document.removeEventListener('click', window.globalClickHandler);
};

window.addGlobalClickHandler = function () {
    // Remove existing handler if any
    window.removeGlobalClickHandler();
    
    // Create and store the handler
    window.globalClickHandler = function (e) {
        const elementId = e.target.id || e.target.parentElement.id;
        DotNet.invokeMethodAsync('MyApp.Web', 'HandleGlobalClick', elementId);
    };
    
    // Add the handler
    document.addEventListener('click', window.globalClickHandler);
};
