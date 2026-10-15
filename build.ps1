#!/usr/bin/env pwsh
[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'NumberFileGenerator.csproj'
$output = Join-Path $PSScriptRoot 'publish\FrameworkDependent'
dotnet publish $project -c Release --self-contained false -o $output
if ($LASTEXITCODE -ne 0) { throw "Publishing failed with exit code $LASTEXITCODE." }
Write-Host "Published $output\NumberFileGenerator.exe"
