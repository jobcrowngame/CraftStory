echo off

cd Release

ExcelToJson.exe ..\Excel\ ..\..\CraftStoryClient\Assets\Resources\Config\
ExcelToJson.exe ..\Excel\ ..\Json\ true