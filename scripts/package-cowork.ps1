param(
    [Parameter(Mandatory)]
    [guid] $AppId,

    [Parameter(Mandatory)]
    [string] $McpServerUrl,

    [Parameter(Mandatory)]
    [string] $OAuthReferenceId,

    [Parameter(Mandatory)]
    [string] $DeveloperName,

    [Parameter(Mandatory)]
    [string] $DeveloperWebsiteUrl,

    [Parameter(Mandatory)]
    [string] $PrivacyUrl,

    [Parameter(Mandatory)]
    [string] $TermsOfUseUrl,

    [string] $OutputPath = "artifacts/cowork/oge-refining-case-cowork.zip"
)

$ErrorActionPreference = "Stop"
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path

function Assert-HttpsUrl([string] $Value, [string] $Name) {
    $parsed = $null
    if (-not [uri]::TryCreate($Value, [System.UriKind]::Absolute, [ref] $parsed) -or
        $parsed.Scheme -ne "https") {
        throw "$Name must be an absolute HTTPS URL."
    }
    return $parsed
}

$mcpUri = Assert-HttpsUrl $McpServerUrl "McpServerUrl"
if ($mcpUri.AbsolutePath.TrimEnd("/") -notmatch "/mcp$") {
    throw "McpServerUrl must point to the API's /mcp endpoint."
}
if ([string]::IsNullOrWhiteSpace($OAuthReferenceId) -or
    $OAuthReferenceId -eq "REPLACE-WITH-OAUTH-CONFIGURATION-ID") {
    throw "OAuthReferenceId must be the ID of the OAuth configuration in the Microsoft Enterprise Token Store."
}
if ([string]::IsNullOrWhiteSpace($DeveloperName)) {
    throw "DeveloperName cannot be empty."
}

[void](Assert-HttpsUrl $DeveloperWebsiteUrl "DeveloperWebsiteUrl")
[void](Assert-HttpsUrl $PrivacyUrl "PrivacyUrl")
[void](Assert-HttpsUrl $TermsOfUseUrl "TermsOfUseUrl")

$templatePath = Join-Path $repoRoot "cowork\appPackage\manifest.template.json"
$toolsPath = Join-Path $repoRoot "cowork\tools\case-app-tools.json"
$colorIconPath = Join-Path $repoRoot "cowork\appPackage\color.png"
$outlineIconPath = Join-Path $repoRoot "cowork\appPackage\outline.png"
foreach ($path in @($templatePath, $toolsPath, $colorIconPath, $outlineIconPath)) {
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        throw "Required Cowork package input was not found: $path"
    }
}

$manifest = Get-Content -LiteralPath $templatePath -Raw | ConvertFrom-Json
$manifest.id = $AppId.ToString()
$manifest.developer.name = $DeveloperName
$manifest.developer.websiteUrl = $DeveloperWebsiteUrl
$manifest.developer.privacyUrl = $PrivacyUrl
$manifest.developer.termsOfUseUrl = $TermsOfUseUrl
$remoteMcpServer = $manifest.agentConnectors[0].toolSource.remoteMcpServer
$remoteMcpServer.mcpServerUrl = $mcpUri.AbsoluteUri.TrimEnd("/")
$remoteMcpServer.authorization.referenceId = $OAuthReferenceId
$manifestJson = $manifest | ConvertTo-Json -Depth 20

if ([System.IO.Path]::IsPathRooted($OutputPath)) {
    $archivePath = [System.IO.Path]::GetFullPath($OutputPath)
} else {
    $archivePath = [System.IO.Path]::GetFullPath((Join-Path $repoRoot $OutputPath))
}
$archiveDirectory = Split-Path -Parent $archivePath
New-Item -ItemType Directory -Path $archiveDirectory -Force | Out-Null

$stagingPath = Join-Path $env:TEMP "oge-cowork-package-$([guid]::NewGuid())"
New-Item -ItemType Directory -Path $stagingPath | Out-Null
try {
    New-Item -ItemType Directory -Path (Join-Path $stagingPath "tools") | Out-Null
    Set-Content -LiteralPath (Join-Path $stagingPath "manifest.json") -Value $manifestJson -Encoding UTF8
    Copy-Item -LiteralPath $colorIconPath -Destination $stagingPath
    Copy-Item -LiteralPath $outlineIconPath -Destination $stagingPath
    Copy-Item -LiteralPath $toolsPath -Destination (Join-Path $stagingPath "tools\case-app-tools.json")
    Compress-Archive -Path (Join-Path $stagingPath "*") -DestinationPath $archivePath -Force
} finally {
    Remove-Item -LiteralPath $stagingPath -Recurse -Force
}

Write-Output "Created Cowork app package: $archivePath"
