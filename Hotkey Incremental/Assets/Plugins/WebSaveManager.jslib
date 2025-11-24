mergeInto(LibraryManager.library, {
    SaveToLocalStorage: function (keyPtr, valuePtr) {
        var key = UTF8ToString(keyPtr);
        var value = UTF8ToString(valuePtr);
        try {
            localStorage.setItem(key, value);
        } catch (e) {
            console.error("Error saving to localStorage:", e);
        }
    },

    LoadFromLocalStorage: function (keyPtr) {
        var key = UTF8ToString(keyPtr);
        try {
            var value = localStorage.getItem(key);
            if (value === null) {
                return null;
            }
            var buffer = _malloc(value.length + 1);
            stringToUTF8(value, buffer, value.length + 1);
            return buffer;
        } catch (e) {
            console.error("Error loading from localStorage:", e);
            return null;
        }
    },

    HasKeyInLocalStorage: function (keyPtr) {
        var key = UTF8ToString(keyPtr);
        try {
            return localStorage.getItem(key) !== null;
        } catch (e) {
            console.error("Error checking localStorage:", e);
            return false;
        }
    },

    DeleteFromLocalStorage: function (keyPtr) {
        var key = UTF8ToString(keyPtr);
        try {
            localStorage.removeItem(key);
        } catch (e) {
            console.error("Error deleting from localStorage:", e);
        }
    },

    ClearLocalStorage: function () {
        try {
            localStorage.clear();
        } catch (e) {
            console.error("Error clearing localStorage:", e);
        }
    },

    CopyToClipboard: function (textPtr) {
        var text = UTF8ToString(textPtr);
        try {
            // Create a temporary textarea element
            var textarea = document.createElement("textarea");
            textarea.value = text;
            textarea.style.position = "fixed";
            textarea.style.opacity = "0";
            document.body.appendChild(textarea);
            textarea.select();
            document.execCommand("copy");
            document.body.removeChild(textarea);
            console.log("Copied to clipboard");
        } catch (e) {
            console.error("Error copying to clipboard:", e);
            // Fallback: try modern clipboard API
            if (navigator.clipboard && navigator.clipboard.writeText) {
                navigator.clipboard.writeText(text).catch(function(err) {
                    console.error("Clipboard API error:", err);
                });
            }
        }
    }
});

