#!/bin/bash

# SportPlannerV2 - Service Restart Script
# Detiene y reinicia servicios de Frontend (Angular) y Backend (.NET)

set -e  # Exit on error

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Project paths
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
FRONTEND_DIR="$PROJECT_ROOT/front/SportPlanner"
BACKEND_DIR="$PROJECT_ROOT/back/SportPlanner"
BACKEND_API_DIR="$BACKEND_DIR/src/SportPlanner.API"

# Ports
FRONTEND_PORT=4200
BACKEND_HTTP_PORT=5000
BACKEND_HTTPS_PORT=5001

echo -e "${BLUE}╔════════════════════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║  SportPlannerV2 - Service Restart Script              ║${NC}"
echo -e "${BLUE}╚════════════════════════════════════════════════════════╝${NC}"
echo ""

# Function: Kill process by port
kill_port() {
    local port=$1
    local process_name=$2

    echo -e "${YELLOW}🔍 Checking port $port for $process_name...${NC}"

    # Windows (Git Bash)
    if [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "win32" ]]; then
        local pid=$(netstat -ano | grep ":$port " | awk '{print $5}' | head -n 1)
        if [ -n "$pid" ] && [ "$pid" != "0" ]; then
            echo -e "${YELLOW}   Killing process $pid on port $port${NC}"
            taskkill //PID $pid //F 2>/dev/null || true
        else
            echo -e "${GREEN}   ✓ Port $port is free${NC}"
        fi
    # Linux/macOS
    else
        local pid=$(lsof -ti:$port 2>/dev/null || true)
        if [ -n "$pid" ]; then
            echo -e "${YELLOW}   Killing process $pid on port $port${NC}"
            kill -9 $pid 2>/dev/null || true
        else
            echo -e "${GREEN}   ✓ Port $port is free${NC}"
        fi
    fi
}

# Function: Kill processes by name
kill_by_name() {
    local process_name=$1
    echo -e "${YELLOW}🔍 Checking for running '$process_name' processes...${NC}"

    if [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "win32" ]]; then
        # Windows
        taskkill //IM "$process_name.exe" //F 2>/dev/null || echo -e "${GREEN}   ✓ No $process_name processes found${NC}"
    else
        # Linux/macOS
        pkill -f "$process_name" 2>/dev/null && echo -e "${GREEN}   ✓ Killed $process_name processes${NC}" || echo -e "${GREEN}   ✓ No $process_name processes found${NC}"
    fi
}

# Step 1: Stop existing processes
echo -e "\n${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "${BLUE}  Step 1: Stopping existing services${NC}"
echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"

# Kill Angular processes
kill_by_name "node"
kill_port $FRONTEND_PORT "Angular"

# Kill .NET processes
kill_by_name "dotnet"
kill_port $BACKEND_HTTP_PORT ".NET HTTP"
kill_port $BACKEND_HTTPS_PORT ".NET HTTPS"

sleep 2

# Step 2: Start Frontend (Angular)
echo -e "\n${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "${BLUE}  Step 2: Starting Frontend (Angular)${NC}"
echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"

if [ ! -d "$FRONTEND_DIR" ]; then
    echo -e "${RED}❌ Frontend directory not found: $FRONTEND_DIR${NC}"
    exit 1
fi

cd "$FRONTEND_DIR"

# Check if node_modules exists
if [ ! -d "node_modules" ]; then
    echo -e "${YELLOW}⚠️  node_modules not found. Running npm install...${NC}"
    npm install
fi

echo -e "${GREEN}🚀 Starting Angular dev server (http://localhost:4200)${NC}"
echo -e "${YELLOW}   Running: npm start${NC}"

# Start Angular in background
npm start > /tmp/angular-dev.log 2>&1 &
ANGULAR_PID=$!

echo -e "${GREEN}   ✓ Angular dev server started (PID: $ANGULAR_PID)${NC}"
echo -e "${YELLOW}   📋 Logs: /tmp/angular-dev.log${NC}"

# Step 3: Start Backend (.NET)
echo -e "\n${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "${BLUE}  Step 3: Starting Backend (.NET API)${NC}"
echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"

if [ ! -d "$BACKEND_API_DIR" ]; then
    echo -e "${RED}❌ Backend API directory not found: $BACKEND_API_DIR${NC}"
    exit 1
fi

cd "$BACKEND_API_DIR"

echo -e "${GREEN}🚀 Starting .NET API (https://localhost:5001)${NC}"
echo -e "${YELLOW}   Running: dotnet run${NC}"

# Start .NET in background
dotnet run > /tmp/dotnet-api.log 2>&1 &
DOTNET_PID=$!

echo -e "${GREEN}   ✓ .NET API started (PID: $DOTNET_PID)${NC}"
echo -e "${YELLOW}   📋 Logs: /tmp/dotnet-api.log${NC}"

# Step 4: Wait for services to be ready
echo -e "\n${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "${BLUE}  Step 4: Waiting for services to be ready${NC}"
echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"

sleep 5

# Check if Angular is responding
echo -e "${YELLOW}🔍 Checking Angular dev server...${NC}"
if curl -s http://localhost:4200 > /dev/null 2>&1; then
    echo -e "${GREEN}   ✓ Angular is responding on http://localhost:4200${NC}"
else
    echo -e "${YELLOW}   ⚠️  Angular might still be starting (check logs)${NC}"
fi

# Check if .NET is responding
echo -e "${YELLOW}🔍 Checking .NET API...${NC}"
if curl -k -s https://localhost:5001/api > /dev/null 2>&1; then
    echo -e "${GREEN}   ✓ .NET API is responding on https://localhost:5001${NC}"
else
    echo -e "${YELLOW}   ⚠️  .NET API might still be starting (check logs)${NC}"
fi

# Final summary
echo -e "\n${GREEN}╔════════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║  ✅ Services Started Successfully                      ║${NC}"
echo -e "${GREEN}╚════════════════════════════════════════════════════════╝${NC}"
echo ""
echo -e "${BLUE}📍 Frontend (Angular):${NC}  http://localhost:4200"
echo -e "${BLUE}📍 Backend (.NET):${NC}     https://localhost:5001"
echo ""
echo -e "${YELLOW}📋 Logs:${NC}"
echo -e "   Frontend: /tmp/angular-dev.log"
echo -e "   Backend:  /tmp/dotnet-api.log"
echo ""
echo -e "${YELLOW}🔍 Monitor logs:${NC}"
echo -e "   tail -f /tmp/angular-dev.log"
echo -e "   tail -f /tmp/dotnet-api.log"
echo ""
echo -e "${YELLOW}⏹️  Stop services:${NC}"
echo -e "   kill $ANGULAR_PID $DOTNET_PID"
echo ""
echo -e "${GREEN}✨ Happy coding!${NC}"
echo ""
