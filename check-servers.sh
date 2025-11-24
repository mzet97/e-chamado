#!/bin/bash

echo "🔍 EChamado Server Status Check"
echo "==============================="

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Check Auth Server
echo "🔐 Checking Auth Server..."
if pgrep -f "Echamado.Auth" >/dev/null; then
    AUTH_PID=$(pgrep -f "Echamado.Auth" | head -1)
    echo -e "${GREEN}✅ Auth Server is running (PID: $AUTH_PID)${NC}"
    
    # Test token endpoint
    if curl -k -s -X POST "https://localhost:7132/connect/token" \
        -H "Content-Type: application/x-www-form-urlencoded" \
        -d "grant_type=password&username=test&password=test&client_id=mobile-client" \
        -w "%{http_code}" -o /dev/null | grep -q "400"; then
        echo -e "${GREEN}✅ Auth Server endpoints responding${NC}"
    else
        echo -e "${YELLOW}⚠️  Auth Server endpoints not responding properly${NC}"
    fi
else
    echo -e "${RED}❌ Auth Server is NOT running${NC}"
fi

echo -e "\n🌐 Checking API Server..."
if pgrep -f "EChamado.Server" >/dev/null; then
    API_PID=$(pgrep -f "EChamado.Server" | head -1)
    echo -e "${GREEN}✅ API Server is running (PID: $API_PID)${NC}"
    
    # Test health endpoint
    HEALTH_CODE=$(curl -k -s -X GET "https://localhost:7296/health-check" \
        -w "%{http_code}" -o /dev/null)
    
    if [ "$HEALTH_CODE" = "200" ]; then
        echo -e "${GREEN}✅ API Server health check passing${NC}"
    else
        echo -e "${YELLOW}⚠️  API Server health check returned: $HEALTH_CODE${NC}"
    fi
else
    echo -e "${RED}❌ API Server is NOT running${NC}"
fi

# Check ports
echo -e "\n🔌 Checking ports..."
if lsof -i :7132 >/dev/null 2>&1; then
    echo -e "${GREEN}✅ Port 7132 (Auth) is in use${NC}"
else
    echo -e "${RED}❌ Port 7132 (Auth) is FREE${NC}"
fi

if lsof -i :7296 >/dev/null 2>&1; then
    echo -e "${GREEN}✅ Port 7296 (API) is in use${NC}"
else
    echo -e "${RED}❌ Port 7296 (API) is FREE${NC}"
fi

# Show recent logs if servers are running
echo -e "\n📋 Recent Auth Server logs:"
if [ -f "/tmp/authserver.log" ]; then
    tail -3 "/tmp/authserver.log"
else
    echo "No log file found"
fi

echo -e "\n📋 Recent API Server logs:"
if [ -f "/tmp/apiserver.log" ]; then
    tail -3 "/tmp/apiserver.log"
else
    echo "No log file found"
fi

