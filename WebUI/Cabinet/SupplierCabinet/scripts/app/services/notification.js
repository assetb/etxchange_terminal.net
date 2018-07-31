define(["angular", "alta-api"], function (ng) {
    var notificationModule = ng.module("NotificationModule", ["alta.service.api"]);

    function guid() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
              .toString(16)
              .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
          s4() + '-' + s4() + s4() + s4();
    }

    notificationModule.controller("NotificationCtrl", ["$scope", "$window", "altaApi", function ($scope, $window, altaApi) {
        $scope.messages = [];

        function saveNotifications(notifications) {
            localStorage.setItem("notifications", ng.toJson(notifications));
        };

        function loadNotifications() {
            return ng.fromJson(localStorage.getItem("notifications"));
        };

        $scope.update = function () {
            altaApi.notification.query({}, function (messages) {

                ng.forEach(messages, function (message) {
                    ng.forEach($scope.messages, function (messageScope) {
                        if (messageScope.id == message.id)
                            message._read = messageScope._read;
                    });
                })

                $scope.messages = messages;
                saveNotifications($scope.messages);
            }, function () {
                $scope.messages = null;
                saveNotifications($scope.messages);
                notificationModule.showErrApplication();
            });
        };

        $scope.read = function (notification) {
            notification._read = true;
            saveNotifications($scope.messages);
        }

        $scope.init = function () {
            $("#main_notification_container").removeClass("hide");
            console.log("module notification initialize");

            $scope.messages = loadNotifications();
            if (!$scope.messages)
                $scope.messages = [];
            $scope.update();
        }

        console.log("module notification loaded");
    }]);

    var MESSAGE_CONTAINER_ID = "#message_container";
    var DEFAULT_TIMEOUT = 3000;

    var templateMessage = "<div id=\"MESSAGE_ID\"  style=\"display: none;\" data-closable class=\"callout alert-box alert-callout-subtle TYPE\">" +
                "<strong>TITLE</strong> MESSAGE" +
            "</div>";

    notificationModule.types = {
        SUCCESS: "success",
        WARNING: "alert"
    }

    notificationModule.removeMessage = function (messageId) {
        var element = $("#" + messageId);

        element.hide(500, function () {
            element.detach();
        });
    }

    notificationModule.showErrApplication = function (status, timeout) {
        switch (status) {
            case (403): case (401): {
                notificationModule.showWarn("Доступ запрещен.", "Ошибка!", timeout);
            } break;
            case (404): {
                notificationModule.showWarn("Ресурс не найден.", "Ошибка!", timeout);
            } break;
            default: {
                notificationModule.showWarn("Внутреняя ошибка программы. Пожалуйста повторите попытку или обратитесь к брокеру", "Ошибка!", timeout);
            }
        }
    }

    notificationModule.showInfo = function (message, title, timeout) {
        return notificationModule.showMessage(notificationModule.types.SUCCESS, title ? title : "Успешно.", message, timeout);
    }

    notificationModule.showWarn = function (message, title, timeout) {
        return notificationModule.showMessage(notificationModule.types.WARNING, title ? title : "Внимание!", message, timeout);
    }

    notificationModule.showMessage = function (type, title, message, timeout) {
        var elementStr = templateMessage
            .replace("MESSAGE_ID", guid())
            .replace("TYPE", type)
            .replace("TITLE", title)
            .replace("MESSAGE", message);
        var element = $(elementStr);

        var elementId = element.attr("id");

        $(MESSAGE_CONTAINER_ID + " > div").append(element);

        element.show(500);

        setTimeout(function () {
            notificationModule.removeMessage(elementId);
        }, timeout != null ? timeout : DEFAULT_TIMEOUT);

        return elementId;
    }

    window.notification = notificationModule;

    $(document).ready(function () {
        var br = $.browser;
        var messageContainer = $(MESSAGE_CONTAINER_ID);
        var topPosition = messageContainer.offset().top;

        $(window).scroll(function () {
            var top = $(document).scrollTop();
            if (top < topPosition) {
                messageContainer.css({ position: 'relative' });
            } else {
                messageContainer.css({ top: '10px', position: 'fixed' });
            }
        });
    });

    return notificationModule;
});