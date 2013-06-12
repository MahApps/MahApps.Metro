param([switch]$PushToRemote)


function Update-GithubPages($current_repository, $base_url) {

	$currentLocation = (Get-Location)

	$tempPath = [System.IO.Path]::GetTempPath()
	$guid = [guid]::NewGuid()
	$repoPath = Join-Path $tempPath $guid

	"Creating temporary folder at $repoPath"

	New-Item -Type directory $repoPath | Out-Null

	"Setting up clone of gh-pages repository"

	Set-Location -Path $repoPath

	. git clone -b gh-pages -o upstream $current_repository . | Out-Null

	"Replacing contents with generated content"

	Remove-Item * -Exclude .git -Recurse -Force
	$site_folder = Join-Path $current_repository "\docs\*"
	Copy-Item -Path $site_folder -Exclude "_site" -Recurse -Destination .

    # update template references to GitHub URL
    $template = Get-Content "_config.yml"
    $template -replace "baseurl: /", "baseurl: $base_url" | Out-File "_config.yml" -encoding "ASCII"

	. git add . | Out-Null
	. git add -u . | Out-Null
	. git commit -m "updating site from scribble generated content" | Out-Null
	. git push upstream gh-pages | Out-Null

	Set-Location -Path $currentLocation

	"Removing temporary folder at $repoPath"
	Remove-Item $repoPath -Recurse -Force | Out-Null
}

Update-GithubPages "D:\Code\github\shiftkey\MahApps.Metro\" "/MahApps.Metro/"

if ($PushToRemote) { 
	"Pushing to GitHub now"
    . git push origin gh-pages --force
}
