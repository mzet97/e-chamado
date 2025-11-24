#!/bin/bash

echo "🚀 EChamado Server Startup Script"
echo "================================="

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to check if port is in use
check_port() {
    local port=$1
    local service=$2
    if lsof -i :$port >/dev/null 2>&1; then
        echo -e "${YELLOW}⚠️  $service port $port is already in use${NC}"
        echo "🔪 Killing existing processes..."
        pkill -f "EChamado" 2>/dev/null || true
        sleep 3
    fi
}

# Function to start server in background
start_server() {
    local project=$1
    local port=$2
    local name=$3
    
    echo -e "\n📦 Starting $name..."
    cd "$project"
    
    # Start server in background with logging
    nohup dotnet run --urls https://localhost:$port > "/tmp/${name,,}.log" 2>&1 &
    local pid=$!
    
    echo "$pid" > "/tmp/${name,,}.pid"
    echo "✅ $name started with PID: $pid"
    
    # Wait for startup
    sleep 5
    
    # Check if still running
    if kill -0 $pid 2>/dev/null; then
        echo -e "${GREEN}✅ $name is running successfully${NC}"
        return 0
    else
        echo -e "${RED}❌ $name failed to start${NC}"
        echo "📋 Last log entries:"
        tail -5 "/tmp/${name,,}.log"
        return 1
    fi
}

# Main execution
echo "🔍 Checking and cleaning up processes..."
check_port 7132 "Auth Server"
check_port 7296 "API Server"

echo -e "\n🎯 Starting servers..."

# Start Auth Server
start_server "/mnt/e/TI/git/e-chamado/src/EChamado/Echamado.Auth" 7132 "AuthServer"
auth_success=$?

# Start API Server  
start_server "/mnt/e/TI/git/e-chamado/src/EChamado/Server/EChamado.Server" 7296 "ApiServer"
api_success=$?

# Final status
echo -e "\n📊 Startup Summary:"
echo "==================="

if [ $auth_success -eq 0 ]; then
    echo -e "${GREEN}✅ Auth Server: READY${NC}"
else
    echo -e "${RED}❌ Auth Server: FAILED${NC}"
fi

if [ $api_success -eq 0 ]; then
    echo -e "${GREEN}✅ API Server: READY${NC}"
else
    echo -e "${RED}❌ API Server: FAILED${NC}"
fi

if [ $auth_success -eq 0 ] && [ $api_success -eq 0 ]; then
    echo -e "\n${GREEN}🎉 All servers are running successfully!${NC}"
    echo "📱 Auth Server: https://localhost:7132"
    echo "🌐 API Server: https://localhost:7296"
    echo "📋 API Docs: https://localhost:7296/api-docs/v1"
else
    echo -e "\n${RED}❌ Some servers failed to start. Check logs above.${NC}"
    exit 1
fi

