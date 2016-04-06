/// <reference path="_intellisense/browserlink.intellisense.js" />

(function (browserLink, $) {
    /// <param name="browserLink" value="bl" />
    /// <param name="$" value="jQuery" />

    function check() {
        alert("Hello from Web Accessibility Checker!");

        var result = {
            violations: [
                {
                    help: "Error message",
                    helpUrl: "http://example.com",
                    id: "rule-id"
                }
            ]
        };

        browserLink.invoke("ProcessResult", JSON.stringify(result));
    }

    return {
        onConnection: check
    };
});