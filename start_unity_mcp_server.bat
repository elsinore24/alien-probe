@echo off
echo Starting Unity MCP Server...
echo.

REM Navigate to the Unity MCP Server directory
cd /d "%USERPROFILE%\AppData\Local\Programs\UnityMCP\UnityMcpServer\src"

REM Run the server directly with Python (same as Claude Desktop does)
python server.py

REM Keep window open if there's an error
pause