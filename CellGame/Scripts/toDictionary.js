/*!
 * jQuery toDictionary() plugin
 *
 * Version 1.2 (11 Apr 2011)
 *
 * Copyright (c) 2011 Robert Koritnik
 * Licensed under the terms of the MIT license
 * http://www.opensource.org/licenses/mit-license.php
 */

(function ($) {
    if ($.isFunction(String.prototype.format) === false) {
        String.prototype.format = function () {
            var s = this;
            var i = arguments.length;
            while (i--) {
                s = s.replace(new RegExp("\\{" + i + "\\}", "gim"), arguments[i]);
            }
            return s;
        };
    }

    if ($.isFunction(Date.prototype.toISOString) === false) {
        Date.prototype.toISOString = function () {
            var pad = function (n, places) {
                n = n.toString();
                for (var i = n.length; i < places; i++) {
                    n = "0" + n;
                }
                return n;
            };
            var d = this;
            return "{0}-{1}-{2}T{3}:{4}:{5}.{6}Z".format(
                d.getUTCFullYear(),
                pad(d.getUTCMonth() + 1, 2),
                pad(d.getUTCDate(), 2),
                pad(d.getUTCHours(), 2),
                pad(d.getUTCMinutes(), 2),
                pad(d.getUTCSeconds(), 2),
                pad(d.getUTCMilliseconds(), 3)
            );
        };
    }

    var _flatten = function (input, output, prefix, includeNulls) {
        if ($.isPlainObject(input)) {
            for (var p in input) {
                if (includeNulls === true || typeof (input[p]) !== "undefined" && input[p] !== null) {
                    _flatten(input[p], output, prefix.length > 0 ? prefix + "." + p : p, includeNulls);
                }
            }
        }
        else {
            if ($.isArray(input)) {
                $.each(input, function (index, value) {
                    _flatten(value, output, "{0}[{1}]".format(prefix, index));
                });
                return;
            }
            if (!$.isFunction(input)) {
                if (input instanceof Date) {
                    output.push({ name: prefix, value: input.toISOString() });
                }
                else {
                    var val = typeof (input);
                    switch (val) {
                    case "boolean":
                    case "number":
                        val = input;
                        break;
                    case "object":
                        if (includeNulls !== true) {
                            return;
                        }
                    default:
                        val = input || "";
                    }
                    output.push({ name: prefix, value: val });
                }
            }
        }
    };

    $.extend({
        toDictionary: function (data, prefix, includeNulls) {
            data = $.isFunction(data) ? data.call() : data;
            if (arguments.length === 2 && typeof (prefix) === "boolean") {
                includeNulls = prefix;
                prefix = "";
            }
            includeNulls = typeof (includeNulls) === "boolean" ? includeNulls : false;

            var result = [];
            _flatten(data, result, prefix || "", includeNulls);

            return result;
        }
    });
})(jQuery);