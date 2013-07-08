(function (window, undefined) {
    var 
        document = window.document,

        nugetButtonCssUrl = 'http://s.prabir.me/nuget-button/0.2.1/nuget-button.min.css',

        installPackageText = 'Install-Package',

        getElementsByClass = function (searchClass, node, tag) {
            var classElements = new Array();
            if (node == null)
                node = document;
            if (tag == null)
                tag = '*';
            var els = node.getElementsByTagName(tag);
            var elsLen = els.length;
            var pattern = new RegExp("(^|\\s)" + searchClass + "(\\s|$)");
            var i, j;
            for (i = 0, j = 0; i < elsLen; i++) {
                if (pattern.test(els[i].className)) {
                    classElements[j] = els[i];
                    j++;
                }
            }
            return classElements;
        },

        text = function (node, textContent) {
            var textNode = document.createTextNode(textContent);
            node.appendChild(textNode);
        },

        createElement = function (name) {
            return document.createElement(name);
        },

        getData = function (element, name) {
            return element.getAttribute('data-nugetbutton-' + name);
        },

        loadCss = function (cssUrl) {
            if (document.createStyleSheet) {
                document.createStyleSheet(cssUrl);
            }
            else {
                var styleSheet = createElement('link');
                styleSheet.setAttribute('rel', 'stylesheet');
                styleSheet.setAttribute('type', 'text/css');
                styleSheet.setAttribute('href', cssUrl);
                document.getElementsByTagName('head')[0].appendChild(styleSheet);
            }
        },
        appendOtherCharacters = function(element, array) {
            for(var index = 2; index < array.length; index++)
            {
                var str = ' ' + array[index];
                text(element, str);
            }
        },

        init = function () {
            var buttons = getElementsByClass('nuget-button', null, 'pre'),
                totalButtons = buttons.length;

            if (totalButtons.length == 0) {
                return;
            }

            loadCss(nugetButtonCssUrl);
            for (var i = 0; i < totalButtons; ++i) {
                var pre = buttons[i],
                    str = pre.innerHTML.split(' ');
                if (str[0] !== installPackageText) {
                    continue;
                }

                var 
                    link = getData(pre, 'link') || 'true',
                    packageName = str[1],
                    commandWrapper = createElement('div'),
                    commandPrompt = createElement('div'),
                    command = createElement('p'),
                    anchor = createElement('a');

                commandWrapper.className = 'nuget-button-commandWrapper';

                commandPrompt.className = 'nuget-button-commandPrompt';
                commandWrapper.appendChild(commandPrompt);

                command.className = 'nuget-button-command';
                text(command, "PM> " + installPackageText + ' ');
                if (link === 'false') {
                    text(command, packageName);
                } else {
                    anchor.setAttribute('href', link === 'true' ? 'http://nuget.org/List/Packages/' + packageName : link);
                    text(anchor, packageName);
                    command.appendChild(anchor);
                }
                appendOtherCharacters(command, str)
                commandPrompt.appendChild(command);
                pre.parentNode.replaceChild(commandWrapper, pre);
            }
        };

    init();

})(window);