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
    }
});

