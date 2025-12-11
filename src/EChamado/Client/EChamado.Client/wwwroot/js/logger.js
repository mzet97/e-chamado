// Simple Logger for EChamado Client Debug
window.EChamadoLogger = {
    _logs: [],
    
    log: function(level, category, message, exception) {
        const timestamp = new Date().toISOString();
        const logEntry = `[${timestamp}] [${level}] [${category}] ${message}`;
        
        if (exception) {
            logEntry += `\n    Exception: ${exception.message || exception}`;
            if (exception.stack) {
                logEntry += `\n    Stack: ${exception.stack}`;
            }
        }
        
        // Store in memory
        window.EChamadoLogger._logs.push(logEntry);
        
        // Also log to console
        if (level === 'ERROR') {
            console.error(logEntry);
        } else if (level === 'WARN') {
            console.warn(logEntry);
        } else {
            console.log(logEntry);
        }
        
        // Keep only last 1000 entries to prevent memory issues
        if (window.EChamadoLogger._logs.length > 1000) {
            window.EChamadoLogger._logs = window.EChamadoLogger._logs.slice(-1000);
        }
    },
    
    info: function(category, message) {
        window.EChamadoLogger.log('INFO', category, message);
    },
    
    warn: function(category, message) {
        window.EChamadoLogger.log('WARN', category, message);
    },
    
    error: function(category, message, exception) {
        window.EChamadoLogger.log('ERROR', category, message, exception);
    },
    
    auth: function(message, exception) {
        window.EChamadoLogger.log('INFO', 'AUTH', message, exception);
    },
    
    authError: function(message, exception) {
        window.EChamadoLogger.log('ERROR', 'AUTH', message, exception);
    },
    
    debug: function(category, message) {
        window.EChamadoLogger.log('DEBUG', category, message);
    },
    
    getAllLogs: function() {
        return window.EChamadoLogger._logs.join('\n');
    },
    
    clearLogs: function() {
        window.EChamadoLogger._logs = [];
    },
    
    downloadLogs: function(filename = 'echamado-debug.log') {
        const logs = window.EChamadoLogger.getAllLogs();
        if (logs.trim() === '') {
            console.warn('No logs to download');
            return;
        }
        
        const blob = new Blob([logs], { type: 'text/plain' });
        const url = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        URL.revokeObjectURL(url);
        
        console.log(`Logs downloaded to: ${filename}`);
    },
    
    // Save to localStorage for persistence
    saveToLocalStorage: function(key = 'echamado_logs') {
        const logs = window.EChamadoLogger.getAllLogs();
        localStorage.setItem(key, logs);
        console.log(`Logs saved to localStorage: ${key}`);
    },
    
    loadFromLocalStorage: function(key = 'echamado_logs') {
        const logs = localStorage.getItem(key);
        if (logs) {
            window.EChamadoLogger._logs = logs.split('\n');
            console.log(`Logs loaded from localStorage: ${key}`);
            return true;
        }
        return false;
    }
};

// Auto-initialize on load
console.log('🔧 EChamado Logger initialized');