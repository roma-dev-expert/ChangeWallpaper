@echo off
REM Проверка наличия Python
python --version >nul 2>&1
if %errorlevel% neq 0 (
    echo Python не установлен на вашем компьютере.
    echo Установите Python и повторите попытку.
    exit /b
)

REM Установка пакета wallpaperscraft с помощью pip
pip3 install wallpaperscraft