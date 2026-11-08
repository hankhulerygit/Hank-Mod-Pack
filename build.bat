@echo off
chcp 65001 >nul
echo === Hank Mod Pack : compilation ===
set "GAME=C:\Program Files (x86)\Steam\steamapps\common\Jibbers Playtest"
if not "%~1"=="" set "GAME=%~1"

set "HAVESDK="
for /f "delims=" %%v in ('dotnet --list-sdks 2^>nul') do set "HAVESDK=1"
if not defined HAVESDK (
  echo.
  echo Le SDK .NET 8 n'est pas installe sur cet ordinateur ^(il y a seulement le runtime^).
  echo Installation avec winget...
  winget install --id Microsoft.DotNet.SDK.8 -e --accept-package-agreements --accept-source-agreements
  echo.
  echo Si l'installation s'est bien passee, FERME cette fenetre et relance build.bat.
  echo Sinon, telecharge le "SDK" ici : https://dotnet.microsoft.com/download/dotnet/8.0
  pause
  exit /b 1
)

set "SDK="
for /f "usebackq delims=" %%i in (`powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0find-sdk.ps1" "%GAME%"`) do set "SDK=%%i"
if not defined SDK (
  echo.
  echo Jibbers.Sdk.dll introuvable dans "%GAME%\JibbersMods\system\clr".
  echo Lance le jeu une fois, puis relance ce fichier. Autre dossier de jeu : build.bat "D:\chemin\du jeu"
  pause
  exit /b 1
)
echo SDK du jeu : %SDK%
dotnet build -c Release -p:JibbersGame="%GAME%" -p:JibbersSdk="%SDK%"
if not exist "%~dp0bin\Release\net8.0\Hank Mod Pack.dll" (
  echo.
  echo La compilation a echoue. Copie les lignes "error" ci-dessus et envoie-les moi.
  pause
  exit /b 1
)
echo.
echo Termine. Le DLL est dans bin\Release\net8.0\Hank Mod Pack.dll
echo Il est aussi copie dans JibbersMods\cs du jeu. Ferme le jeu avant de compiler, il verrouille le DLL.
pause
