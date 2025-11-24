#!/bin/bash

echo "🔧 EChamado Server Issues Fix Script"
echo "===================================="

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

echo -e "${BLUE}This script will fix common EChamado server startup issues.${NC}"

# Step 1: Kill all processes
echo -e "\n🧹 Step 1: Cleaning up all EChamado processes..."
pkill -f "EChamado" 2>/dev/null || true
pkill -f "Echamado" 2>/dev/null || true
sleep 2

echo -e "${GREEN}✅ Cleanup complete${NC}"

# Step 2: Check ports
echo -e "\n🔌 Step 2: Checking port availability..."
if lsof -i :7132 >/dev/null 2>&1; then
    echo -e "${YELLOW}⚠️ Port 7132 still in use, forcing cleanup...${NC}"
    fuser -k 7132/tcp 2>/dev/null || true
fi

if lsof -i :7296 >/dev/null 2>&1; then
    echo -e "${YELLOW}⚠️ Port 7296 still in use, forcing cleanup...${NC}"
    fuser -k 7296/tcp 2>/dev/null || true
fi

sleep 3

# Step 3: Clean build artifacts
echo -e "\n🧽 Step 3: Cleaning build artifacts..."
cd /mnt/e/TI/git/e-chamado/src/EChamado
dotnet clean >/dev/null 2>&1

echo -e "${GREEN}✅ Build artifacts cleaned${NC}"

# Step 4: Start Auth Server
echo -e "\n🚀 Step 4: Starting Auth Server..."
cd /mnt/e/TI/git/e-chamado/src/EChamado/Echamado.Auth

# Start with explicit settings
nohup dotnet run --urls https://localhost:7132 --environment Development > /tmp/fixed-auth.log 2>&1 &
AUTH_PID=$!
echo $AUTH_PID > /tmp/fixed-auth.pid

echo "Auth Server PID: $AUTH_PID"

# Wait for startup
echo "⏳ Waiting for Auth Server to start..."
sleep 8

# Check if still running
if kill -0 $AUTH_PID 2>/dev/null; then
    echo -e "${GREEN}✅ Auth Server started successfully${NC}"
    AUTH_OK=true
else
    echo -e "${RED}❌ Auth Server failed to start${NC}"
    echo "📋 Last auth server logs:"
    tail -10 /tmp/fixed-auth.log
    AUTH_OK=false
fi

# Step 5: Start API Server
echo -e "\n🚀 Step 5: Starting API Server..."
cd /mnt/e/TI/git/e-chamado/src/EChamado/Server/EChamado.Server

nohup dotnet run --urls https://localhost:7296 --environment Development > /tmp/fixed-api.log 2>&1 &
API_PID=$!
echo $API_PID > /tmp/fixed-api.pid

echo "API Server PID: $API_PID"

# Wait for startup
echo "⏳ Waiting for API Server to start..."
sleep 10

# Check if still running
if kill -0 $API_PID 2>/dev/null; then
    echo -e "${GREEN}✅ API Server started successfully${NC}"
    API_OK=true
else
    echo -e "${RED}❌ API Server failed to start${NC}"
    echo "📋 Last API server logs:"
    tail -10 /tmp/fixed-api.log
    API_OK=false
fi

# Step 6: Test functionality
echo -e "\n🧪 Step 6: Testing server functionality..."

if [ "$AUTH_OK" = true ]; then
    echo "🔐 Testing Auth Server..."
    if curl -k -s -X POST "https://localhost:7132/connect/token" \
        -H "Content-Type: application/x-www-form-urlencoded" \
        -d "grant_type=password&username=admin@admin.com&password=Admin@123&client_id=mobile-client" \
        | grep -q "access_token"; then
        echo -e "${GREEN}✅ Auth Server token generation: WORKING${NC}"
    else
        echo -e "${YELLOW}⚠️ Auth Server token generation: ISSUE${NC}"
    fi
fi

if [ "$API_OK" = true ]; then
    echo "🌐 Testing API Server..."
    if curl -k -s -X GET "https://localhost:7296/health-check" | grep -q "OK"; then
        echo -e "${GREEN}✅ API Server health check: WORKING${NC}"
    else
        echo -e "${YELLOW}⚠️ API Server health check: ISSUE${NC}"
    fi
fi

# Final summary
echo -e "\n📊 Final Status:"
echo "================"

if [ "$AUTH_OK" = true ] && [ "$API_OK" = true ]; then
    echo -e "${GREEN}🎉 SUCCESS: Both servers are running!${NC}"
    echo ""
    echo -e "${BLUE}📱 Auth Server: https://localhost:7132${NC}"
    echo -e "${BLUE}🌐 API Server: https://localhost:7296${NC}"
    echo -e "${BLUE}📋 API Docs: https://localhost:7296/api-docs/v1${NC}"
    echo ""
    echo -e "${YELLOW}💡 To monitor servers:${NC}"
    echo "   ./check-servers.sh"
    echo ""
    echo -e "${YELLOW}💡 To test endpoints:${NC}"
    echo "   ./test-auth-fix.sh"
else
    echo -e "${RED}❌ FAILURE: Some servers failed to start${NC}"
    echo ""
    echo -e "${YELLOW}🔍 Check logs for details:${NC}"
    [ "$AUTH_OK" = false ] && echo "   tail -f /tmp/fixed-auth.log"
    [ "$API_OK" = false ] && echo "   tail -f /tmp/fixed-api.log"
fi

