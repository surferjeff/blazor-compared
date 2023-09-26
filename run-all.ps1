Start-Job -Name BlazorApp -ScriptBlock { dotnet run -c Release } `
    -InitializationScript ([ScriptBlock]::Create("Set-Location '$PSScriptRoot\BlazorApp'"))
Start-Job -Name RazorApp -ScriptBlock { dotnet run -c Release } `
    -InitializationScript ([ScriptBlock]::Create("Set-Location '$PSScriptRoot\RazorApp'"))
Start-Job -Name GoApp -ScriptBlock { go run . } `
    -InitializationScript ([ScriptBlock]::Create("Set-Location '$PSScriptRoot\GoApp'"))