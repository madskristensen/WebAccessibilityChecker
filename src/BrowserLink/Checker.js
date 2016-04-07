/// <reference path="_intellisense/browserlink.intellisense.js" />

(function (browserLink, $) {
    /// <param name="browserLink" value="bl" />
    /// <param name="$" value="jQuery" />

    function check() {
        var options = {
            "rules": {
                "region": { "enabled": false }
            }
        }

        axe.a11yCheck(document, options, function (results) {

            if (results.violations.length > 0) {

                for (var i = 0; i < results.violations.length; i++) {

                    var nodes = results.violations[i].nodes;

                    if (!nodes || nodes.length === 0)
                        continue;

                    results.violations[i].html = nodes[0].html;

                    var target = nodes[0].target[0];
                    var element = document.querySelector(target);
                    var hasSourceMap = browserLink.sourceMapping.canMapToSource(element);

                    if (hasSourceMap) {
                        var sourcemap = browserLink.sourceMapping.getCompleteRange(element);
                        results.violations[i].fileName = sourcemap.sourcePath;
                        results.violations[i].position = sourcemap.startPosition
                    }
                }

                console.log(results);

                browserLink.invoke("ProcessResult", JSON.stringify(results));
            }
        });
    }

    //[axe.js]

    return {
        onConnected: function () {
            setTimeout(check, 3000);
        }
    };
});