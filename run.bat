@echo off
chcp 65001 > nul

cd /d "%~dp0"

echo ================================================
echo Report Generator
echo ================================================
echo.

dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ОШИБКА: .NET SDK не установлен!
    pause
    exit /b 1
)

echo Сборка проекта...
dotnet build --configuration Release -p:NoWarn=* -v:q
if errorlevel 1 (
    echo ОШИБКА при сборке проекта!
    pause
    exit /b 1
)
echo.

echo Запуск программы...
cls
echo ================================================
echo Report Generator
echo ================================================
echo.
dotnet run --no-build --configuration Release


echo.
echo ================================================
echo Программа завершена
echo ================================================
echo.

for /f "delims=" %%f in ('dir "FiasReports\report_*.html" /b /o-d 2^>nul') do (
    echo Открытие %%f
    start "" "FiasReports\%%f"
    goto :docx
)

:docx
for /f "delims=" %%f in ('dir "FiasReports\report_*.docx" /b /o-d 2^>nul') do (
    echo Открытие %%f
    start "" "FiasReports\%%f"
    goto :end
)

:end
pause