#!/bin/bash

echo "🔐 EChamado Authentication Debug Script"
echo "======================================="
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}📍 Testing Authentication Flow Steps${NC}"
echo ""

echo -e "${YELLOW}1. Testing Auth Server Connectivity${NC}"
if curl -s -f http://localhost:7132/.well-known/openid-configuration > /dev/null; then
    echo -e "${GREEN}✅ Auth Server OpenID Config: OK${NC}"
else
    echo -e "${RED}❌ Auth Server OpenID Config: FAIL${NC}"
    exit 1
fi

echo ""
echo -e "${YELLOW}2. Testing Token Generation${NC}"
TOKEN_RESPONSE=$(curl -s -X POST http://localhost:7132/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password&username=admin@admin.com&password=Admin@123&client_id=mobile-client&scope=openid profile email roles api chamados")

if echo "$TOKEN_RESPONSE" | grep -q "access_token"; then
    echo -e "${GREEN}✅ Token Generation: OK${NC}"
    
    # Extract token
    ACCESS_TOKEN=$(echo "$TOKEN_RESPONSE" | grep -o '"access_token":"[^"]*' | cut -d'"' -f4)
    echo "🔑 Token: ${ACCESS_TOKEN:0:50}..."
    
    echo ""
    echo -e "${YELLOW}3. Testing Token Structure${NC}"
    
    # Check if token has proper JWT structure
    TOKEN_PARTS=$(echo "$ACCESS_TOKEN" | tr '.' '\n' | wc -l)
    if [ $TOKEN_PARTS -eq 3 ]; then
        echo -e "${GREEN}✅ JWT Structure: Valid (3 parts)${NC}"
        
        # Try to decode payload
        echo ""
        echo -e "${YELLOW}4. Analyzing JWT Claims${NC}"
        
        PAYLOAD=$(echo "$ACCESS_TOKEN" | cut -d'.' -f2)
        
        # Add padding for base64 decode
        case $((${#PAYLOAD} % 4)) in
            2) PAYLOAD="${PAYLOAD}==" ;;
            3) PAYLOAD="${PAYLOAD}=" ;;
        esac
        
        DECODED=$(echo "$PAYLOAD" | base64 -d 2>/dev/null)
        if [ $? -eq 0 ]; then
            echo -e "${GREEN}✅ JWT Payload Decoded Successfully${NC}"
            
            # Check for key claims
            if echo "$DECODED" | grep -q '"sub"'; then
                USER_ID=$(echo "$DECODED" | grep -o '"sub":"[^"]*' | cut -d'"' -f4)
                echo "👤 User ID: $USER_ID"
            fi
            
            if echo "$DECODED" | grep -q '"email"'; then
                EMAIL=$(echo "$DECODED" | grep -o '"email":"[^"]*' | cut -d'"' -f4)
                echo "📧 Email: $EMAIL"
            fi
            
            if echo "$DECODED" | grep -q '"aud"'; then
                AUDIENCE=$(echo "$DECODED" | grep -o '"aud":"[^"]*' | cut -d'"' -f4)
                echo "🎯 Audience: $AUDIENCE"
            fi
            
            if echo "$DECODED" | grep -q '"exp"'; then
                EXPIRY=$(echo "$DECODED" | grep -o '"exp":[0-9]*' | cut -d':' -f2)
                EXPIRY_DATE=$(date -d "@$EXPIRY" 2>/dev/null || echo "Unknown")
                echo "⏰ Expires: $EXPIRY_DATE"
            fi
            
        else
            echo -e "${RED}❌ Failed to decode JWT payload${NC}"
        fi
        
    else
        echo -e "${RED}❌ JWT Structure: Invalid ($TOKEN_PARTS parts, expected 3)${NC}"
    fi
    
else
    echo -e "${RED}❌ Token Generation: FAIL${NC}"
    echo "Response: $TOKEN_RESPONSE"
    exit 1
fi

echo ""
echo -e "${YELLOW}5. Testing Blazor Client Application${NC}"
if curl -s -f http://localhost:7274/ > /dev/null; then
    echo -e "${GREEN}✅ Blazor Client: Accessible${NC}"
    
    # Check for debug features
    if curl -s http://localhost:7274/ | grep -q "logger.js"; then
        echo -e "${GREEN}✅ JavaScript Logger: Included${NC}"
    else
        echo -e "${YELLOW}⚠️  JavaScript Logger: Not Found${NC}"
    fi
    
    # Check OAuth test page
    if curl -s -f http://localhost:7274/test-oauth-flow.html > /dev/null; then
        echo -e "${GREEN}✅ OAuth Test Page: Available${NC}"
    else
        echo -e "${YELLOW}⚠️  OAuth Test Page: Not Found${NC}"
    fi
    
else
    echo -e "${RED}❌ Blazor Client: Not Accessible${NC}"
fi

echo ""
echo -e "${BLUE}🧪 Authentication Debugging Scenarios${NC}"
echo ""
echo "If 'Not authorized' occurs, check these potential issues:"
echo ""
echo "1. ${YELLOW}Token Storage Issue${NC}"
echo "   - Does localStorage.setItem('authToken', token) succeed?"
echo "   - Check browser localStorage for 'authToken'"
echo ""
echo "2. ${YELLOW}JWT Parsing Issue${NC}"
echo "   - Does CookieAuthenticationStateProvider parse JWT correctly?"
echo "   - Check if GetAuthenticationStateAsync() returns authenticated state"
echo ""
echo "3. ${YELLOW}Navigation Issue${NC}"
echo "   - Does LoginCallback redirect properly after OAuth?"
echo "   - Check if authentication state triggers re-render"
echo ""
echo "4. ${YELLOW}PKCE Flow Issue${NC}"
echo "   - Does code_challenge generate correctly?"
echo "   - Does code_verifier match during token exchange?"
echo ""
echo -e "${BLUE}📋 Debug Tools Available${NC}"
echo ""
echo "1. Browser Console Logs"
echo "   - Check for JavaScript errors"
echo "   - Monitor authentication flow logs"
echo ""
echo "2. Home Page Debug Panel"
echo "   - Navigate to http://localhost:7274/"
echo "   - Use debug buttons to test auth state"
echo "   - Download logs with '📥 Baixar Logs' button"
echo ""
echo "3. OAuth Test Page"
echo "   - Open http://localhost:7274/test-oauth-flow.html"
echo "   - Test complete OAuth PKCE flow"
echo "   - Analyze detailed debug output"
echo ""

echo -e "${GREEN}🎯 Next Step: Open browser and test authentication flow${NC}"
echo "   URL: http://localhost:7274"
echo "   Check console and download logs to identify exact failure point"

# Save token for manual testing
if [ -n "$ACCESS_TOKEN" ]; then
    echo "$ACCESS_TOKEN" > /tmp/echamado_access_token.txt
    echo -e "${GREEN}💾 Access token saved to /tmp/echamado_access_token.txt${NC}"
fi

echo ""
echo "🔍 Script completed successfully!"