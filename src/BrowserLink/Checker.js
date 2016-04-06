/// <reference path="_intellisense/browserlink.intellisense.js" />

(function (browserLink, $) {
    /// <param name="browserLink" value="bl" />
    /// <param name="$" value="jQuery" />

    function check() {
        axe.a11yCheck(document, function (results) {

            console.log(results);

            if (results.violations.length > 0) {

                for (var i = 0; i < results.violations.length; i++) {

                    var nodes = results.violations.nodes;

                    if (!nodes || nodes.length === 0)
                        continue;

                    var target = nodes[0].target[0];
                    var element = document.querySelector(target);

                    if (browserLink.sourceMapping.canMapToSource(element)) {
                        nodes[0].fileName = browserLink.sourceMapping.getCompleteRange(element).sourcePath;
                    }
                }

                browserLink.invoke("ProcessResult", JSON.stringify(results));
            }
        });
    }

    //[axe.js]

    return {
        onConnected: check
    };
});