// FileLogger.js - JavaScript functions for file logging
window.fileLogger = {
    downloadFile: function(filename, dataUri) {
        const link = document.createElement('a');
        link.setAttribute('href', dataUri);
        link.setAttribute('download', filename);
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    },

    saveLocalStorageData: function(key, data) {
        localStorage.setItem(key, data);
        console.log(`Data saved to localStorage: ${key}`);
    },

    getLocalStorageData: function(key) {
        return localStorage.getItem(key);
    },

    clearLocalStorageData: function(key) {
        localStorage.removeItem(key);
        console.log(`Data removed from localStorage: ${key}`);
    },

    // Log method for debugging
    log: function(level, category, message, exception) {
        const timestamp = new Date().toISOString();
        const logEntry = `[${timestamp}] [${level}] [${category}] ${message}`;
        if (exception) {
            console.error(logEntry + ` | Exception: ${exception.message}`, exception.stack);
        } else {
            console.log(logEntry);
        }
        return logEntry;
    }
};