# setup tool dependencies (windows)
# 1. must have java installed and java/bin added to path environment variable

# setup tool (windows)
# https://jeremylong.github.io/DependencyCheck/dependency-check-cli/index.html
# 1. download from Github release page
# 2. Extract the zip file to a location on your computer 
# 3. put the ‘bin’ directory into the path environment variable

Clear-Host
$scriptPath = (Get-Location);
$rootPath =(get-item $scriptPath).parent.parent.FullName
$solutionPath = $rootPath+"\resource\webapp\Portal"
Write-Output $solutionPath
dependency-check.bat `
    --project "portal-local-test" `
    --scan $solutionPath `
    --format HTML `
    --failOnCVSS 8 `
    --suppression $rootPath"\owasp-dependency-suppression-file.xml" `
    --out $scriptPath"\owasp-dependency-check-report.html" `
    --enableExperimental