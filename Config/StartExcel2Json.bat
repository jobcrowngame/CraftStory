echo off

cd Release

ExcelToJson.exe ..\Excel\ ..\..\CraftStoryClient\Assets\Resources\Config\
ExcelToJson.exe ..\Excel\ ..\..\..\..\ProgramFiles\Apache24\htdocs\Server\PHP\Local\Json\
ExcelToJson.exe ..\Excel\ ..\Json\ true