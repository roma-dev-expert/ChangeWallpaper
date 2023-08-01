@echo off
REM Checking for the presence of Python
python --version >nul 2>&1
if %errorlevel% neq 0 (
    echo Python is not installed on your computer.
    echo Please install Python and try again.
    exit /b
)

REM Installing 'wallpaperscraft' package using pip3
pip3 install wallpaperscraft