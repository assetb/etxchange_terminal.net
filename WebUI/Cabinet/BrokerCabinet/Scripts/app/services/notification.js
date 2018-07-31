(function (win) {
    if (!win)
        win = {};

    function guid() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
              .toString(16)
              .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
          s4() + '-' + s4() + s4() + s4();
    };

    function createMessageNode(title, message, type, id) {
        var messageNode = document.createElement("div");

        messageNode.setAttribute("style", "display: none;");
        messageNode.setAttribute("data-closable", "");
        messageNode.setAttribute("class", "callout alert-box alert-callout-subtle " + type);
        messageNode.id = id ? id : guid();

        var messageTitle = document.createElement("b");
        messageTitle.innerText = title;

        messageNode.appendChild(messageTitle);
        messageNode.innerText += " "+ message;

        return messageNode;
    }

    function close(selector, container, durationAnimate) {
        $(selector, container).hide(durationAnimate, function (e) {
            selector.detach();
        });
    }

    var notification = {
        container: null,
        settings: {
            duration: 3000,
            durationAnimate: 400,
        },
        init: function (selector, settings) {
            this.container = selector;
            this.settings = Object.assign({}, this.settings, settings);

            var container = this.container;

            container.addClass("container-padded row align-center");

            var br = $.browser;
            var topPosition = container.offset().top;

            $(window).scroll(function () {
                var top = $(document).scrollTop();
                if (top < topPosition) {
                    container.addClass("row");
                    container.css({ "position": "relative", "width": "inherit" });
                } else {
                    container.removeClass("row");
                    container.css({ "top": "10px", "position": "fixed", "width": "350px", "z-index": "100" });
                }
            });
        },
        show: function (title, message, timeout, type, id) {
            var container = this.container;
            var durationAnimate = this.settings.durationAnimate;
            var duration = timeout ? timeout : this.settings.duration;

            var messageNode = createMessageNode(title, message, type, id);

            var element = $(messageNode);

            container.append(element);
            element.show(durationAnimate, function () {
                if (duration > 0) {
                    setTimeout(function () {
                        close(element, container, durationAnimate);
                    }, duration);
                }
            });
        },
        success: function (title, message, timeout) {
            this.show(title, message, timeout, "success");
        },
        alert: function (title, message, timeout) {
            this.show(title, message, timeout, "alert");
        }
    };

    win.notification = notification;
})(window)