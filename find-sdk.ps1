param([string]$Game)
$root = Join-Path $Game 'JibbersMods\system\clr'
if (-not (Test-Path $root)) { exit 1 }
Get-ChildItem -Path $root -Recurse -Depth 1 -Filter 'Jibbers.Sdk.dll' |
  Sort-Object LastWriteTime -Descending |
  Select-Object -First 1 -ExpandProperty FullName
