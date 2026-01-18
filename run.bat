@echo off
chcp 65001 > nul
echo ================================================
echo ФИАС Report Generator
echo ================================================
echo.

REM Проверка установки .NET
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ОШИБКА: .NET SDK не установлен!
    pause
    exit /b 1
)

echo Сборка проекта...
dotnet build --configuration Release -p:WarningLevel=0
if errorlevel 1 (
    echo ОШИБКА при сборке проекта!
    pause
    exit /b 1
)

echo.
echo Запуск программы...
echo.
dotnet run --configuration Release

echo.
echo ================================================
echo Программа завершена
echo ================================================
echo.

REM Открываем отчеты, если они существуют
if exist "FiasReport\report_*.html" (
    for %%f in (FiasReport\report_*.html) do start "" "%%f"
)

if exist "FiasReport\report_*.docx" (
    for %%f in (FiasReport\report_*.docx) do start "" "%%f"
)

pause