$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

<# --------------------------------------------------- #>
function main 
{
	invoke-expression "$scriptPath\WpfApp1.exe"
}

## Entry point
main