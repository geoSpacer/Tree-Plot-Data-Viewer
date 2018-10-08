// PSPDataViewer_scripts.js
// Keith Olsen - 16 August 2017
//
// For use with the Permanent Sample Plot Program Data website

function helpAlert(elID) {
    if (elID == "selectHelp") {
        alert("a) Select individual StandID's by checking boxes at left, or use the pull-down menu below to select StandID's by category.\n\nb) Within a category, select from options in the box below.\n\nc) Select download (CSV format) to transfer the stand descriptors file to your local computer.");
    } else if (elID == "showDataHelp") {
        alert("a) State variables: tree status, tree dbh, trees per hectare, basal area, stem volume, stem biomass (Means et al. 1994, Jenkins et al. 2003)\n\nb) Change variables: trees per hectare, basal area, volume, biomass, net primary productivity.\n\nc) Select download (CSV format) to transfer the file currently being viewed to your local computer.");
    } else if (elID == "graphDataHelp") {
        alert("a) Total stand summaries and NPP can be displayed for up to 5 stands. If more than 5 stands are selected, only the first 5 will be graphed.\n\nb) For graph summaries by species, only the first stand selected will be graphed.");
    }
}
