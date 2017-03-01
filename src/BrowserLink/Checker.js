/// <reference path="_intellisense/browserlink.intellisense.js" />

(function (browserLink, $) {
    /// <param name="browserLink" value="bl" />
    /// <param name="$" value="jQuery" />

    var project;

    function runAxe(results) {

        results.project = project;

        for (var i = 0; i < results.violations.length; i++) {

            var nodes = results.violations[i].nodes;

            if (!nodes || nodes.length === 0)
                continue;

            results.violations[i].html = nodes[0].html;

            var target = nodes[0].target[0];
            var element = document.querySelector(target);

            var sourcemap = getSourceMap(element);
            results.violations[i].fileName = sourcemap ? sourcemap.sourcePath : "";
            results.violations[i].position = sourcemap ? sourcemap.startPosition : -1;
        }

        browserLink.invoke("ProcessResult", JSON.stringify(results));
    }

    function getSourceMap(element) {
        try {
            return browserLink.sourceMapping.getCompleteRange(element);
        } catch (e) {
            return null;
        }
    }

    function check(options, projectName) {

        var json = JSON.parse(options);
        project = projectName;

        setTimeout(function () {
            axe.a11yCheck(document, json, runAxe);
        }, 2000);
    }

    //[axe.min.js]

    return {
        check: check
    };
});